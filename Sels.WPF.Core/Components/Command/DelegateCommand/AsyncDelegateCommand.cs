using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.General.Validation;

namespace Sels.WPF.Core.Components.Command.DelegateCommand
{
    public class AsyncDelegateCommand : ICommand
    {
        // Properties
        public Func<bool> CanExecuteDelegate { get; set; }
        public Func<Task> ExecuteDelegate { get; set; }

        // Events
        public event EventHandler CanExecuteChanged = delegate { };
        public AsyncDelegateCommand(Func<Task> executeDelegate, Func<bool> canExecuteDelegate = null)
        {
            executeDelegate.ValidateVariable(nameof(executeDelegate));

            ExecuteDelegate = executeDelegate;
            CanExecuteDelegate = canExecuteDelegate;
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate.HasValue())
            {
                return CanExecuteDelegate();
            }

            return true;
        }

        public async void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                await ExecuteDelegate();
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged.Invoke(this, new EventArgs());
        }
    }

    public class AsyncDelegateCommand<TParameter> : ICommand
    {
        // Properties
        public Predicate<TParameter> CanExecuteDelegate { get; set; }
        public Func<TParameter, Task> ExecuteDelegate { get; set; }

        // Events
        public event EventHandler CanExecuteChanged = delegate { };
        public AsyncDelegateCommand(Func<TParameter, Task> executeDelegate, Predicate<TParameter> canExecuteDelegate = null)
        {
            executeDelegate.ValidateVariable(nameof(executeDelegate));

            ExecuteDelegate = executeDelegate;
            CanExecuteDelegate = canExecuteDelegate;
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate.HasValue())
            {
                return CanExecuteDelegate((TParameter)parameter);
            }

            return true;
        }

        public async void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                await ExecuteDelegate((TParameter)parameter);
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged.Invoke(this, new EventArgs());
        }
    }
}
