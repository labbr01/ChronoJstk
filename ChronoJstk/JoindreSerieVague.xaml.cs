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
    /// Logique d'interaction pour JoindreSerieVague.xaml
    /// </summary>
    public partial class JoindreSerieVague : Window
    {
        public JoindreSerieVague()
        {
            InitializeComponent();
            this.serie1.IsEnabled = false;
            this.vague1.IsEnabled = false;
            this.Loaded += this.ChoisirCompetition_Loaded;
        }

        private void ChoisirCompetition_Loaded(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
