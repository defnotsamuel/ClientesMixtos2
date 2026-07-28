using ClientesMixtos.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientesMixtos.Views.Dialogs
{
    /// <summary>
    /// Lógica de interacción para PinView.xaml
    /// </summary>
    public partial class PinDialog : Window
    {
        public PinDialog(PinViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
