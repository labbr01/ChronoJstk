using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ChronoJstk
{
    /// <summary>
    /// Logique d'interaction pour CompteTour.xaml
    /// </summary>
    public partial class CompteTour : Window, INotifyPropertyChanged
    {
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = null;
        private int nombre = 20;
        private ImageBrush background = null;

        public CompteTour()
        {
            InitializeComponent();
            this.DataContext = this;
            //if (this.dispatcherTimer == null)
            //{
            //    dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //    dispatcherTimer.Tick += DispatcherTimer_Tick;
            //    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 3, 0);
            //}
            //dispatcherTimer.Start();
            this.background = this.ZoneText.Background as ImageBrush;
            this.ZoneText.Background = null;
        }   

        public void Initialiser(int nbTour)
        {
            if (this.dispatcherTimer != null)
            {
                this.dispatcherTimer.Stop();
            }

            this.ZoneText.Background = null;
            this.nombre = nbTour;
            this.NotifierChangementPropriete("NbTour");
        }

        public void DecrementerTour()
        {
            if (this.nombre > 0)
            {

                this.nombre -= 1;
                if (this.dispatcherTimer == null)
                {
                    dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                    dispatcherTimer.Tick += DispatcherTimer_Tick;
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 3, 0);
                }
                dispatcherTimer.Start();
            }
        }


        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke((Action)delegate() 
            { 
                //this.nombre -= 1; 
                this.NotifierChangementPropriete("NbTour");
                this.dispatcherTimer.Stop();             
            });
        }

        public void MarquerFinCourse()
        {
            if (this.nombre == 0)
            {
                this.ZoneText.Background = this.background;
                // Mettre la bandrole de fin
            }
        }

        public int NbTour { get { return this.nombre; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifierChangementPropriete(string nom) 
        {
            if (this.PropertyChanged != null) 
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(nom));
            }
        }
    }
}
