using ChronoStick_Affaires;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChronoStick_Input
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Model.ChronostickInputViewModel viewModel = null;
        public MainWindow()
        {
            InitializeComponent();
            this.viewModel = Model.ChronostickInputViewModel.Instance;
            this.DataContext = this.viewModel;            
            //this.txtEtat.Text = "Non connecté!";
            //this.txtEtat.Background = new SolidColorBrush(Colors.Red);

            Evenement ev = new Evenement();
            ev.NoPatineur = 0;
            ev.Heure = DateTime.Now;
            ev.Action = TypeEvenement.Demarrage;
            ev.Doublon = false;
            ev.Origine = OrigineEvenement.Systeme;
                
            this.viewModel.AjouterJournal(ev);
            this.viewModel.Dispatcher = this.Dispatcher;
            //this.Closed += MainWindow_Closed;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            this.viewModel.Dispatcher = null;
        }

        private void Reconnecter_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.Reconnecter();
        }

        private void Depart_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.Depart( OrigineEvenement.Souris);
        }

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            int noPat;
            int.TryParse(b.Name.Replace("b",string.Empty),out noPat);
            this.viewModel.Tour(noPat, OrigineEvenement.Souris);
        }
    }
}
