using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

//2017-09-26 - Ordre du jour AG Spécial.docx

namespace ChronoJstk
{
    [DataContract]
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        bool diffusionWeb = false;
        public event PropertyChangedEventHandler PropertyChanged;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = null;
        //private Dictionary<string, List<PatineurVague>> _lpv = null;
        private Dictionary<Chat.ChronoSignalR.TypeMessage, string> logMessages = new Dictionary<Chat.ChronoSignalR.TypeMessage, string>();
        Chat.ChronoSignalR ccw = null;
        private ChronistickInput.ChronistickInputMgr mgrInput;
        private CompteTour ct = null;
        Dispatcher Dispatcher = null;
        private Action EnleverEnfant;
        private Action<InfoCoursePatineur> AjouterEnfant;

        public MainWindowViewModel(Dispatcher d, Action enleverEnfant, Action<InfoCoursePatineur> ajouterEnfant)
        {
            this.EnleverEnfant = enleverEnfant;
            this.AjouterEnfant = ajouterEnfant;
            this.Dispatcher = d;
            this.blocs = new ObservableCollection<int>();
            this.series = new ObservableCollection<int>();
            this.vagues = new ObservableCollection<string>();

            mgrInput = new ChronistickInput.ChronistickInputMgr();
            mgrInput.DepartEventHandler += MgrInput_DepartEventHandler;
            mgrInput.TourEventHandler += MgrInput_TourEventHandler;
            this.SauvegardeVisibility = Visibility.Hidden;
            this.InterrompreVisibility = Visibility.Hidden;

            this.TitreFenetre = "Aucune compétition";
        }

        private string _TitreFenetre;
        [DataMember]
        public string TitreFenetre { get { return this._TitreFenetre; } set { this._TitreFenetre = value; this.NotifierChangementPropriete("TitreFenetre"); } }

        private Visibility _SauvegardeVisibility = Visibility.Hidden;
        [DataMember]
        public Visibility SauvegardeVisibility { get { return this._SauvegardeVisibility; } set { this._SauvegardeVisibility = value; this.NotifierChangementPropriete("SauvegardeVisibility"); } }
        private Visibility _InterrompreVisibility = Visibility.Hidden;
        [DataMember]
        public Visibility InterrompreVisibility { get { return this._InterrompreVisibility; } set { this._InterrompreVisibility = value; this.NotifierChangementPropriete("InterrompreVisibility"); } }

        //JoindreVisibility
        private Visibility _JoindreVisibility = Visibility.Hidden;
        [DataMember]
        public Visibility JoindreVisibility { get { return this._JoindreVisibility; } set { this._JoindreVisibility = value; this.NotifierChangementPropriete("JoindreVisibility"); } }

        private void MgrInput_TourEventHandler(object sender, ChronistickInput.ChronstickInputEventArgs e)
        {
            Dispatcher.Invoke((Action)delegate ()
            {
                if (this.patcs.Count() >= e.Evenement.NoPatineur)
                {
                    this.patcs[e.Evenement.NoPatineur - 1].Tour(e.Evenement.Heure);
                }
            });
        }
        [DataMember]
        public Dictionary<string, List<PatineurVague>> Lpv { get { return ParamCommuns.Instance.DescVagues; } set { ParamCommuns.Instance.DescVagues = value; } }
        private double nbTourDbl = double.MinValue;
        [DataMember]
        public double NbTourDbl { get { return this.nbTourDbl; } set { this.nbTourDbl = value; } }
        private int nbTourInt = int.MinValue;
        [DataMember]
        public int NbTourInt { get { return this.nbTourInt; } set { this.nbTourInt = value; } }
        private SolidColorBrush _lblEtatBackground;
        //[DataMember]
        public SolidColorBrush lblEtatBackground { get { return this._lblEtatBackground; } set { this._lblEtatBackground = value; this.NotifierChangementPropriete("lblEtatBackground"); } }
        private string _lblEtatTxt;
        [DataMember]
        public string lblEtatTxt
        {
            get
            {
                return this._lblEtatTxt;
            }
            set
            {
                this._lblEtatTxt = value;
                this.NotifierChangementPropriete("lblEtatTxt");
                this.afficherMessageWeb(Chat.ChronoSignalR.TypeMessage.EtatCourse, this._lblEtatTxt);
            }
        }
        private string _lblDepartTxt;
        [DataMember]
        public string lblDepartTxt
        {
            get
            {
                return this._lblDepartTxt;
            }
            set
            {
                this._lblDepartTxt = value;
                this.NotifierChangementPropriete("lblDepartTxt");
                this.afficherMessageWeb(Chat.ChronoSignalR.TypeMessage.DepartCourse, this._lblDepartTxt);
                // Va partir du côté javascript 
                this.afficherMessageWeb(Chat.ChronoSignalR.TypeMessage.ChronoCourse, this._lblDepartTxt);
            }
        }
        private string _lblChronoTxt;
        [DataMember]
        public string lblChronoTxt
        {
            get
            {
                return this._lblChronoTxt;
            }
            set
            {
                this._lblChronoTxt = value;
                this.NotifierChangementPropriete("lblChronoTxt");
                // Ne pas faire cette action, le chrono est simulé en javascript sur le client
                //this.afficherMessageWeb(Chat.ChronoSignalR.TypeMessage.ChronoCourse, this._lblChronoTxt);
            }
        }
        private string _lblFinTxt;
        [DataMember]
        public string lblFinTxt
        {
            get
            {
                return this._lblFinTxt;
            }
            set
            {
                this._lblFinTxt = value;
                this.NotifierChangementPropriete("lblFinTxt");
                this.afficherMessageWeb(Chat.ChronoSignalR.TypeMessage.FinCourse, this._lblFinTxt);
            }
        }
        private DateTime depart;
        [DataMember]
        public DateTime Depart { get { return this.depart; } set { this.Depart = value; } }
        private string nomFichierPat = string.Empty;
        [DataMember]
        public string NomFichierPat { get { return this.nomFichierPat; } set { this.nomFichierPat = value; } }
        private string cheminFichierPath = string.Empty;
        [DataMember]
        public string CheminFichierPath { get { return this.cheminFichierPath; } set { this.cheminFichierPath = value; } }
        private int noCompe = 1;
        [DataMember]
        public int NoCompe { get { return this.noCompe; } set { this.noCompe = value; } }


        // private string cheminAcces;
        private ObservableCollection<PatineurCourse> patcs;
        [DataMember]
        public ObservableCollection<PatineurCourse> Patcs { get { return this.patcs; } set { this.patcs = value; } }

        private ObservableCollection<ProgrammeCourse> pcg = null;
        [DataMember]
        public ObservableCollection<ProgrammeCourse> Pcg { get { return ParamCommuns.Instance.Programmes; } set { ParamCommuns.Instance.Programmes = value; } }

        [DataMember]
        public ObservableCollection<int> blocs { get; private set; }
        [DataMember]
        public ObservableCollection<int> series { get; private set; }
        [DataMember]
        public ObservableCollection<string> vagues { get; private set; }

        private int _blocSel;
        [DataMember]
        public int blocSel
        {
            get { return this._blocSel; }
            set
            {
                this._blocSel = value;
                if (this.Pcg != null)
                {
                    this.series.Clear();
                    this.Pcg.Where(z => z.Bloc == this._blocSel).Select(z => z.Serie).Distinct().ToList().ForEach(z => this.series.Add(z));
                    this.serieSel = this.series.First();
                    this.NotifierChangementPropriete("blocSel");
                }
                this.afficherMessageWeb(Chat.ChronoSignalR.TypeMessage.Bloc, this._blocSel.ToString());
            }
        }
        private int _serieSel;
        [DataMember]
        public int serieSel
        {
            get { return this._serieSel; }
            set
            {
                this._serieSel = value;
                if (this.Pcg != null)
                {
                    this.vagues.Clear();                    
                    ProgrammeCourse pcc = this.Pcg.SingleOrDefault(z => z.Bloc == this._blocSel && z.Serie == this._serieSel);
                    string possibles = "ABCDEFGHIJKLMNOP";
                    char dep = System.Convert.ToChar(pcc.De[0]);
                    char fin = System.Convert.ToChar(pcc.A[0]);
                    this.vagues.Clear();
                    pcc.LVagues.ForEach(z => this.vagues.Add(z));
                    //foreach (char de in possibles)
                    //{
                    //    this.vagues.Add(de.ToString());
                    //    if (char.Equals(de, fin))
                    //    {
                    //        break;
                    //    }

                    //}
                    this.vagueSel = this.vagues.First();
                    this.NotifierChangementPropriete("serieSel");
                }
                this.afficherMessageWeb(Chat.ChronoSignalR.TypeMessage.Serie, this._serieSel.ToString());
            }
        }
        private string _vagueSel;
        [DataMember]
        public string vagueSel
        {
            get { return this._vagueSel; }
            set
            {

                this._vagueSel = value;
                this.NotifierChangementPropriete("vagueSel");

                KeyValuePair<int,string> kvp = new KeyValuePair<int, string>(this.serieSel,this.vagueSel);
                List<KeyValuePair<int, string>> lkvp = new List<KeyValuePair<int, string>>();
                lkvp.Add(kvp);

                this.PreparerVagues(lkvp);
          
                this.JoindreVisibility = Visibility.Visible;
            }
        }

        private void PreparerVagues(List<KeyValuePair<int, string>> lkvp)
        {
            bool vagueDouble = lkvp.Count() > 1;
            List<string> vagues = new List<string>();
            List<int> traces = new List<int>();
            List<double> nbtours = new List<double>();
            List<string> typeCourses = new List<string>();
            foreach (KeyValuePair<int,string> kvp in lkvp) { 
                ProgrammeCourse pcc = this.Pcg.SingleOrDefault(z => z.Bloc == this._blocSel && z.Serie == this._serieSel);
                traces.Add(pcc.Trace);
                nbtours.Add(pcc.NbTour);
                typeCourses.Add(pcc.TypeCourse);                
            }
            if (traces.Distinct().Count() == 1 && nbtours.Distinct().Count() == 1 && typeCourses.Distinct().Count() == 1)
            {
                this.traceTxt = traces.First();
                this.nbTourTxt = nbtours.First();
                this.typeTxt = typeCourses.First();
            }
            else {
                if (traces.Distinct().Count() > 1 )
                {
                    MessageBox.Show(string.Format("Le regroupement doit se faire sur un tracé identique : tracés :[{0}]", string.Join(",", traces)), "Regroupement de course", MessageBoxButton.OK);
                    return;
                }
                if (nbtours.Distinct().Count() > 1)
                {
                    MessageBox.Show(string.Format("Le regroupement doit se faire sur un nombre de tour identique : nbTour :[{0}]", string.Join(",", nbtours)), "Regroupement de course", MessageBoxButton.OK);
                    return;
                }
                
                // Tracé ou nombre de tour ou type de course différent, on ne peut regrouper.
                if ( MessageBox.Show(string.Format("Êtes vous certain de regrouper des course de type différents : {{0}]", string.Join(",", typeTxt)), "Regroupement de course", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
                this.traceTxt = traces.First();
                
                this.typeTxt = string.Join(",", traces);
            }

            double nbTour = nbtours.First();
            this.nbTourTxt = nbTour;

            // Initialiser le nombre de tour de la course
            if (this.ct != null)
            {
                this.ct.Initialiser((int)nbtours.First());
            }
            //Initialiser

            // Terminer les évènements sur les anciens patineurs
            if (this.patcs != null)
            {
                foreach (PatineurCourse pcx in this.patcs)
                {
                    pcx.TourComplete -= Pc_TourComplete;
                    pcx.CourseTerminee -= this.Pc_CourseTerminee;
                }
                this.patcs.Clear();
            }
            else
            {
                this.patcs = new ObservableCollection<PatineurCourse>();
            }

            // Effacer l'affichage actuel
            if (this.EnleverEnfant != null)
            {
                this.EnleverEnfant();
            }

            //if (this.patcs != null)
            //{
            //    foreach (PatineurCourse pc in this.patcs)
            //    {
            //        pc.CourseTerminee -= this.Pc_CourseTerminee;
            //        pc.TourComplete -= Pc_TourComplete;
            //    }
            //}
            //else
            //{
            //    this.patcs = new ObservableCollection<PatineurCourse>();
            //}

           // this.patcs.Clear();


            ObservableCollection<PatineurVague> patvs = new ObservableCollection<PatineurVague>();
            foreach (KeyValuePair<int, string> kvp in lkvp)
            {
                int serieTravail = kvp.Key;
                string vagueTravail = kvp.Value;
                if (string.IsNullOrEmpty(vagueTravail))
                {
                    return;
                }
                if (!vagueTravail.Contains(kvp.Value))
                {
                    vagues.Add(vagueTravail);
                }
                ObservableCollection<PatineurVague> patvsLocal = new ObservableCollection<PatineurVague>();
                List<PatineurVague> lpv = ParamCommuns.Instance.DescVagues.SingleOrDefault(z => z.Key == serieTravail.ToString().PadLeft(4, '0')).Value;
                //List<PatineurVague> lpv = this._lpv.SingleOrDefault(z => z.Key == serieTravail.ToString().PadLeft(4, '0')).Value;
                if (lpv != null)
                {
                    lpv.Where(k => k.Vague == vagueTravail).ToList().ForEach(z => patvsLocal.Add(z));
                }

                patvsLocal.OrderBy(c=> c.Casque).ToList().ForEach(z =>
                {

                    PatineurCourse pc = new PatineurCourse()
                    {                 
                        Casque = z.Casque.ToString(),
                        Bloc = this.blocSel,
                        Serie = serieTravail,
                        Vague = vagueTravail,
                        Patineur = z,
                        NbTourCourse = System.Convert.ToInt32(Math.Truncate(nbTour)),
                        Evenements = new ObservableCollection<EvenementPatineur>(),
                        Temps = "0:00.00",
                        AssignerChronoWeb = this.ccw
                    };

                    if (vagueDouble)
                    {
                        pc.Casque = string.Format("{0}{1}:{2}", serieTravail, vagueTravail, pc.Casque);
                    }

                    pc.TourComplete += Pc_TourComplete;
                    pc.CourseTerminee += Pc_CourseTerminee;
                    patcs.Add(pc);
                    InfoCoursePatineur icp = new InfoCoursePatineur();
                    icp.DataContext = pc;
                    if (this.AjouterEnfant != null)
                    {
                        this.AjouterEnfant(icp);
                    }
                    //this.SpEvenements.Children.Add(icp);
                });
                patvsLocal.ToList().OrderBy(t => t.Casque).ToList().ForEach(zz => patvs.Add(zz));
            }

            if (!string.IsNullOrEmpty(this._vagueSel))
            {
                this.afficherMessageWeb(Chat.ChronoSignalR.TypeMessage.Vague, string.Format("{0}",",",vagues.Distinct()));
            }


            mgrInput.ConfigurerNbPatineur(patvs.Count);

            this.lblEtatTxt = "PRET";
            this.lblEtatBackground = new SolidColorBrush(Colors.Yellow);
            this.lblDepartTxt = "";
            this.lblChronoTxt = "";
            this.lblFinTxt = "";

            if (this.ct != null)
            {
                this.nbTourDbl = nbtours.First();
                this.nbTourInt = System.Convert.ToInt32(Math.Truncate(this.nbTourDbl));
                this.ct.Initialiser(nbTourInt);
            }
          
        }

        private int _traceTxt;
        [DataMember]
        public int traceTxt
        {
            get { return this._traceTxt; }
            set
            {
                this._traceTxt = value; this.NotifierChangementPropriete("traceTxt");
                this.afficherMessageWeb(Chat.ChronoSignalR.TypeMessage.Trace, this._traceTxt.ToString());
            }
        }
        private double _nbTourTxt;
        [DataMember]
        public double nbTourTxt
        {
            get { return this._nbTourTxt; }
            set
            {
                this._nbTourTxt = value; this.NotifierChangementPropriete("nbTourTxt");
                this.afficherMessageWeb(Chat.ChronoSignalR.TypeMessage.NbTour, this._nbTourTxt.ToString());
            }
        }
        private bool _blocIsEnabled;
        [DataMember]
        public bool blocIsEnabled { get { return this._blocIsEnabled; } set { this._blocIsEnabled = value; this.NotifierChangementPropriete("blocIsEnabled"); } }
        private bool _serieIsEnabled;
        [DataMember]
        public bool serieIsEnabled { get { return this._serieIsEnabled; } set { this._serieIsEnabled = value; this.NotifierChangementPropriete("serieIsEnabled"); } }
        private bool _vagueIsEnabled;
        [DataMember]
        public bool vagueIsEnabled { get { return this._vagueIsEnabled; } set { this._vagueIsEnabled = value; this.NotifierChangementPropriete("vagueIsEnabled"); } }
        private string _typeTxt;
        [DataMember]
        public string typeTxt
        {
            get { return this._typeTxt; }
            set
            {
                this._typeTxt = value; this.NotifierChangementPropriete("typeTxt");
                this.afficherMessageWeb(Chat.ChronoSignalR.TypeMessage.TypeCourse, this._typeTxt);
            }
        }

        public void Pc_CourseTerminee(object sender, EventArgs e)
        {
            if (this.ct != null)
            {
                this.ct.MarquerFinCourse();
            }
            bool superfin = true;
            foreach (PatineurCourse pc in this.patcs)
            {
                if (!pc.Termine)
                {
                    superfin = false;
                    break;
                }
            }
            if (superfin)
            {
                // La course est vraiement complétée, on informe le web
                this.finCourse_Click(null, null);
            }
        }

        private void afficherMessageWeb(Chat.ChronoSignalR.TypeMessage type, string message)
        {
            if (this.logMessages.ContainsKey(type))
            {
                this.logMessages[type] = message;
            }
            else
            {
                this.logMessages.Add(type, message);
            }
            if (this.ccw != null)
            {
                this.ccw.AfficherMessage(type, message);
            }
        }

        /// <summary>
        /// Un tour est complété pour le patineur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Pc_TourComplete(object sender, EventArgs e)
        {
            PatineurCourse pc = sender as PatineurCourse;
            int nombre = pc.NbTourAffiche;
            int min = int.MaxValue;
            int max = int.MinValue;
            foreach (PatineurCourse pcc in this.patcs)
            {
                if (pcc.NbTourAffiche < min)
                {
                    min = pcc.NbTourAffiche;
                }
                if (pcc.NbTourAffiche > max)
                {
                    max = pcc.NbTourAffiche;
                }
            }

            if (min == -1)
            {
                return;
            }

            if (this.ct != null && min < this.ct.NbTour)
            {
                while (this.ct.NbTour > min)
                {
                    this.ct.DecrementerTour();
                }

            }
            System.Diagnostics.Debug.WriteLine(string.Format("{0} - {1} - {2}", nbTourTxt, min, max));

        }

        public void departManuel_Click(object sender, RoutedEventArgs e)
        {
            if (this.patcs == null || this.patcs.Count() == 0)
            {
                MessageBox.Show("Charger le programme de course avant le signal de départ");
                return;
            }

            ProgrammeCourseMgr.InterrompreMajCompe();
            this.JoindreVisibility = Visibility.Visible;
            this.InterrompreVisibility = Visibility.Visible;
            DateTime dt = DateTime.Now;
            this.depart = dt;           
            ParamCommuns.Instance.Etat = ParamCommuns.EtatEnum.EnCourse;
            this.lblEtatTxt = "En Course";
            this.lblEtatBackground = new SolidColorBrush(Colors.Green);
            this.lblDepartTxt = string.Format("{0}h:{1}m:{2}.{3}s", dt.Hour, dt.Minute, dt.Second, System.Convert.ToInt32(dt.Millisecond / 100));

            if (this.ct != null)
            {
                ProgrammeCourse pcc = this.Pcg.SingleOrDefault(z => z.Bloc == this._blocSel && z.Serie == this._serieSel);
                this.ct.Initialiser((int)pcc.NbTour);
            }

            this.lblChronoTxt = "";
            this.lblFinTxt = "";

            foreach (PatineurCourse pc in this.patcs)
            {
                pc.Depart(dt);
            }

            if (this.dispatcherTimer == null)
            {
                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            }
            dispatcherTimer.Start();

            if (this.ct != null)
            {
                double reste = this.nbTourDbl - this.nbTourInt;
                if (reste <= 0.1)
                {
                    // Nombre de tour d'une course complète décrémente dès le départ
                    this.ct.DecrementerTour();
                }
            }

            this.blocIsEnabled = false;
            this.serieIsEnabled = false;
            this.vagueIsEnabled = false;


            //            Process() // method to be called after regular interval in Timer
            //{
            //                // lengthy process, i.e. data fetching and processing etc.

            //                // here comes the UI update part
            //                Dispatcher.Invoke((Action)delegate () { /* update UI */ });
            //            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan dt = DateTime.Now - this.depart;
            Dispatcher.Invoke((Action)delegate () { this.lblChronoTxt = string.Format("{0}m:{1}.{2}s", dt.Minutes, dt.Seconds, System.Convert.ToInt32(dt.Milliseconds / 100)); ; });
        }

        public void finCourse_Click(object sender, RoutedEventArgs e)
        {
            if (this.patcs == null || this.patcs.Count() == 0)
            {
                MessageBox.Show("Charger le programme de course avant le signal de fin de course");
                return;
            }
            this.SauvegardeVisibility = Visibility.Visible;
            this.InterrompreVisibility = Visibility.Hidden;

            if (this.dispatcherTimer != null)
            {
                this.dispatcherTimer.Stop();
            }
            DateTime dt = DateTime.Now;
            this.depart = dt;
            this.lblEtatTxt = "Terminé";
            this.lblEtatBackground = new SolidColorBrush(Colors.Red);
            this.lblFinTxt = string.Format("{0}h:{1}m:{2}.{3}s", dt.Hour, dt.Minute, dt.Second, System.Convert.ToInt32(dt.Millisecond / 100));
            this.lblChronoTxt = "";

            foreach (PatineurCourse pc in this.patcs)
            {
                if (!pc.Termine)
                {
                    pc.Fin(dt);
                }
            }

  
                this.SauvegarderClCourse();
           
        }

        private void SauvegarderClCourse()
        {
            // Faire le code pour sauvegarder;            
            this.blocIsEnabled = true;
            this.serieIsEnabled = true;
            this.vagueIsEnabled = true;
            this.SauvegardeVisibility = Visibility.Hidden;

            
            string dirLog = System.IO.Path.Combine(ParamCommuns.Instance.RepLog, ParamCommuns.Instance.NomCompetition.Replace("/","_").Replace("\\", "_").Replace(" ","_"));
            if (!System.IO.Directory.Exists(dirLog)) 
            {
                System.IO.Directory.CreateDirectory(dirLog);
            }
                
            string nomFichBkp = System.IO.Path.Combine(dirLog, this.serieSel + this.vagueSel + "_Chrono.json");

            if (System.IO.File.Exists(nomFichBkp))
            {
                System.IO.File.Move(nomFichBkp, nomFichBkp.Replace(".json", "_" + System.Guid.NewGuid().ToString() + ".json"));
            }

            using (var stream = File.Open(nomFichBkp, FileMode.Create))
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                DataContractJsonSerializerSettings Settings =
            new DataContractJsonSerializerSettings
            { UseSimpleDictionaryFormat = true };

                try
                {
                    using (var writer = JsonReaderWriterFactory.CreateJsonWriter(
                        stream, Encoding.UTF8, true, true, "  "))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(MainWindowViewModel), Settings);
                        serializer.WriteObject(writer, this);
                        writer.Flush();
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.ToString());
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }


            IEnumerable<PatineurCourse> enumpc = this.patcs.Distinct(new ComparateurPatineur()).ToList();


            foreach(PatineurCourse pcg in enumpc) {

                string nomFich = System.IO.Path.Combine(ParamCommuns.Instance.RepCL, pcg.Serie + pcg.Vague + ".cl");

                if (System.IO.File.Exists(nomFich))
                {
                    System.IO.File.Move(nomFich, nomFich.Replace(".cl", "_" + System.Guid.NewGuid().ToString() + ".cl"));
                }

                Resultats.Resultats res = new Resultats.Resultats();

         
            foreach (PatineurCourse pc in this.patcs.Where(pc => pc.Serie == pcg.Serie && pc.Bloc == pcg.Bloc && pc.Vague == pcg.Vague).OrderBy(zz => zz.Casque))
            {
                    res.AjouterResultatPatineur(pc,null);
            }
                bool? rep = res.ShowDialog();
                nomFich = System.IO.Path.Combine(ParamCommuns.Instance.RepCL, pcg.Serie + pcg.Vague + ".cl");


                res.CreerPat(nomFich);
                res.CreerLogSerieVague();

            }

            ProgrammeCourseMgr.RafraichirCompe();
            ProgrammeCourseMgr.ReprendreMajCompe();

            // Passer au suivant
            if (this.vagues.IndexOf(this._vagueSel) == this.vagues.Count() - 1)
            {
                // Il faut passer à la série suivante                   
                if (this.series.IndexOf(this._serieSel) == this.series.Count() - 1)
                {
                    // Il faut passer au bloc suivant
                    if (this.blocs.IndexOf(this._blocSel) == this.blocs.Count() - 1)
                    {
                        // Est-ce qu'on a autre chose à charger du programme
                        MessageBox.Show("Charger la suite du programme");
                    }
                    else
                    {
                        this.blocSel = this.blocs[this.blocs.IndexOf(this._blocSel) + 1];
                    }
                }
                else
                {
                    this.serieSel = this.series[this.series.IndexOf(this._serieSel) + 1];
                }
            }
            else
            {
                this.vagueSel = this.vagues[this.vagues.IndexOf(this._vagueSel) + 1];
            }
        }

        public void CompteTour_Click(object sender, RoutedEventArgs e)
        {
            if (this.ct == null)
            {
                this.ct = new CompteTour();
            }

            ProgrammeCourse pcc = this.Pcg.SingleOrDefault(z => z.Bloc == this._blocSel && z.Serie == this._serieSel);
            if (pcc != null)
            {
                ct.Initialiser((int)pcc.NbTour);
            }

            ct.Show();
        }

        public void DiffusionWeb_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format("Diffusion du compte tour sur le web ({0}?", this.diffusionWeb), "Diffusion Web", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ccw = new Chat.ChronoSignalR();
                if (this.patcs != null)
                {
                    foreach (PatineurCourse pc in this.patcs)
                    {
                        pc.AssignerChronoWeb = ccw;
                    }

                    foreach (KeyValuePair<Chat.ChronoSignalR.TypeMessage, string> kvp in this.logMessages.ToList())
                    {
                        this.afficherMessageWeb(kvp.Key, kvp.Value);
                    }
                }
                //Thread.Sleep(2000);
                //ccw.MessageChrono("1");
                //Thread.Sleep(2000);
                //ccw.MessageChrono("4");
                //Thread.Sleep(2000);
                //ccw.MessageChrono("2");
                //Thread.Sleep(2000);
                //ccw.MessageChrono("3");
                //Thread.Sleep(2000);
                //ccw.MessageChrono("5");


                //BackgroundWorker bw = new BackgroundWorker();
                //bw.DoWork += Bw_DoWork;
                //bw.RunWorkerAsync();
                //bw




                //this.are = new AutoResetEvent(false);

                //Thread thread = new Thread(new ThreadStart(WorkThreadFunction));
                //this.are.Reset();                
                //thread.Start();
                ////thread.ExecutionContext
                //Thread.Sleep(2000);
                //this.message = "1";
                //this.are.Set();
                //Thread.Sleep(2000);
                //this.message = "2";
                //this.are.Set();
                //Thread.Sleep(2000);
                //this.message = "5";                
                //this.are.Set();
                //Thread.Sleep(2000);
                //this.message = "4";
                //this.are.Set();
                //Thread.Sleep(2000);
                //this.message = "3";
                //this.are.Set();

                ////////if (chat == null) {
                //////    AutoResetEvent are = new AutoResetEvent(true);

                //////Chat.ChatWithServer2.Msg = "1";
                //////are.Reset();
                //////Chat.ChatWithServer2.Main(are);
                //////Chat.ChatWithServer2.Msg = "2";

                ////////chat = Chat.ChatWithServer2(are);
                ////////    Task t = chat.init();                    
                ////////    t = chat.StatutCourse(this.lblEtat.Content as string);
                ////////    t.Wait();
                ////////}
            }
            else
            {
                this.diffusionWeb = false;
            }
        }

        public void Interrompre_Click(object sender, RoutedEventArgs e)
        {
            DateTime actuel = DateTime.Now;
            foreach (PatineurCourse pc in this.patcs)
            {
                pc.Reprise(actuel);
            }
            this.lblEtatTxt = "REPRISE";
            this.lblEtatBackground = new SolidColorBrush(Colors.Yellow);
        }

        public void Sauvegarde_Click(object sender, RoutedEventArgs e)
        {
            this.SauvegarderClCourse();
        }

        private void MgrInput_DepartEventHandler(object sender, ChronistickInput.ChronstickInputEventArgs e)
        {
            Dispatcher.Invoke((Action)delegate ()
            {
                /* update UI */

                if (e.Evenement.NoPatineur == 1)
                {
                    // On reset le chrono général seulement pour le premier patineur
                    this.depart = e.Evenement.Heure;
                    ParamCommuns.Instance.Etat = ParamCommuns.EtatEnum.EnCourse;
                    this.lblEtatTxt = "En Course";
                    this.lblEtatBackground = new SolidColorBrush(Colors.Green);
                    this.lblDepartTxt = string.Format("{0}h:{1}m:{2}.{3}s", e.Evenement.Heure.Hour, e.Evenement.Heure.Minute, e.Evenement.Heure.Second, System.Convert.ToInt32(e.Evenement.Heure.Millisecond / 100));
                    this.lblChronoTxt = "";
                    this.lblFinTxt = "";
                }

                if (this.patcs.Count() >= e.Evenement.NoPatineur)
                {
                    this.patcs[e.Evenement.NoPatineur - 1].Depart(e.Evenement.Heure);
                }

                if (this.dispatcherTimer == null)
                {
                    dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                    dispatcherTimer.Tick += DispatcherTimer_Tick;
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                }
                dispatcherTimer.Start();

                if (e.Evenement.NoPatineur == 1 && this.ct != null)
                {
                    // encore une fois, on décrement les tours seulement au départ du patineur 1
                    double reste = this.nbTourDbl - this.nbTourInt;
                    if (reste <= 0.1)
                    {
                        // Nombre de tour d'une course complète décrémente dès le départ
                        this.ct.DecrementerTour();
                    }
                }
            });
        }

        public void MainWindow_Closed(object sender, EventArgs e)
        {
            mgrInput.Terminer(0);
        }

        public void OuvrirResultat_Click(object sender, RoutedEventArgs e)
        {
            Resultats.Resultats.ResultatData oox = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "fichiers Data.json (*Data.json)|*Data.json";
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string nomFichier = openFileDialog.FileName;
                if (System.IO.File.Exists(nomFichier))
                {
                    using (var stream = File.Open(nomFichier, FileMode.Open))
                    {
                        var currentCulture = Thread.CurrentThread.CurrentCulture;
                        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                        DataContractJsonSerializerSettings Settings =
                    new DataContractJsonSerializerSettings
                    { UseSimpleDictionaryFormat = true };

                        try
                        {
                            StreamReader sr = new StreamReader(stream);
                            string txt = sr.ReadToEnd();
                            byte[] b = System.Text.Encoding.UTF8.GetBytes(txt);
                                var serializer = new DataContractJsonSerializer(typeof(Resultats.Resultats.ResultatData), Settings);
                            MemoryStream ms = new MemoryStream(b);
                            oox = serializer.ReadObject(ms) as Resultats.Resultats.ResultatData;

                            ms.Close();
                            //}

                        }
                        catch (Exception exception)
                        {
                            Debug.WriteLine(exception.ToString());
                        }
                        finally
                        {
                            Thread.CurrentThread.CurrentCulture = currentCulture;
                        }
                    }
                }
            }
            if (oox != null) {
                Resultats.Resultats res = new Resultats.Resultats();
                res.ResultatObj = oox;
                res.ResultatInit(oox);
                bool? rep = res.ShowDialog();
                if (rep.HasValue && rep.Value)
                {
                    string nomFich = System.IO.Path.Combine(ParamCommuns.Instance.RepCL, oox.LPatCourse.First().Serie + oox.LPatCourse.First().Vague + ".cl");

                    if (System.IO.File.Exists(nomFich))
                    {
                        System.IO.File.Move(nomFich, nomFich.Replace(".cl", "_" + System.Guid.NewGuid().ToString() + ".cl"));
                    }

                    res.CreerPat(nomFich);
                    res.CreerLogSerieVague();
                }
            }
        }

        public void OuvrirProgramme_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.DefaultExt = ".pat";
            openFileDialog.Filter = "fichiers PAT (*.pat)|*.pat";
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string nomFichier = openFileDialog.FileName;
                if (System.IO.File.Exists(nomFichier))
                {
                    this.nomFichierPat = nomFichier;
                    FileInfo fi = new FileInfo(this.nomFichierPat);
                    this.cheminFichierPath = fi.DirectoryName;
                    //"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\Bruno\Documents\Arpvq 2017-2018\TestsChrono\Publication.pat;User Id=admin;Password=;" providerName="System.Data.OleDb"
                    string connectString = string.Format(@"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; User Id = admin; Password =; ", nomFichier);
                    //List<ProgrammeCourse> lpgm = null;
                    //Dictionary<string, List<PatineurVague>> lpv = null;
                    ProgrammeCourseMgr pgm = new ProgrammeCourseMgr();
                    int noCompt = -1;
                    string nomCompe;
                    pgm.Obtenir(connectString, out noCompt, out nomCompe);
                    if (noCompt != -1)
                    {
                        ParamCommuns.Instance.NomFichierPat = this.nomFichierPat;
                        ParamCommuns.Instance.NomCompetition = nomCompe;
                        ParamCommuns.Instance.NoCompetition = noCompt;                       
                        this.OuvrirCompetition(nomCompe, noCompt);
                    }
                }
            }            
        }

        public void OuvrirCompetition(string nomCompe, int noCompt)
        {
            this.TitreFenetre = nomCompe;
            this.noCompe = noCompt;
            this.vagues.Clear();
            this.series.Clear();
            this.blocs.Clear();
            if (this.Pcg != null)
            {
                this.Pcg.Select(z => z.Bloc).Distinct().ToList().ForEach(z => this.blocs.Add(z));
                this.blocSel = this.blocs.First();
            }
            this.blocIsEnabled = true;
            this.serieIsEnabled = true;
            this.vagueIsEnabled = true;
        }

        public void Joindre_Click(object sender, RoutedEventArgs e)
        {
            JoindreSerieVaguesViewModel jvm = new JoindreSerieVaguesViewModel(this.Pcg, this.series, this.vagues, this.blocSel, this.serieSel, this.vagueSel);
            JoindreSerieVague jvmIde = new JoindreSerieVague();
            jvmIde.DataContext = jvm;
            bool? rep = jvmIde.ShowDialog();
            if (rep.HasValue && rep.Value)
            {  // Il est temps d'ajouter des patineurs à la sélections de patineurs
                int serie = jvm.serieSel;
                string vague = jvm.vagueSel;
                int serie1 = jvm.serieSel1;
                string vague1 = jvm.vagueSel1;
                int serie2 = jvm.serieSel2;
                string vague2 = jvm.vagueSel2;
                List<KeyValuePair<int, string>> lkvp = new List<KeyValuePair<int, string>>();
                KeyValuePair<int, string> kvp = new KeyValuePair<int, string>(serie, vague);
                lkvp.Add(kvp);
                if (!string.IsNullOrEmpty(vague1) && vague1 != "Aucun")
                {
                    kvp = new KeyValuePair<int, string>(serie1, vague1);
                    lkvp.Add(kvp);
                }
                if (!string.IsNullOrEmpty(vague2) && vague2 != "Aucun")
                {
                    kvp = new KeyValuePair<int, string>(serie2, vague2);
                    lkvp.Add(kvp);
                }

                this.PreparerVagues(lkvp);
            }

        }


        private void NotifierChangementPropriete(string nom)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(nom));
            }
        }


    }
}
