using System.ComponentModel;

namespace DocumentManagementService.ViewModels
{
    //Базовая модель представления, реализующая интерфейс уведомления необходимый для автоматического обновления представления
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
