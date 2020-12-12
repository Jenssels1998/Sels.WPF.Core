using Sels.WPF.Core.Components.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace Sels.WPF.Core.Components.Behaviours
{
    public class BindLoadedCommandBehaviour
    {
        public static int GetBindCommand(DependencyObject obj)
        {
            return (int)obj.GetValue(BindCommandProperty);
        }

        public static void SetBindCommand(DependencyObject obj, string value)
        {
            obj.SetValue(BindCommandProperty, int.Parse(value));
        }

        // Using a DependencyProperty as the backing store for LoadedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindCommandProperty =
            DependencyProperty.RegisterAttached("BindCommand", typeof(int), typeof(LoadedCommandBehaviour), new PropertyMetadata(0, BindCommandChanged));

        private static void BindCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;


            if (d is FrameworkElement frameworkElement)
            {
                frameworkElement.Loaded
                  += (o, args) =>
                  {
                      if(frameworkElement.DataContext is BaseViewModel viewModel)
                      {
                          viewModel.InitializeControlCommandAsync.Execute(null);
                      }                     
                  };
            }
        }
    }
}
