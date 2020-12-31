using System;
using System.Collections.Generic;
using System.Text;

namespace Sels.WPF.Core.Templates.MainWindow.Navigation
{
    public interface IHomeNavigator
    {
        event Action HomeNavigationRequest;
    }
}
