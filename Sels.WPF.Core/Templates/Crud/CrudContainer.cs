using Sels.WPF.Core.Components.Command.DelegateCommand;
using Sels.WPF.Core.Components.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Sels.Core.Extensions.General.Validation;
using System.Threading.Tasks;

namespace Sels.WPF.Core.Templates.Crud
{
    public abstract class CrudContainer<TObject, TValidationError, TListViewModel, TDetailViewModel, TCreateOrUpdateViewModel> : BaseViewModel where TListViewModel : CrudList<TObject> where TDetailViewModel : CrudDetail<TObject> where TCreateOrUpdateViewModel : CrudCreateOrEdit<TObject, TValidationError> where TObject : new()
    {
        // Properties
        public BaseViewModel CurrentControl {
            get
            {
                return GetValue<BaseViewModel>(nameof(CurrentControl));
            }
            private set
            {
                SetValue(nameof(CurrentControl), value);
            }
        }
        public TListViewModel ListViewModel {
            get
            {
                return GetValue<TListViewModel>(nameof(ListViewModel));
            }
            private set
            {
                SetValue(nameof(ListViewModel), value);
            }
        }
        public TDetailViewModel DetailViewModel {
            get
            {
                return GetValue<TDetailViewModel>(nameof(DetailViewModel));
            }
            private set
            {
                SetValue(nameof(DetailViewModel), value);
            }
        }
        public TCreateOrUpdateViewModel CreateOrUpdateViewModel {
            get
            {
                return GetValue<TCreateOrUpdateViewModel>(nameof(CreateOrUpdateViewModel));
            }
            private set
            {
                SetValue(nameof(CreateOrUpdateViewModel), value);
            }
        }

        // Commands
        /// <summary>
        /// Command to trigger request that user wants to create an object
        /// </summary>
        public ICommand CreateObjectRequestCommand { get; }

        public CrudContainer(TListViewModel listViewModel, TDetailViewModel detailViewModel, TCreateOrUpdateViewModel createOrUpdateViewModel)
        {
            listViewModel.ValidateVariable(nameof(listViewModel));
            detailViewModel.ValidateVariable(nameof(detailViewModel));
            createOrUpdateViewModel.ValidateVariable(nameof(createOrUpdateViewModel));

            ListViewModel = listViewModel;
            DetailViewModel = detailViewModel;
            CreateOrUpdateViewModel = createOrUpdateViewModel;

            CreateObjectRequestCommand = new AsyncDelegateCommand(() => { return CreateOrUpdateViewModel.SetupForCreate(); });
        }

        /// <summary>
        /// Code that runs when control gets rendered
        /// </summary>
        /// <returns></returns>
        protected override Task InitializeControl()
        {
            var task = Initialize();

            ListViewModel.SelectedObjectChanged += SelectedObjectChangedHandler;
            DetailViewModel.EditObjectRequest += EditObjectRequestHandler;
            CreateOrUpdateViewModel.ObjectPersistedEvent += ObjectPersistedHandler;

            return task;
        }

        // Events Handlers
        private void SelectedObjectChangedHandler(TObject objectChanged)
        {
            objectChanged.ValidateVariable(nameof(objectChanged));

            DetailViewModel.DetailObject = objectChanged;
            CurrentControl = DetailViewModel;
        }

        private async void EditObjectRequestHandler(TObject objectToEdit)
        {
            objectToEdit.ValidateVariable(nameof(objectToEdit));

            await CreateOrUpdateViewModel.SetupForUpdate(objectToEdit);
            CurrentControl = CreateOrUpdateViewModel;
        }

        private void ObjectPersistedHandler(TObject objectPersisted)
        {
            objectPersisted.ValidateVariable(nameof(objectPersisted));

            SelectedObjectChangedHandler(objectPersisted);
        }

        // Abstractions
        /// <summary>
        /// Code that runs when control gets rendered
        /// </summary>
        /// <returns></returns>
        protected abstract Task Initialize();
    }
}
