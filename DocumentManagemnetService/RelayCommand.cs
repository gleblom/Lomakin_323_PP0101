using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocumentManagementService
{
    public class RelayCommand : ICommand
    {
        private readonly Action execute; //Выполняет логику команды

        private readonly Predicate<object> canExecute; //Определяет, может ли выполниться команда

        public event EventHandler CanExecuteChanged //Событие, уведомляющие, что canExecute изменился
        {
            add { CommandManager.RequerySuggested += value; }
            remove {  CommandManager.RequerySuggested -= value;}
        } 
        public RelayCommand(Action execute, Predicate<object> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute();
        }
    }
}
