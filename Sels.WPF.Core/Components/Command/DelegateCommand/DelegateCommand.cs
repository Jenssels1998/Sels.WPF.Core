using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.General.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Sels.Core.Extensions.Execution.Linq;

namespace Sels.WPF.Core.Components.Command.DelegateCommand
{
    public class DelegateCommand : ICommand
    {
        // Properties
        public Func<bool> CanExecuteDelegate { get; set; }
        public Action ExecuteDelegate { get; set; }
        public Action<Exception> ExceptionHandler { get; set; }

        // Events
        public event EventHandler CanExecuteChanged = delegate{};
        public DelegateCommand(Action executeDelegate, Func<bool> canExecuteDelegate = null, Action<Exception> exceptionHandler = null)
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
            catch (Exception ex)
            {
                ExceptionHandler.ForceExecuteOrDefault(ex);
            }

            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                if (CanExecute(parameter))
                {
                    ExecuteDelegate();
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

    public class DelegateCommand<TParameter> : ICommand
    {
        // Properties
        public Predicate<TParameter> CanExecuteDelegate { get; set; }
        public Action<TParameter> ExecuteDelegate { get; set; }
        public Action<Exception> ExceptionHandler { get; set; }

        // Events
        public event EventHandler CanExecuteChanged = delegate { };
        public DelegateCommand(Action<TParameter> executeDelegate, Predicate<TParameter> canExecuteDelegate = null, Action<Exception> exceptionHandler = null)
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
                    if (parameter is TParameter typedParameter)
                    {
                        return CanExecuteDelegate(typedParameter);
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.ForceExecuteOrDefault(ex);
            }

            return true;
        }

        public void Execute(object parameter)
        {
            try
            {               
                if (CanExecute(parameter))
                {
                    ExecuteDelegate((TParameter)parameter);
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
