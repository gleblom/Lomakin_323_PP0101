using DocumentManagementService.Models;
using DocumentManagementService.ViewModels;
using DocumentManagemnetService;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace DocumentManagementService.Views
{

    public partial class MyDocumentsView : UserControl
    {
        private string selectedFilePath;
        private ViewDocument selectedDocument;
        private readonly DocumentService documentService;
        private readonly INavigationService navigationService;
        public MyDocumentsView()
        {
            InitializeComponent();
            navigationService = App.NavigationService;
            MyDocumentsViewModel vm = new();
            DataContext = vm;
        }

        private void Button_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button && button.DataContext is ViewDocument document)
            {
                var menu = new ContextMenu();

                selectedDocument = document;

                switch (document.Status)
                {
                    case "Не согласован":
                        menu.Items.Add(
                            new MenuItem 
                        { 
                            Header = "Отправить на согласование", 
                            Command = new RelayCommand(OnApprove)
                        });
                        menu.Items.Add(
                            new MenuItem 
                        { 
                            Header = "Удалить", 
                            Command = new RelayCommand(Delete)
                        });
                        menu.Items.Add(
                            new MenuItem
                            {
                                Header = "Загрузить новую версию",
                                Command = new RelayCommand(Update)

                            });
                        break;

                    case "Согласован":
                        menu.Items.Add(
                            new MenuItem 
                            {
                                Header = "Загрузить новую версию", 
                                Command = new RelayCommand(Update) 
                            });
                        break;

                    default:
                        menu.Items.Add(
                            new MenuItem 
                            { 
                                Header = "Нет доступных действий" 
                            });
                        break;
                }

                button.ContextMenu = menu;
                menu.PlacementTarget = button;
                menu.IsOpen = true;
            }
        }
        private async void Update()
        {
            var dialoig = new OpenFileDialog()
            {
                Filter = "Документы (*.pdf;*.docx)|*.pdf;*.docx",
                Multiselect = false
            };
            if (dialoig.ShowDialog() == true)
            {
                selectedFilePath = dialoig.FileName;
            }
            var success = await documentService.Update(selectedFilePath, selectedDocument);
            if (success)
            {
                MessageBox.Show("Документ сохранен", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении документа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Delete()
        {
 
        }

        private void OnApprove()
        {
            navigationService.Navigate("RouteView");
        }

    }
}
