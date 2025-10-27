﻿using DocumentManagementService.DTO;
using DocumentManagementService.Models;
using DocumentManagementService.Views;
using DocumentManagemnetService;
using QuickGraph;
using Supabase;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class RoutesViewModel : BaseViewModel
    {
        private readonly Client client;
        private readonly DocumentService documentService;
        private readonly INavigationService navigationService;
        private readonly ViewDocument document;
        private readonly GraphService graphService;

        public ICommand CreateRouteCommand { get; }
        public ICommand OnApproveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand EditRouteCommand { get; }

        public ObservableCollection<ApprovalRoute> Routes { get; } = [];
        public ObservableCollection<RouteStep> Steps { get; } = [];
        public IBidirectionalGraph<RouteNode, RouteEdge> Graph { get; set; }
        public Action ShowAction { get; set; }
        private ApprovalRoute selectedRoute;
        public ApprovalRoute? SelectedRoute
        {
            get { return selectedRoute;}
            set 
            {
                if (selectedRoute != value) 
                {
                    selectedRoute = value;
                    OnPropertyChanged();
                    if (selectedRoute != null)
                    {
                       Graph = graphService.LoadRoute(selectedRoute?.GraphJson);
                       //graphService.UpdateRoute(selectedRoute);
                    }
                }             
            } 
        }


        public RoutesViewModel(INavigationService navigationService) {
            CreateRouteCommand = new RelayCommand(ConfirmSelection, obj => !App.IsWindowOpen<RouteEditorWindow>());
            EditRouteCommand = new RelayCommand(OpenRoutesEditorWindow, obj => selectedRoute != null && !App.IsWindowOpen<RouteEditorWindow>());
            OnApproveCommand = new RelayCommand(OnApprove, obj => selectedRoute != null);
            CancelCommand = new RelayCommand(Back);

            client = App.SupabaseService.Client;
            documentService = new(client);
            document = App.SelectedDocument;
            graphService = new();
            this.navigationService = navigationService;

            LoadRoutes();
            LoadUserInfo();

 
        }

        public void BuildGraph()
        {
            Graph = graphService.BuildGraph();
            OnPropertyChanged(nameof(Graph));
        }
        private void Unselect()
        {
            SelectedRoute = null;
            Steps.Clear();
            BuildGraph();
        }
        //private void LoadRoute(string json)
        //{
        //    graphService.LoadRoute(json);
        //}
        public async Task<User?> LoadUserInfo()
        {
            var model = await client.From<User>().
                 Where(x => x.Email == client.Auth.CurrentUser.Email).
                 Get();
            return model.Model;

        }
        private async void OpenRoutesEditorWindow()
        {
            var user = await LoadUserInfo();
            if (user.Role == 1)
            {
                var window = new RouteEditorWindow();
                RouteEditorViewModel vm = new(selectedRoute);
                window.DataContext = vm;
                vm.UpdateAction ??= new Action(LoadRoutes);
                vm.UnselectAction ??= new Action(Unselect);
                vm.RouteName = selectedRoute.Name;
                window.Show();
            }
        }
        //private async void LoadUserInfo()
        //{
        //    var user = await client.From<User>().
        //          Where(x => x.Email == client.Auth.CurrentUser.Email).
        //          Get();
        //    if (user.Model != null) 
        //    {
        //        if(user.Model.Role == 1)
        //        {
        //            ShowAction(); //Если пользователь админ, отображаем кнопки создания и редактирования
        //        }
        //    }
        //}

        private async void LoadRoutes()
        {
            var response = await client
                .From<ApprovalRoute>()
                .Select("*")
                .Get();

            Routes.Clear();
            foreach (var route in response.Models)
            {
                Routes.Add(route);
            }
        }
        private void ConfirmSelection() 
        {
            var window = new RouteEditorWindow();
            RouteEditorViewModel vm = new();    
            window.DataContext = vm;
            vm.UpdateAction ??= new Action(LoadRoutes);
            vm.UnselectAction ??= new Action(Unselect);
            window.Show();
        }
        private void Back()
        {
            navigationService.Navigate("MyDocuments");
        }
        private async void OnApprove()
        {
           await documentService.OnApprove(document, SelectedRoute);
        }
    }
}
