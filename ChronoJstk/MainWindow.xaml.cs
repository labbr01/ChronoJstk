using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChronoJstk
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>    
    public partial class MainWindow : Window //, INotifyPropertyChanged
    {
        private MainWindowViewModel mwvm = null;

        /// <summary>
        /// Initialize a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            MainWindowViewModel mwvm = new MainWindowViewModel(this.Dispatcher, this.EnleverTousEnfantSP, this.AjouterEnfantSP);            
            this.DataContext = mwvm;
            ProgrammeCourseMgr.Instance.Mwvm = mwvm;
            this.mwvm = mwvm;
            this.Loaded += MainWindow_Loaded;            
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            Ouverture cc = new Ouverture();
            bool? r = cc.ShowDialog();
            if (r.HasValue && r.Value)
            {
                if (cc.OuvertureValidee)
                {
                    ResultatCompetitionMgr.Instance.EffacerResultats();
                    string nomCompe = ParamCommuns.Instance.NomCompetition;
                    int noCompt = ParamCommuns.Instance.NoCompetition;
                    this.mwvm.OuvrirCompetition(nomCompe, noCompt );
                    if (ParamCommuns.Instance.WebChrono)
                    {
                        this.mwvm.DiffusionWeb();
                    }

                    if (ParamCommuns.Instance.WebResultat)
                    {
                        List<string> res = ResultatCompetitionMgr.Instance.ObtenirResultatCompetition();
                        if (res != null && res.Count > 0)
                        {
                            this.mwvm.AfficherMessageWeb(Chat.ChronoSignalR.TypeMessage.Defilement1, string.Format("Résultats obtenus pour les groupes : {0}", string.Join(",", res)));
                        }
                    }
                    this.mwvm.AppelPatineurs();
                }
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            this.mwvm.MainWindow_Closed(sender, e);
        }
        
        private void EnleverTousEnfantSP()
        {
            while (this.SpEvenements.Children.Count > 1)
            {
                this.SpEvenements.Children.RemoveAt(1);
            }
        }

        private void AjouterEnfantSP(InfoCoursePatineur icp)
        {
            this.SpEvenements.Children.Add(icp);
        }

        private void OuvrirResultat_Click(object sender, RoutedEventArgs e)
        {
            this.mwvm.OuvrirResultat_Click(sender, e);
        }

        private void OuvrirProgramme_Click(object sender, RoutedEventArgs e)
        {
            this.mwvm.OuvrirProgramme_Click(sender, e);
        }

        private void Pc_CourseTerminee(object sender, EventArgs e)
        {
            this.mwvm.Pc_CourseTerminee(sender, e);
        }

        /// <summary>
        /// Un tour est complété pour le patineur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pc_TourComplete(object sender, EventArgs e)
        {
            this.mwvm.Pc_TourComplete(sender, e);
        }

        private void departManuel_Click(object sender, RoutedEventArgs e)
        {
            this.mwvm.departManuel_Click(sender, e);
        }

        private void finCourse_Click(object sender, RoutedEventArgs e)
        {
            this.mwvm.finCourse_Click(sender, e);
        }
   
        private void CompteTour_Click(object sender, RoutedEventArgs e)
        {
            this.mwvm.CompteTour_Click(sender, e);
        }

        private void DiffusionWeb_Click(object sender, RoutedEventArgs e)
        {
            this.mwvm.DiffusionWeb_Click(sender, e);
        }

        private void Interrompre_Click(object sender, RoutedEventArgs e)
        {
            this.mwvm.Interrompre_Click(sender, e);
        }

        private void Sauvegarde_Click(object sender, RoutedEventArgs e)
        {
            this.mwvm.Sauvegarde_Click(sender, e);
        }

        private void Joindre_Click(object sender, RoutedEventArgs e)
        {
            this.mwvm.Joindre_Click(sender, e);            
        }

        private void PublierResultat_Click(object sender, RoutedEventArgs e)
        {
            if (ParamCommuns.Instance.WebResultat)
            {
                List<string> res = ResultatCompetitionMgr.Instance.ObtenirResultatCompetition();
                if (res != null && res.Count > 0)
                {
                    this.mwvm.AfficherMessageWeb(Chat.ChronoSignalR.TypeMessage.Defilement1, string.Format("Résultats obtenus pour les groupes : {0}", string.Join(",", res)));
                }
            }
        }

        private void RelireGroupeVague_Click(object sender, RoutedEventArgs e)
        {
            ProgrammeCourseMgr.Instance.RafraichirCompe();
        }

        //private void Bw_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    BackgroundWorker bw = sender as BackgroundWorker;
        //    if (bw.CancellationPending)
        //    {

        //    }
        //    throw new NotImplementedException();
        //}

        //private void WorkThreadFunction()
        //{
        //    Task t = Chat.WebSocketClient.Init();
        //    t.Wait();
        //    //t = Chat.WebSocketClient.Talk(this.message);
        //    //t.Wait();
        //    //MessageBox.Show("Terminé");
        //    while (true)
        //    {
        //        this.are.WaitOne();
        //        this.are.Reset();
        //        string nouveau = this.message;
        //        t = Chat.WebSocketClient.Talk(this.message);
        //        System.Diagnostics.Debug.WriteLine("Message = " + this.message);
        //    }

        //}

        //private AutoResetEvent are;
        //private string message;
    }
}
