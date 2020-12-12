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
        public TObject SelectedObject {
            get
            {
                return GetValue<TObject>(nameof(SelectedObject));
            }
            set
            {
                SetValue(nameof(SelectedObject), value, x => RaiseSelectedObjectChanged());               
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

        public CrudList()
        {
            
        }

        protected override async Task InitializeControl()
        {
            var objects = await LoadObjects();
            Objects = new ObservableCollection<TObject>(objects);
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

        // Abstractions
        /// <summary>
        /// Method that should load the Objects when the page loads
        /// </summary>
        /// <returns></returns>
        protected abstract Task<IEnumerable<TObject>> LoadObjects();
    }
}
