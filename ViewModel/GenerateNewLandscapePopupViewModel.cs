using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wayfinder.ViewModel
{
    public partial class GenerateNewLandscapePopupViewModel: BaseViewModel
    {
        [ObservableProperty]
        public int? rows;
        [ObservableProperty]
        public int? columns;
        [ObservableProperty]
        public bool submited;

        private Window Popup;
        public GenerateNewLandscapePopupViewModel(Window _popup)
        {
            Title = "Landschaft erstellen";
            Submited = false;

            Popup = _popup;
        }

        [RelayCommand]
        public void OnSubmit()
        {
            Submited = true;
            Popup.Close();
        }

        [RelayCommand]
        public void OnCancel()
        {
            Rows = null;
            Columns = null;
            Submited = false;
            Popup.Close();
        }
    }
}
