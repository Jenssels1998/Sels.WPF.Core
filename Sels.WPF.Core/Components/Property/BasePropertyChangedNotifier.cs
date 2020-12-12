using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.General.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using Sels.Core.Extensions.Reflection.Object;
using Sels.Core.Extensions.Object.ItemContainer;

namespace Sels.WPF.Core.Components.Property
{
    public abstract class BasePropertyChangedNotifier : INotifyPropertyChanged
    {
        // Fields
        private readonly Dictionary<PropertyInfo, object> _propertyValues = new Dictionary<PropertyInfo, object>();


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void RaisePropertyChanged(PropertyInfo propertyThatChanged)
        {
            propertyThatChanged.ValidateVariable(nameof(propertyThatChanged));

            if (PropertyChanged.GetInvocationList().HasValue())
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyThatChanged.Name));
            }
        }
        #endregion


        public T GetValue<T>(PropertyInfo sourceProperty)
        {
            sourceProperty.ValidateVariable(nameof(sourceProperty));

            if (sourceProperty.CanAssign<T>())
            {
                return (T)_propertyValues.TryGetOrSet(sourceProperty, default(T));
            }
            else
            {
                throw new InvalidOperationException($"Could not get value on property <{sourceProperty.Name}> because value type <{typeof(T)}> is not assignable from <{sourceProperty.PropertyType}>");
            }
        }

        public T SetValue<T>(PropertyInfo sourceProperty, T value, Action<PropertyInfo> changedAction = null)
        {
            sourceProperty.ValidateVariable(nameof(sourceProperty));

            if (sourceProperty.CanAssign<T>())
            {
                var propertyValue = sourceProperty.GetValue<T>(this);

                if (propertyValue == null || !propertyValue.Equals(value))
                {
                    _propertyValues.AddValue(sourceProperty, value);
                    RaisePropertyChanged(sourceProperty);
                    changedAction.InvokeOrDefault(sourceProperty);
                }
            }
            else
            {
                throw new InvalidOperationException($"Could not set value on property <{sourceProperty.Name}> because value type <{typeof(T)}> is not assignable from <{sourceProperty.PropertyType}>");
            }

            return value;
        }

        public T GetValue<T>(string propertyName)
        {
            propertyName.ValidateVariable(nameof(propertyName));

            return GetValue<T>(this.GetPropertyInfo(propertyName));
        }

        public T SetValue<T>(string propertyName, T value, Action<PropertyInfo> changedAction = null)
        {
            propertyName.ValidateVariable(nameof(propertyName));

            return SetValue(this.GetPropertyInfo(propertyName), value, changedAction);
        }
    }
}
