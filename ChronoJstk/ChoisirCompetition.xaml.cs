using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ChronoJstk
{
    /// <summary>
    /// Logique d'interaction pour ChoisirCompetition.xaml
    /// </summary>
    public partial class ChoisirCompetition : Window
    {
        public ChoisirCompetition()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += ChoisirCompetition_Loaded;
        }

        private void ChoisirCompetition_Loaded(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
        }

        public ObservableCollection<string> ListCompe { get; set; }

        public string CompeSele { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
