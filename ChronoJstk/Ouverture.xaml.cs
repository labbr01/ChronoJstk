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
using Microsoft.Win32;
using System.IO;
using PVModele;
using PVModele.Tables;
using System.Configuration;

namespace ChronoJstk
{
    /// <summary>
    /// Logique d'interaction pour Ouverture.xaml
    /// </summary>
    public partial class Ouverture : Window
    {
        public Ouverture()
        {
            InitializeComponent();
            this.Loaded += ChoisirCompetition_Loaded;
        }

        private RegistryKey softwareKey;
        private bool ouvertureValidee = false;

        private void ChoisirCompetition_Loaded(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;

            
            this.softwareKey = Registry.CurrentUser.OpenSubKey("ChronoJstk", true);
            if (this.softwareKey == null) { 
                this.softwareKey = Registry.CurrentUser.CreateSubKey("ChronoJstk");
            }
         

            string nomFichierPat = this.softwareKey.GetValue("NomFichierPAT") as string;
            if (!string.IsNullOrEmpty(nomFichierPat) )
                {
                this.FichierPat.Text = nomFichierPat;
                this.FichierPat_TextChanged(null, null);
            }
            
            object o = this.softwareKey.GetValue("RepLog");
            if (o != null)
            {
                string reptmp = (string)o;
                if (System.IO.Directory.Exists(reptmp))
                {
                    this.RepLog.Text = reptmp;
                    ParamCommuns.Instance.RepLog = reptmp;
                }
            }

            bool mixte = true;
            string otx = ConfigurationManager.AppSettings["Mixte"] as string;
            bool.TryParse(otx, out mixte);
            o = this.softwareKey.GetValue("GroupesMixtes");
            if (o != null)
            {
                bool b = bool.Parse(this.softwareKey.GetValue("GroupesMixtes").ToString());                
                if (b)
                {
                    this.cbxGroupesMixtes.SelectedIndex = 1;
                }
                else {
                    this.cbxGroupesMixtes.SelectedIndex = 0;
                }
            }

            o = this.softwareKey.GetValue("RepCL");
            if (o != null)
            {
                string reptmp = (string)o;
                if (System.IO.Directory.Exists(reptmp))
                {
                    this.RepCL.Text = reptmp;
                    ParamCommuns.Instance.RepCL = reptmp;
                }
            }

            o = this.softwareKey.GetValue("ChronistickInputExe");
            if (o != null)
            {
                string reptmp = (string)o;
                if (System.IO.File.Exists(reptmp))
                {
                    this.RepCL.Text = reptmp;
                    ParamCommuns.Instance.ChronistickInputExe = reptmp;
                }
            }

            o = this.softwareKey.GetValue("UrlWeb");
            if (o != null)
            {
                string reptmp = (string)o;
                this.UrlWeb.Text = reptmp;
            }

            o = this.softwareKey.GetValue("SignalRServer");
            if (o != null)
            {
                string reptmp = (string)o;
                this.SignalRServer.Text = reptmp;
                ParamCommuns.Instance.SignalRServer = reptmp;
            }

            o = this.softwareKey.GetValue("SignalRHub");
            if (o != null)
            {
                string reptmp = (string)o;
                this.SignalRHub.Text = reptmp;
                ParamCommuns.Instance.SignalRHub = reptmp;
            }

            o = this.softwareKey.GetValue("UsagerFtp");
            if (o != null)
            {
                string reptmp = (string)o;
                this.UsagerFtp.Text = reptmp;
            }

            o = this.softwareKey.GetValue("SiteFtp");
            if (o != null)
            {
                string reptmp = (string)o;
                this.SiteFtp.Text = reptmp;
            }

            o = this.softwareKey.GetValue("TravailFtp");
            if (o != null)
            {
                string reptmp = (string)o;
                this.TravailFtp.Text = reptmp;
            } 
            o = this.softwareKey.GetValue("MotPasse");
            if (o != null)
            {
                string reptmp = (string)o;
                this.MotPasse.Password = reptmp;
            }

            // < add key = "PortConfig" value = "2244" />  
            // < add key = "PortInput" value = "2243" />
            // < add key = "HostConfigMaison" value = "192.168.0.193" />
            // < add key = "HostConfig" value = "172.20.70.107" />
            // < add key = "ChronistickInputExe" value = "C:\Users\Bruno\Documents\Visual Studio 2017\Projects\ChronoJstk\ChronoStick-Input\bin\Debug\ChronoStick-Input.exe" />
            // < add key = "SignalRServerx" value = "http://localhost:62751/signalr/hubs" />
            // < add key = "SignalRServer" value = "http://cpvq.mmetara.com/signalr/hubs" />
            // < add key = "SignalRServerProd" value = "http://cpvq.mmetara.com/signalr/hubs" />
            // < add key = "SignalRServerDebug" value = "http://localhost:62751/signalr/hubs" />
            // < add key = "SignalRHub" value = "chatChronoHub" />
            //< add key = "DiffusionWeb" value = "ws://localhost:62751/WSHandler.ashx" />    

            o = this.softwareKey.GetValue("PortConfig");
            if (o != null)
            {
                int reptmp = (int)o;
                this.PortConfig.Text = reptmp.ToString();
            }

            o = this.softwareKey.GetValue("PortInput");
            if (o != null)
            {
                int reptmp = (int)o;
                this.PortInput.Text = reptmp.ToString();
            }

            o = this.softwareKey.GetValue("HostConfig");
            if (o != null)
            {
                string reptmp = (string)o;
                this.HostConfig.Text = reptmp;
            }


            o = this.softwareKey.GetValue("WebChrono");
            if (o != null)
            {
                bool ob = bool.Parse(o.ToString());
                if (ob)
                {
                    this.cbxWebChrono.SelectedIndex = 0;
                }
                else {
                    this.cbxWebChrono.SelectedIndex = 1;
                }
            }

            // this.softwareKey.SetValue("WebResultat", false);
            o = this.softwareKey.GetValue("WebResultat");
            if (o != null)
            {
                bool ob = bool.Parse(o.ToString());
                if (!ob)
                {
                    this.cbxWebResultat.SelectedIndex = 0;
                }
                else
                {
                    this.cbxWebResultat.SelectedIndex = 1;
                }
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        //ChronistickInput
        private void btnChronistickInput_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.DefaultExt = ".exe";
            openFileDialog.Filter = "fichiers EXE (*.exe)|*.exe";
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string nomFichier = openFileDialog.FileName;
                this.ChronistickInput.Text = nomFichier;
                this.softwareKey.SetValue("ChronistickInputExe", this.ChronistickInput.Text);
                ParamCommuns.Instance.ChronistickInputExe = nomFichier;
            }
        }

        //private void ChronistickInput_TextChanged(object sender, TextChangedEventArgs e)
        //{         
        //    this.softwareKey.SetValue("ChronistickInputExe", this.ChronistickInput.Text);
        //    ParamCommuns.Instance.ChronistickInputExe = this.UsagerFtp.Text;
        //}

        private void UsagerFtp_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.softwareKey.SetValue("UsagerFtp", this.UsagerFtp.Text);
            ParamCommuns.Instance.UsagerFTP = this.UsagerFtp.Text;
        }

        private void SiteFtp_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.softwareKey.SetValue("SiteFtp", this.SiteFtp.Text);
            ParamCommuns.Instance.SiteFTP = this.SiteFtp.Text;
        }

        private void TravailFtp_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.softwareKey.SetValue("TravailFTP", this.TravailFtp.Text);
            ParamCommuns.Instance.TravailFTP = this.TravailFtp.Text;
        }


        private void btnFichierPat_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.DefaultExt = ".pat";
            openFileDialog.Filter = "fichiers PAT (*.pat)|*.pat";
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string nomFichier = openFileDialog.FileName;
                this.FichierPat.Text = nomFichier;
                this.FichierPat_TextChanged(sender, null);
            }
        }

        private void FichierPat_TextChanged(object sender, TextChangedEventArgs e)
        {
            string nomFichierPat = this.FichierPat.Text;
            if (System.IO.File.Exists(nomFichierPat))
            {
                this.softwareKey.SetValue("NomFichierPAT", nomFichierPat);
                ParamCommuns.Instance.NomFichierPat = nomFichierPat;
                //"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\Bruno\Documents\Arpvq 2017-2018\TestsChrono\Publication.pat;User Id=admin;Password=;" providerName="System.Data.OleDb"
                string connectString = string.Format(@"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; User Id = admin; Password =; ", nomFichierPat);
                using (DBPatinVitesse db = new DBPatinVitesse(connectString))
                {
                    var  comps = new List<string>();
                     var noComps = new List<int>();
                    foreach (Competition c in db.Competition)
                    {
                        comps.Add(c.Lieu);
                        noComps.Add(c.NoCompetition);
                    }
                    this.cbxCompetition.ItemsSource = comps;
                    ParamCommuns.Instance.Comps = comps;
                    ParamCommuns.Instance.NoComps = noComps;
                }
                if (this.softwareKey.GetValue("NoCompetition") != null)
                {
                    int noCompe = (int)this.softwareKey.GetValue("NoCompetition");
                    ParamCommuns.Instance.NoCompetition = noCompe;
                    this.cbxCompetition.SelectedIndex = ParamCommuns.Instance.NoComps.IndexOf(noCompe);
                }
            }
        }     

        private void cbxCompetition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbxCompetition.SelectedIndex != -1)
            {
                int idx = this.cbxCompetition.SelectedIndex;
                string connectString = string.Format(@"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; User Id = admin; Password =; ", ParamCommuns.Instance.NomFichierPat);
                using (DBPatinVitesse db = new DBPatinVitesse(connectString))
                {
                    int noCompe = ParamCommuns.Instance.NoComps[idx];
                    string nomCompe = db.Competition.SingleOrDefault(z => z.NoCompetition == noCompe).Lieu;
                    this.softwareKey.SetValue("NoCompetition", noCompe);
                    ParamCommuns.Instance.NoCompetition = noCompe;
                    this.softwareKey.SetValue("NomCompetition", nomCompe);
                    ParamCommuns.Instance.NomCompetition = nomCompe;
                    ProgrammeCourseMgr.Instance.DetailCompe(db, noCompe);
                    this.ouvertureValidee = true;
                }
            }
        }

        private void cbxGroupesMixtes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbxGroupesMixtes.SelectedIndex != -1)
            {
                if (cbxGroupesMixtes.SelectedIndex == 0)
                {
                    this.softwareKey.SetValue("GroupesMixtes", false);
                    ParamCommuns.Instance.GroupesMixtes = false;
                }
                else
                {
                    this.softwareKey.SetValue("GroupesMixtes", true);
                    ParamCommuns.Instance.GroupesMixtes = true;
                }
             
            }
        }

        public bool OuvertureValidee {  get { return this.ouvertureValidee; } }

        private void btnRepLog_Click(object sender, RoutedEventArgs e)
        {
            WPFFolderBrowser.WPFFolderBrowserDialog dlg = new WPFFolderBrowser.WPFFolderBrowserDialog();
            string reptmp = this.RepLog.Text;
                if (System.IO.Directory.Exists(reptmp))
                {
                    dlg.InitialDirectory = reptmp;
                }
            
          
            dlg.Title = "Répertoire data du programme";
            bool? rep = dlg.ShowDialog();
            if (rep.HasValue && rep.Value)
            {
                string nom = dlg.FileName;
                this.RepLog.Text = nom;
                this.softwareKey.SetValue("RepLog", nom);
                ParamCommuns.Instance.RepLog = nom;
            }
               
        }


        //btnTravailFtp_Click

        private void btnTravailFtp_Click(object sender, RoutedEventArgs e)
        {
            WPFFolderBrowser.WPFFolderBrowserDialog dlg = new WPFFolderBrowser.WPFFolderBrowserDialog();
            string reptmp = this.TravailFtp.Text;
            if (System.IO.Directory.Exists(reptmp))
            {
                dlg.InitialDirectory = reptmp;
            }


            dlg.Title = "Répertoire de travail FTP";
            bool? rep = dlg.ShowDialog();
            if (rep.HasValue && rep.Value)
            {
                string nom = dlg.FileName;
                this.TravailFtp.Text = nom;
                this.softwareKey.SetValue("TravailFTP", nom);
                ParamCommuns.Instance.TravailFTP = nom;
            }
        }

        private void btnRepCL_Click(object sender, RoutedEventArgs e)
        {
            WPFFolderBrowser.WPFFolderBrowserDialog dlg = new WPFFolderBrowser.WPFFolderBrowserDialog();
            string reptmp = this.RepCL.Text;
            if (System.IO.Directory.Exists(reptmp))
            {
                dlg.InitialDirectory = reptmp;
            }


            dlg.Title = "Répertoire des fichiers CL";
            bool? rep = dlg.ShowDialog();
            if (rep.HasValue && rep.Value)
            {
                string nom = dlg.FileName;
                this.RepCL.Text = nom;
                this.softwareKey.SetValue("RepCL", nom);
                ParamCommuns.Instance.RepCL = nom;
            }

        }

        private void UrlWeb_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.softwareKey.SetValue("UrlWeb", this.UrlWeb.Text);
            ParamCommuns.Instance.UrlWeb = this.UrlWeb.Text;
        }

        //private void UsagerFTP_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    this.softwareKey.SetValue("UsagerFTP", this.UsagerFtp.Text);
        //    ParamCommuns.Instance.UsagerFTP = this.UsagerFtp.Text;
        //}

      
        private void UrlHub_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.softwareKey.SetValue("SignalRServer", this.SignalRServer.Text);
            ParamCommuns.Instance.SignalRServer = this.SignalRServer.Text;
        }

        //SignalRHub_TextChanged
        private void SignalRHub_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.softwareKey.SetValue("SignalRHub", this.SignalRHub.Text);
            ParamCommuns.Instance.SignalRHub = this.SignalRHub.Text;
        }


        private void PortConfig_TextChanged(object sender, TextChangedEventArgs e)
        {
            int port;
            if (!string.IsNullOrEmpty(this.PortConfig.Text) && int.TryParse(this.PortConfig.Text,out port))
            { 
                this.softwareKey.SetValue("PortConfig", port);
                ParamCommuns.Instance.PortConfig = port;
            }
        }

        private void HostConfig_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.softwareKey.SetValue("HostConfig", this.UrlWeb.Text);
            ParamCommuns.Instance.HostConfig = this.UrlWeb.Text;
        }

        private void PortInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            int port;
            if (!string.IsNullOrEmpty(this.PortInput.Text) && int.TryParse(this.PortInput.Text, out port))
            {
                this.softwareKey.SetValue("PortInput", port);
                ParamCommuns.Instance.PortInput = port;
            }                
        }

        private void cbxWebResultat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbxWebResultat.SelectedIndex == 0)
            {
                this.softwareKey.SetValue("WebResultat", false);
                ParamCommuns.Instance.WebResultat = false;
            }
            else
            {
                this.softwareKey.SetValue("WebResultat", true);
                ParamCommuns.Instance.WebResultat = true;
            }
        }

        private void cbxWebChrono_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbxWebResultat.SelectedIndex == 0)
            {
                this.softwareKey.SetValue("WebChrono", false);
                ParamCommuns.Instance.WebChrono = false;
            }
            else
            {
                this.softwareKey.SetValue("WebChrono", true);
                ParamCommuns.Instance.WebChrono = true;
            }
        }

        private void MotPasse_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.softwareKey.SetValue("MotPasse", this.MotPasse.Password);
            ParamCommuns.Instance.MotPasse = this.MotPasse.Password;
        }
    }
}
