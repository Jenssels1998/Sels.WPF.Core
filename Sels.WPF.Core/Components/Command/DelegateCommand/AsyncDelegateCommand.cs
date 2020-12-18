using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.General.Validation;
using Sels.Core.Extensions.Execution.Linq;

namespace Sels.WPF.Core.Components.Command.DelegateCommand
{
    public class AsyncDelegateCommand : ICommand
    {
        // Properties
        public Func<bool> CanExecuteDelegate { get; set; }
        public Func<Task> ExecuteDelegate { get; set; }
        public Action<Exception> ExceptionHandler { get; set; }

        // Events
        public event EventHandler CanExecuteChanged = delegate { };
        public AsyncDelegateCommand(Func<Task> executeDelegate, Func<bool> canExecuteDelegate = null, Action<Exception> exceptionHandler = null)
        {
            executeDelegate.ValidateVariable(nameof(executeDelegate));

            ExecuteDelegate = executeDelegate;
            CanExecuteDelegate = canExecuteDelegate;
            ExceptionHandler = exceptionHandler;
        }

        public bool CanExecute(object parameter)
        {
            try
            {
                if (CanExecuteDelegate.HasValue())
                {
                    return CanExecuteDelegate();
                }
            }
            catch (Exception ex) {
                ExceptionHandler.ForceExecuteOrDefault(ex);
            }
            
            return true;
        }

        public async void Execute(object parameter)
        {
            try
            {
                if (CanExecute(parameter))
                {
                    await ExecuteDelegate();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.ForceExecuteOrDefault(ex);
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
        public Action<Exception> ExceptionHandler { get; set; }

        // Events
        public event EventHandler CanExecuteChanged = delegate { };
        public AsyncDelegateCommand(Func<TParameter, Task> executeDelegate, Predicate<TParameter> canExecuteDelegate = null, Action<Exception> exceptionHandler = null)
        {
            executeDelegate.ValidateVariable(nameof(executeDelegate));

            ExecuteDelegate = executeDelegate;
            CanExecuteDelegate = canExecuteDelegate;
            ExceptionHandler = exceptionHandler;
        }

        public bool CanExecute(object parameter)
        {
            try
            {
                if (CanExecuteDelegate.HasValue())
                {
                    return CanExecuteDelegate((TParameter)parameter);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.ForceExecuteOrDefault(ex);
            }

            return true;
        }

        public async void Execute(object parameter)
        {
            try
            {
                if (CanExecute(parameter))
                {
                    await ExecuteDelegate((TParameter)parameter);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.ForceExecuteOrDefault(ex);
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged.Invoke(this, new EventArgs());
        }
    }
}
