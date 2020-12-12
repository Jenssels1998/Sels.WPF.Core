using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.General.Validation;
using Sels.WPF.Core.Components.Command.DelegateCommand;
using Sels.WPF.Core.Components.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sels.WPF.Core.Templates.MainWindow.Navigation
{
    public abstract class NavigatableViewModel<TDefaultPage> : BaseViewModel where TDefaultPage : BaseViewModel
    {
        // Constants
        private const char NavigateOptionSplit = ',';

        // Properties       
        public BaseViewModel CurrentPage {
            get
            {
                return GetValue<BaseViewModel>(nameof(CurrentPage));
            }
            set
            {
                SetValue(nameof(CurrentPage), value);
            }
        }

        // Commands
        public ICommand InitializeCommand { get; }
        public ICommand NavigateCommand { get; }

        public NavigatableViewModel()
        {
            // Setup commands
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        protected override Task InitializeControl()
        {
            Initialize();

            Navigate(ResolveViewModel<TDefaultPage>());

            return Task.CompletedTask;
        }

        #region Navigation
        private void Navigate(string options)
        {
            options.ValidateVariable(nameof(options));

            var splitOptions = options.Split(NavigateOptionSplit);

            Navigate(splitOptions[0], splitOptions.Length > 1 ? splitOptions[1] : null);
        }

        private void Navigate(string viewModelName, string contextName = null)
        {
            viewModelName.ValidateVariable(nameof(viewModelName));

            Navigate(ResolveViewModel(viewModelName), contextName.HasValue() ? ResolveNavigationContext(contextName) : null);
        }

        private void Navigate(BaseViewModel viewToNavigateTo, object context = null)
        {
            viewToNavigateTo.ValidateVariable(nameof(viewToNavigateTo));

            CurrentPage = viewToNavigateTo;
        }
        #endregion

        // Abstractions
        public abstract void Initialize();
        public abstract object ResolveNavigationContext(string contextName);
        public abstract BaseViewModel ResolveViewModel(string viewModelName);

        public abstract BaseViewModel ResolveViewModel<TViewModel>() where TViewModel : BaseViewModel;
    }
}
