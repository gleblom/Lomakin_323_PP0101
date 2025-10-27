﻿using DocumentManagementService.Models;
using DocumentManagemnetService;
using Supabase;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Category = DocumentManagementService.Models.Category;

namespace DocumentManagementService.ViewModels
{
    public class MyDocumentsViewModel : BaseViewModel
    {
        private readonly Client client;
        private readonly INavigationService navigationService;
        private ObservableCollection<ViewDocument> Documents { get; } = [];
        public ObservableCollection<Category> Categories { get; } = [];
        public ICollectionView FilteredDocuments { get; }
        private ViewDocument selectedDocument;
        public ViewDocument SelectedDocument
        {
            get { return selectedDocument; }
            set
            {
                if (selectedDocument != value)
                {

                    selectedDocument = value;
                    OnPropertyChanged();

                    App.SelectedDocument = SelectedDocument;
                    navigationService.Navigate("Viewer"); //Когда пользователь выбирает пункт меню, вызывается переход
                }
            }
        }
        private string title;
        public string Title 
        {
            get { return title; }
            set
            {
                title = value; 
                OnPropertyChanged();
                ApplyFilters();
            }
        }
        private DateTime? fromDate;
        public DateTime? FromDate
        {
            get{ return fromDate; }
            set
            {
                fromDate = value; 
                OnPropertyChanged();
                ApplyFilters();

            }
        }
        private DateTime? toDate;
        public DateTime? ToDate 
        {
            get { return toDate; }
            set 
            {
                toDate = value;
                OnPropertyChanged();
                ApplyFilters();
            } 
        }
        public MyDocumentsViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            client = App.SupabaseService.Client;

            LoadDocuments();
            LoadCategories();

            FilteredDocuments = new CollectionViewSource 
            {
                Source = Documents

            }.View;

            FilteredDocuments.Filter = obj => FilterDocument(obj as ViewDocument);

        }
        private void ApplyFilters() => FilteredDocuments.Refresh();
        public async void LoadCategories()
        {
            Categories.Clear();
            var categories = await client.From<Category>().Get();
            foreach (var category in categories.Models)
            {
                Categories.Add(category);
            }
            foreach (var item in Categories)
            {
                item.PropertyChanged += (s, e) => ApplyFilters();
            }
        }
        private async void LoadDocuments()
        {
            Documents.Clear();
            var documents = await client.From<ViewDocument>().
                Where(x => x.AuthorId == client.Auth.CurrentUser.Id).
                Get();
            foreach (var document in documents.Models)
            {
                Documents.Add(document);
            }
        }
        private bool FilterDocument(ViewDocument doc)
        {
            if (doc == null) return false;

            // Фильтр по названию
            if (!string.IsNullOrWhiteSpace(Title) &&
                !doc.Title.Contains(Title, StringComparison.OrdinalIgnoreCase))
                return false;

            // Фильтр по категориям
            if (Categories.Any(c => c.IsChecked) &&
                !Categories.Where(c => c.IsChecked).Any(c => c.CategoryName == doc.Category))
                return false;

            // Фильтр по дате
            if (FromDate.HasValue && doc.CreatedAt < FromDate.Value)
                return false;
            if (ToDate.HasValue && doc.CreatedAt > ToDate.Value)
                return false;

            return true;
        }
    }
}
