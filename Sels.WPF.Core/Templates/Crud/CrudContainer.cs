﻿using Sels.WPF.Core.Components.Command.DelegateCommand;
using Sels.WPF.Core.Components.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Sels.Core.Extensions.General.Validation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Sels.Core.Extensions.General.Generic;

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
            protected set
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
        /// <summary>
        /// Used to refresh the current view model
        /// </summary>
        public ICommand RefreshCommand { get; }

        public CrudContainer(TListViewModel listViewModel, TDetailViewModel detailViewModel, TCreateOrUpdateViewModel createOrUpdateViewModel)
        {
            listViewModel.ValidateVariable(nameof(listViewModel));
            detailViewModel.ValidateVariable(nameof(detailViewModel));
            createOrUpdateViewModel.ValidateVariable(nameof(createOrUpdateViewModel));

            ListViewModel = listViewModel;
            DetailViewModel = detailViewModel;
            CreateOrUpdateViewModel = createOrUpdateViewModel;

            CreateObjectRequestCommand = new AsyncDelegateCommand(CreateObjectRequestHandler, exceptionHandler: RaiseExceptionOccured);
            RefreshCommand = new AsyncDelegateCommand(RefreshPage, exceptionHandler: RaiseExceptionOccured);
        }

        /// <summary>
        /// Code that runs when control gets rendered
        /// </summary>
        /// <returns></returns>
        protected async override Task InitializeControl()
        {
            var initTask = Initialize();

            ListViewModel.SelectedObjectChanged += SelectedObjectChangedHandler;
            DetailViewModel.EditObjectRequest += EditObjectRequestHandler;
            DetailViewModel.DeleteObjectRequest += DeleteObjectRequestHandler;
            CreateOrUpdateViewModel.ObjectPersistedEvent += ObjectPersistedHandler;
            CreateOrUpdateViewModel.CancelEditRequest += CancelEditRequestHandler;          

            await initTask;
            await RefreshPage();
        }

        private async Task RefreshPage()
        {
            var objects = await LoadObjects();
            ListViewModel.Objects = objects.HasValue() ? new ObservableCollection<TObject>(objects) : new ObservableCollection<TObject>();

            CurrentControl = null;
        }

        // Events Handlers
        private Task CreateObjectRequestHandler()
        {
            try
            {
                var task = CreateOrUpdateViewModel.SetupForCreate();
                CurrentControl = CreateOrUpdateViewModel;
                return task;
            }
            catch(Exception ex)
            {
                RaiseExceptionOccured(ex);
            }
            return Task.CompletedTask;
        }
        private void SelectedObjectChangedHandler(TObject objectChanged)
        {
            try
            {
                objectChanged.ValidateVariable(nameof(objectChanged));

                DetailViewModel.DetailObject = objectChanged;
                CurrentControl = DetailViewModel;
            }
            catch (Exception ex)
            {
                RaiseExceptionOccured(ex);
            }
        }
        private async void EditObjectRequestHandler(TObject objectToEdit)
        {
            try
            {
                objectToEdit.ValidateVariable(nameof(objectToEdit));

                await CreateOrUpdateViewModel.SetupForUpdate(objectToEdit);
                CurrentControl = CreateOrUpdateViewModel;
            }
            catch (Exception ex)
            {
                RaiseExceptionOccured(ex);
            }
        }
        private void ObjectPersistedHandler(TObject objectPersisted)
        {
            try
            {
                objectPersisted.ValidateVariable(nameof(objectPersisted));

                if (!ListViewModel.Objects.Contains(objectPersisted))
                {
                    ListViewModel.Objects.Add(objectPersisted);
                }

                SelectedObjectChangedHandler(objectPersisted);
            }
            catch (Exception ex)
            {
                RaiseExceptionOccured(ex);
            }
        }
        private async void CancelEditRequestHandler(TObject objectEditCanceled)
        {
            try
            {
                CurrentControl = null;
                await CancelEdit(objectEditCanceled);                
            }
            catch (Exception ex)
            {
                RaiseExceptionOccured(ex);
            }          
       }
        private async void DeleteObjectRequestHandler(TObject objectEditCanceled)
        {
            try
            {              
                var deleted = await DeleteObject(objectEditCanceled);

                if (deleted)
                {
                    CurrentControl = null;
                }
            }
            catch (Exception ex)
            {
                RaiseExceptionOccured(ex);
            }
        }


        // Abstractions
        /// <summary>
        /// Code that runs when control gets rendered
        /// </summary>
        /// <returns></returns>
        protected abstract Task Initialize();
        /// <summary>
        /// Method that should load the Objects when the page loads
        /// </summary>
        /// <returns></returns>
        protected abstract Task<IEnumerable<TObject>> LoadObjects();
        /// <summary>
        /// Used to cancel the editing of the supplied object.
        /// </summary>
        /// <param name="objectEditCanceled">Object that was cancelled editing</param>
        /// <returns></returns>
        protected abstract Task CancelEdit(TObject objectEditCanceled);
        /// <summary>
        /// Used to delete the supplied object
        /// </summary>
        /// <param name="objectToDelete">Object to delete</param>
        /// <returns>Was object deleted</returns>
        protected abstract Task<bool> DeleteObject(TObject objectToDelete);
    }
}
