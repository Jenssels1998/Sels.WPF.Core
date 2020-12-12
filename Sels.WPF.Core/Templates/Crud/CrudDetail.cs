using Sels.WPF.Core.Components.Command.DelegateCommand;
using Sels.WPF.Core.Components.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Sels.WPF.Core.Templates.Crud
{
    public abstract class CrudDetail<TObject> : BaseViewModel
    {
        // Properties
        public TObject DetailObject
        {
            get
            {
                return GetValue<TObject>(nameof(DetailObject));
            }
            set
            {
                SetValue(nameof(DetailObject), value, x => DetailObjectChanged(value));
            }
        }

        // Commands
        /// <summary>
        /// Command that triggers request that the user wants to edit the current DetailObject
        /// </summary>
        public ICommand EditObjectCommand { get; }

        public CrudDetail()
        {
            EditObjectCommand = new DelegateCommand(RaiseEditObjectRequest);
        }

        // Events
        /// <summary>
        /// Events that gets raised when the user whats to edit the current DetailObject
        /// </summary>
        public event Action<TObject> EditObjectRequest = delegate { };

        private void RaiseEditObjectRequest()
        {
            EditObjectRequest.Invoke(DetailObject);
        }

        // Abstractions
        /// <summary>
        /// Triggers when the DetailObject changes. Can be used to fetch additional information
        /// </summary>
        /// <param name="detailObjectChanged">Current DetailObject</param>
        protected abstract void DetailObjectChanged(TObject detailObjectChanged);
    }
}
