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
    /// Lógica de interacción para NotasView.xaml
    /// </summary>
    public partial class NotasDialog : Window
    {
        public NotasDialog(NotasViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;
        }
    }
}
