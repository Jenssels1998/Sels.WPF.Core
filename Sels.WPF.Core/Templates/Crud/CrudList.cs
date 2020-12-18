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
    public abstract class CrudList<TObject> : BaseViewModel
    {
        // Properties
        public bool ReadOnly {
            get
            {
                return GetValue<bool>(nameof(ReadOnly));
            }
            set
            {
                SetValue(nameof(ReadOnly), value, (x, y) => { if (x) RaisePropertyChanged(nameof(CanChange)); });
            }
        }
        public bool CanChange => !ReadOnly;
        public TObject SelectedObject {
            get
            {
                return GetValue<TObject>(nameof(SelectedObject));
            }
            set
            {
                SetValue(nameof(SelectedObject), value, () => RaiseSelectedObjectChanged());               
            }
        }

        public ObservableCollection<TObject> Objects
        {
            get
            {
                return GetValue<ObservableCollection<TObject>>(nameof(Objects));
            }
            set
            {
                SetValue(nameof(Objects), value);
            }
        }

        // Commands
        public ICommand DeleteObjectCommand { get; set; }

        public CrudList()
        {
            ReadOnly = true;
            DeleteObjectCommand = new AsyncDelegateCommand<TObject>(DeleteObjectFromList, exceptionHandler: RaiseExceptionOccured);
        }

        private Task DeleteObjectFromList(TObject objectToDelete)
        {
            try
            {
                if(objectToDelete != null)
                {
                    RaiseObjectDeleted(objectToDelete);
                    Objects.Remove(objectToDelete);
                }              
            }
            catch (Exception ex)
            {
                RaiseExceptionOccured(ex);          
            }
            return Task.CompletedTask;
        }

        // Events
        /// <summary>
        /// Events that gets raised when the SelectedObject changes
        /// </summary>
        public event Action<TObject> SelectedObjectChanged = delegate{};
        private void RaiseSelectedObjectChanged()
        {
            SelectedObjectChanged.Invoke(SelectedObject);
        }

        public event Action<TObject> ObjectDeleted = delegate { };
        private void RaiseObjectDeleted(TObject objectDeleted)
        {
            ObjectDeleted.Invoke(objectDeleted);
        }

        // Abstractions
        /// <summary>
        /// Triggered when DeleteObjectCommand is called
        /// </summary>
        /// <param name="objectToDelete">Object to be deleted</param>
        /// <returns></returns>
    }
}
