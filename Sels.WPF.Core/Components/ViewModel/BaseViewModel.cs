using Sels.WPF.Core.Components.Command.DelegateCommand;
using Sels.WPF.Core.Components.Property;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sels.WPF.Core.Components.ViewModel
{
    public abstract class BaseViewModel : BasePropertyChangedNotifier
    {
        // Fields
        private bool _isInitialized = false;

        // Commands
        /// <summary>
        /// Command that gets called when the control loads
        /// </summary>
        public ICommand InitializeControlCommandAsync { get; }

        public BaseViewModel()
        {
            InitializeControlCommandAsync = new AsyncDelegateCommand(() => { var task = InitializeControl(); _isInitialized = true; return task; }, () => !_isInitialized);
        }

        // Events
        /// <summary>
        /// Raised when an exception gets thrown by viewmodel. Can be used to send exceptions to upper viewmodels. First parameter is sender, second is message from viewmodel and third is the exception thrown.
        /// </summary>
        public event Action<object, string, Exception> ExceptionOccured = delegate { };
        protected void RaiseExceptionOccured(Exception exception)
        {
            ExceptionOccured.Invoke(this, string.Empty, exception);   
        }

        protected void RaiseExceptionOccured(string message, Exception exception)
        {
            ExceptionOccured.Invoke(this, message, exception);
        }

        // Abstractions
        /// <summary>
        /// Can be used to perform actions when the control gets loaded
        /// </summary>
        /// <returns></returns>
        protected abstract Task InitializeControl();
    }
}
