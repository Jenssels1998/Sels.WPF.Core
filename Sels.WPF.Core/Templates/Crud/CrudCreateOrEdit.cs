using Sels.Core.Extensions.General.Generic;
using Sels.WPF.Core.Components.Command.DelegateCommand;
using Sels.WPF.Core.Components.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sels.WPF.Core.Templates.Crud
{
    public abstract class CrudCreateOrEdit<TObject, TValidationError> : BaseViewModel
    {
        // Properties
        public TObject Object {
            get
            {
                return GetValue<TObject>(nameof(Object));
            }
            set
            {
                SetValue(nameof(Object), value);
            }
        }

        public ObservableCollection<TValidationError> ValidationErrors {
            get
            {
                return GetValue<ObservableCollection<TValidationError>>(nameof(ValidationErrors));
            }
            set
            {
                SetValue(nameof(ValidationErrors), value);
            }
        }

        // Commands
        /// <summary>
        /// Command to trigger the saving of the current Object
        /// </summary>
        public ICommand SaveObjectCommand { get; }

        public CrudCreateOrEdit()
        {
            ClearState();

            SaveObjectCommand = new AsyncDelegateCommand(SaveObject, () => Object != null);
        }

        /// <summary>
        /// Sets up viewmodel for creation of object
        /// </summary>
        /// <returns></returns>
        public async Task SetupForCreate()
        {
            ClearState();
            Object = await InitializeForCreate();
        }
        /// <summary>
        /// Sets up viewmodel for updating of object
        /// </summary>
        /// <param name="objectToEdit">Object that needs to be edited</param>
        /// <returns></returns>
        public async Task SetupForUpdate(TObject objectToEdit)
        {
            ClearState();
            await InitializeForEdit(objectToEdit);
            Object = objectToEdit;
        }

        private async Task SaveObject()
        {
            ValidationErrors.Clear();
            var errors = await ValidateObject(Object);

            if (!errors.HasValue())
            {
                await PersistObject(Object);
                RaiseObjectPersisted();
            }
            else
            {
                ValidationErrors = new ObservableCollection<TValidationError>(errors);
            }
        }

        private void ClearState()
        {
            Object = default;
            ValidationErrors = new ObservableCollection<TValidationError>();
        }

        // Event 
        /// <summary>
        /// Events that triggers when an object was persisted
        /// </summary>
        public event Action<TObject> ObjectPersistedEvent = delegate { };

        private void RaiseObjectPersisted()
        {
            ObjectPersistedEvent.Invoke(Object);
        }

        // Abstractions
        /// <summary>
        /// Setup viewmodel for creation of object
        /// </summary>
        /// <returns></returns>
        protected abstract Task<TObject> InitializeForCreate();
        /// <summary>
        /// Setup viewmodel for updating of object
        /// </summary>
        /// <param name="objectToEdit"></param>
        /// <returns></returns>
        protected abstract Task InitializeForEdit(TObject objectToEdit);
        /// <summary>
        /// Validates object before saving
        /// </summary>
        /// <param name="objectToValidate">Object to be validated</param>
        /// <returns></returns>
        protected abstract Task<IEnumerable<TValidationError>> ValidateObject(TObject objectToValidate);
        /// <summary>
        /// Creates or updates current Object
        /// </summary>
        /// <param name="objectToPersist">Object that needs to be persisted</param>
        /// <returns></returns>
        protected abstract Task PersistObject(TObject objectToPersist);
    }
}
