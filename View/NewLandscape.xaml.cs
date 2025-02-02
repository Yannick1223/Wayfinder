using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wayfinder.ViewModel;

namespace Wayfinder.View
{
    /// <summary>
    /// Interaktionslogik für NewLandscape.xaml
    /// </summary>
    public partial class NewLandscape : Window
    {
        public GenerateNewLandscapePopupViewModel GeneratorVM {  get; private set; }
        public NewLandscape()
        {
            GeneratorVM = new GenerateNewLandscapePopupViewModel(this);
            DataContext = GeneratorVM;
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
        }
    }
}
