using ClientesMixtos.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace ClientesMixtos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;
        }
    }
}