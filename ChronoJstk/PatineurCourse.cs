using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChronoJstk
{
    [DataContract]
    public class PatineurCourse : INotifyPropertyChanged
    {
        Chat.ChronoSignalR ccw;
        public PatineurCourse()
        {
            this.Termine = false;
            this.NbTourAffiche = int.MaxValue;
        }

        public event EventHandler CourseTerminee;

        public event EventHandler TourComplete;

        [DataMember(IsRequired = false)]
        public string Casque { get; set; }
        [DataMember(IsRequired = false)]
        public int Bloc { get; set; }
        [DataMember(IsRequired = false)]
        public int Serie { get; set; }
        [DataMember(IsRequired = false)]
        public string Vague { get; set; }
        [DataMember(IsRequired = false)]
        public PatineurVague Patineur { get; set; }
        [DataMember(IsRequired = false)]
        public int NbTourCourse { get; set; }
        [DataMember(IsRequired = false)]
        public bool Termine { get; set; }

        [DataMember(IsRequired = false)]
        public int NoCasqueWeb { get; set;  }

        private int _NbTour = int.MinValue;
        [DataMember(IsRequired = false)]
        public int NbTour
        {
            get
            {
                return this._NbTour;
            }
            set
            {
                this._NbTour = value;
                this.NotifierChangementPropriete("NbTour");

            }
        }

        [DataMember(IsRequired = false)]
        public int NbTourAffiche { get; set; }

        private string _DernierTour;
        [DataMember(IsRequired = false)]
        public string DernierTour { get { return this._DernierTour; } set { this._DernierTour = value; this.NotifierChangementPropriete("DernierTour"); } }

        private string _DernierTemps;
        [DataMember(IsRequired = false)]
        public string DernierTemps { get { return this._DernierTemps; } set { this._DernierTemps = value; this.NotifierChangementPropriete("DernierTemps"); } }

        [DataMember(IsRequired = false)]
        public string Temps { get; set; }
        [DataMember(IsRequired = false)]
        public ObservableCollection<EvenementPatineur> Evenements { get; set; }

        public EvenementPatineur _EvenementSel;
        [DataMember(IsRequired = false)]
        public EvenementPatineur EvenementSel
        {
            get
            { return this._EvenementSel; }
            set
            {
                this._EvenementSel = value; this.NotifierChangementPropriete("EvenementSel");
                if (this._EvenementSel != null) { 
                this.DernierTemps = this._EvenementSel.DernierTemps();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;        

        public void Reprise(DateTime dt)
        {
            this.Termine = false;
            EvenementPatineur ep = new EvenementPatineur();
            ep.HeureEvenement = dt;
            ep.TempsElapse = dt - dt;
            ep.Evenement = "REPRISE";
            ep.NbTour = this.NbTourCourse;
            this.Evenements.Add(ep);
            this._HeureDepart = dt;
            NotifierChangementPropriete("HeureDepart");
            this.DernierTour = string.Empty;
            this.DernierTemps = string.Empty;

            NotifierWeb();
        }

        public void Depart(DateTime dt)
        {
            this.Termine = false;
            EvenementPatineur ep = new EvenementPatineur();
            ep.HeureEvenement = dt;
            ep.TempsElapse = dt - dt;
            ep.Evenement = "DEPART";
            ep.NbTour = this.NbTourCourse;
            this.Evenements.Add(ep);
            this._HeureDepart = dt;
            NotifierChangementPropriete("HeureDepart");
            this.DernierTour = string.Empty;
            this.DernierTemps = string.Empty;
            this.NbTour = ep.NbTour - 1;

            NotifierWeb();
        }

        private void NotifierWeb()
        {
            if (this.ccw != null)
            {
                string msg = "{ \"TypeMessage\" : \"Patineur\", \"NoCasqueWeb\": " + NoCasqueWeb.ToString() + ", \"Casque\": \"" + this.Casque + "\", \"Nom\": \"" + this.Patineur.Patineurs + "\", \"Club\": \"" + this.Patineur.Club + "\", \"Tour\": " + this.NbTour.ToString() + "  , \"Temps\" : \"" + this.DernierTemps + "\", \"DernierTour\" : \"" + this.DernierTour + "\" }";
                this.ccw.AfficherWeb(this.NoCasqueWeb, msg);
            }
        }

        public void ResetAffichage()
        {
            if (this.ccw != null)
            {
                string msg = "{ \"TypeMessage\" : \"Patineur\", \"NoCasqueWeb\": " + NoCasqueWeb.ToString() + ", \"Casque\":\"" + this.Casque + "\", \"Nom\": \"\", \"Club\": \"\", \"Tour\":-1, \"Temps\" : \"\", \"DernierTour\" : \"\" }";
                this.ccw.AfficherWeb(this.NoCasqueWeb, msg);
            }
        }

        public Chat.ChronoSignalR AssignerChronoWeb
        {
            set
            {
                this.ccw = value;
                this.NotifierWeb();
            }
        }

        public void NonFinDepart()
        {
            if (ParamCommuns.Instance.Etat != ParamCommuns.EtatEnum.EnCourse)
            {
                if (this.Evenements == null)
                {
                    this.Evenements = new ObservableCollection<EvenementPatineur>();
                }

                EvenementPatineur ep = new EvenementPatineur();
                ep.Evenement = "NFDTOUR";
                ep.HeureEvenement = DateTime.Now;
                ep.TempsElapse = new TimeSpan(23, 59, 59);
                ep.NbTour = 0;
                this.Evenements.Add(ep);
                //MessageBox.Show("La course doit être en cours afin d'enregistrer un temps");
                return;
            }

            if (!this.Termine)
            {
                EvenementPatineur ep = new EvenementPatineur();
                ep.Evenement = "TOUR";
                ep.HeureEvenement = DateTime.Now;
                ep.TempsElapse = new TimeSpan(23,59,59);
                ep.NbTour = 0;
                this.Evenements.Add(ep);
                this.DernierTemps = ep.DernierTemps();
                this.FinCourse(ep.HeureEvenement, ep.TempsElapse);
            }
        }

        public void Tour()
        {
            this.Tour(DateTime.Now);
        }

        public void Tour(DateTime temps)
        {
            if (ParamCommuns.Instance.Etat != ParamCommuns.EtatEnum.EnCourse)
            {
                if (this.Evenements == null)
                {
                    this.Evenements = new ObservableCollection<EvenementPatineur>();
                }

                EvenementPatineur ep = new EvenementPatineur();
                ep.Evenement = "NTOUR";
                ep.HeureEvenement = DateTime.Now;
                ep.TempsElapse = new TimeSpan(23, 59, 59);
                ep.NbTour = 0;
                this.Evenements.Add(ep);
                //MessageBox.Show("La course doit être en cours afin d'enregistrer un temps");
                return;
            }

            if (this.Evenements == null || this.Evenements.Count == 0)
            {
                if (this.Evenements == null)
                {
                    this.Evenements = new ObservableCollection<EvenementPatineur>();
                }

                EvenementPatineur ep = new EvenementPatineur();
                ep.Evenement = "NTOUR";
                ep.HeureEvenement = DateTime.Now;
                ep.TempsElapse = new TimeSpan(23, 59, 59);
                ep.NbTour = 0;
                this.Evenements.Add(ep);
                //MessageBox.Show("La course doit être en cours afin d'enregistrer un temps");
                return;
            }

            EvenementPatineur eve = this.Evenements.Where(z => z.Evenement == "DEPART").Last();
            if (eve != null)
            {
                DateTime dep = eve.HeureEvenement;
                int pos = this.Evenements.IndexOf(eve);
                List<EvenementPatineur> lep = new List<EvenementPatineur>();
                for (int i = pos ; i < this.Evenements.Count(); i++)
                {
                    if (this.Evenements[i].Evenement == "TOUR" || this.Evenements[i].Evenement == "DEPART")
                    {
                        lep.Add(this.Evenements[i]);
                    }
                }
                //DateTime preced = null;
                bool decrementTour = true;
                bool finCourse = false;
                bool tourComplet = false;

                EvenementPatineur ep = new EvenementPatineur();
                if (lep.Count > 0)
                {
                    decrementTour = false;
                    int trMin = lep.Min(z => z.NbTour);
                    EvenementPatineur preced = lep.Where(z => z.NbTour == trMin).First();
                    DateTime trPreced = preced.HeureEvenement;
                    TimeSpan ts = temps - trPreced;
                    if (ts.TotalSeconds > 5)
                    {
                        decrementTour = true;
                    }

                    if (decrementTour)
                    {
                        ep.NbTour = preced.NbTour - 1;
                        this.DernierTour = ep.DernierTour(ts);

                        tourComplet = true;

                        if (ep.NbTour == 0)
                        {
                            // Enregistrer la fin de la course pour ce patineur.
                            finCourse = true;
                        }
                    }
                    else
                    {
                        ep.NbTour = preced.NbTour;
                    }

                }
                else
                {
                    ep.NbTour = this.NbTourCourse - 1;
                    tourComplet = true;
                }

                if (ep.NbTour != 0 && this.Termine)
                {
                    // On inscrit pas de temps si le patineur a terminé sa course 
                    // EXCEPTION du double tour zéro!
                    return;
                }

                ep.HeureEvenement = temps;
                ep.TempsElapse = temps - dep;
                this.DernierTemps = ep.DernierTemps();
                ep.Evenement = "TOUR";
                this.NbTour = ep.NbTour;
                this.NbTourAffiche = ep.NbTour - 1;
                this.Evenements.Add(ep);

                if (tourComplet && this.TourComplete != null)
                {
                    this.TourComplete(this, new EventArgs());
                }

                this.NotifierWeb();

                if (finCourse)
                {
                    this.FinCourse(ep.HeureEvenement, ep.HeureEvenement - dep);
                }                
            }
        }

        private void FinCourse(DateTime heure, TimeSpan chrono)
        {
            EvenementPatineur epf = new EvenementPatineur();
            epf.HeureEvenement = heure;
            epf.Evenement = "FIN";
            epf.TempsElapse = chrono;
            this.Evenements.Add(epf);
            this.Termine = true;

            if (this.CourseTerminee != null)
            {
                this.CourseTerminee(this, new EventArgs());
            }
        }

        public void Fin(DateTime dt)
        {
            EvenementPatineur ep = new EvenementPatineur();
            ep.HeureEvenement = dt;

            var k = this.Evenements.Where(z => z.Evenement == "DEPART");
            if (k != null && k.Count() > 0)
            {
                DateTime dep = k.Last().HeureEvenement;
                ep.TempsElapse = dt - dep;
            }
            else
            {
                ep.TempsElapse = new TimeSpan(0);
            }
            ep.Evenement = "FIN";
            this.Evenements.Add(ep);

        }

        public DateTime _HeureDepart;
        [DataMember]
        public DateTime HeureDepart
        {
            get
            {
                return _HeureDepart;
            }
            set
            {
                this._HeureDepart = value;
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
