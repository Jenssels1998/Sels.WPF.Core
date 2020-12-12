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

        // Abstractions
        /// <summary>
        /// Can be used to perform actions when the control gets loaded
        /// </summary>
        /// <returns></returns>
        protected abstract Task InitializeControl();
    }
}
