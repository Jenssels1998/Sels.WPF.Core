using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.General.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Sels.WPF.Core.Components.Command.DelegateCommand
{
    public class DelegateCommand : ICommand
    {
        // Properties
        public Func<bool> CanExecuteDelegate { get; set; }
        public Action ExecuteDelegate { get; set; }

        // Events
        public event EventHandler CanExecuteChanged = delegate{};
        public DelegateCommand(Action executeDelegate, Func<bool> canExecuteDelegate = null)
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

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                ExecuteDelegate();
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

        // Events
        public event EventHandler CanExecuteChanged = delegate { };
        public DelegateCommand(Action<TParameter> executeDelegate, Predicate<TParameter> canExecuteDelegate = null)
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

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                ExecuteDelegate((TParameter)parameter);
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged.Invoke(this, new EventArgs());
        }
    }
}
