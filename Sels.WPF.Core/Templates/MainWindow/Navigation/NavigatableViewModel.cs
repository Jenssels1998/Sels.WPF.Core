using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.General.Validation;
using Sels.WPF.Core.Components.Command.DelegateCommand;
using Sels.WPF.Core.Components.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sels.Core.Extensions.Reflection.Object;

namespace Sels.WPF.Core.Templates.MainWindow.Navigation
{
    public abstract class NavigatableViewModel<TDefaultPage> : BaseViewModel where TDefaultPage : BaseViewModel
    {
        // Constants
        private const char NavigateOptionSplit = ',';

        // Properties       
        public BaseViewModel CurrentControl {
            get
            {
                return GetValue<BaseViewModel>(nameof(CurrentControl));
            }
            set
            {
                SetValue(nameof(CurrentControl), value, () => { SubscribeToExceptionOccuredEvents(CurrentControl); SubscribeToNavigatorEvents(CurrentControl); });
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

        private void SubscribeToExceptionOccuredEvents(BaseViewModel viewModel)
        {
            if (viewModel.HasValue())
            {
                viewModel.ExceptionOccured += ExceptionHandler;

                foreach(var property in viewModel.GetProperties())
                {
                    var propertyValue = property.GetValue(viewModel);

                    if(propertyValue.HasValue() && propertyValue is BaseViewModel subViewModel)
                    {
                        SubscribeToExceptionOccuredEvents(subViewModel);
                    }
                }
            }
        }

        private void SubscribeToNavigatorEvents(BaseViewModel viewModel)
        {
            if (viewModel.HasValue() && viewModel is INavigator navigator)
            {
                navigator.NavigationRequest += NavigationRequestHandler;

                foreach (var property in viewModel.GetProperties())
                {
                    var propertyValue = property.GetValue(viewModel);

                    if (propertyValue.HasValue() && propertyValue is BaseViewModel subViewModel)
                    {
                        SubscribeToNavigatorEvents(subViewModel);
                    }
                }
            }
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
            try
            {
                viewToNavigateTo.ValidateVariable(nameof(viewToNavigateTo));

                if(viewToNavigateTo is INavigatable navigatableView)
                {
                    navigatableView.SetNavigationContext(context);
                }

                CurrentControl = viewToNavigateTo;
            }
            catch(Exception ex)
            {
                RaiseExceptionOccured(ex);
            }         
        }
        #endregion

        // Event Handlers
        private void NavigationRequestHandler(string viewModelName, object context)
        {
            try
            {
                var viewModel = ResolveViewModel(viewModelName);
                Navigate(viewModel, context);
            }
            catch(Exception ex)
            {
                RaiseExceptionOccured(ex);
            }
        }

        // Abstractions
        /// <summary>
        /// Initialize view model
        /// </summary>
        public abstract void Initialize();
        /// <summary>
        /// Resolves context object by name
        /// </summary>
        /// <param name="contextName">Name of context</param>
        /// <returns></returns>
        public abstract object ResolveNavigationContext(string contextName);
        /// <summary>
        /// Used to resolve the view model
        /// </summary>
        /// <param name="viewModelName">View model to resolve</param>
        /// <returns></returns>
        public abstract BaseViewModel ResolveViewModel(string viewModelName);
        /// <summary>
        /// Resolve view model by type
        /// </summary>
        /// <typeparam name="TViewModel">Type of view model to resolve</typeparam>
        /// <returns></returns>
        public abstract BaseViewModel ResolveViewModel<TViewModel>() where TViewModel : BaseViewModel;
        public abstract void ExceptionHandler(object sender, string senderMessage, Exception exceptionToHandle);
    }
}
