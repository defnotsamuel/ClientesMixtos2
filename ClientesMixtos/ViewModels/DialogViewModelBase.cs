using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace ClientesMixtos.ViewModels
{
    public abstract class DialogViewModelBase : ObservableObject
    {
        public event Action<bool?>? CloseRequested;

        protected void RequestClose(bool? result)
        {
            CloseRequested?.Invoke(result);
        }
    }
}
