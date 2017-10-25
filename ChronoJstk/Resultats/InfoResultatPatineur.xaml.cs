using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
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

namespace ChronoJstk.Resultats
{
    /// <summary>
    /// Logique d'interaction pour InfoResultatPatineur.xaml
    /// </summary>    
    public partial class InfoResultatPatineur : UserControl, INotifyPropertyChanged
    {
        bool traiterEvenement = true;
        public InfoResultatPatineur()
        {
            InitializeComponent();
            this.DataContext = this;
            this.traiterEvenement = false;            
        }

        public InfoResultatPatineur(PatineurCourse patc)
        {
            InitializeComponent();
            this.InfoResultatDataElement = new InfoResultatData();
            this.DataContext = this;            
            this.AjouterResultatPatineur(patc);            
            this.NotifierChangementPropriete("NomPatineur");
            this.NotifierChangementPropriete("CasquePatineur");
            this.NotifierChangementPropriete("ClubPatineur");            
        }

        public InfoResultatData InfoResultatDataElement { get; set; }
        public string NomPatineur
        {
            get
            {
                return InfoResultatDataElement.NomPatineur;
            }
            set { InfoResultatDataElement.NomPatineur = value;
                this.NotifierChangementPropriete("NomPatineur");
            }
        }

        public string CasquePatineur
        {
            get
            {
                return InfoResultatDataElement.CasquePatineur;
            }
            set
            {
                InfoResultatDataElement.CasquePatineur = value;
                this.NotifierChangementPropriete("CasquePatineur");
            }
        }

        public string ClubPatineur
        {
            get
            {
                return InfoResultatDataElement.ClubPatineur;
            }
            set
            {
                InfoResultatDataElement.ClubPatineur = value;
                this.NotifierChangementPropriete("ClubPatineur");
            }
        }

        public string TempsPatineur
        {
            get
            {
                return InfoResultatDataElement.TempsPatineur;
            }
            set
            {
                InfoResultatDataElement.TempsPatineur = value;
                this.NotifierChangementPropriete("TempsPatineur");
            }
        }

        public ObservableCollection<EvenementPatineur> Evenements
        {
            get
            {
                return InfoResultatDataElement.Evenements;
            }
            set
            {
                InfoResultatDataElement.Evenements = value;
                this.NotifierChangementPropriete("Evenements");
            }
        }

        public EvenementPatineur EvenementSel
        {
            get
            {
                return InfoResultatDataElement.EvenementSel;
            }
            set
            {
                InfoResultatDataElement.EvenementSel = value;
                this.NotifierChangementPropriete("EvenementSel");
                if (InfoResultatDataElement.EvenementSel != null && this.traiterEvenement)
                {
                    this.TempsPatineur = string.Format(string.Format("{0:00}{1:00}{2:00}", InfoResultatDataElement.EvenementSel.TempsElapse.Minutes, this.InfoResultatDataElement.EvenementSel.TempsElapse.Seconds, System.Convert.ToInt32(this.InfoResultatDataElement.EvenementSel.TempsElapse.Milliseconds / 100)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private  void AjouterResultatPatineur(PatineurCourse patc)
        {
            this.Evenements = patc.Evenements;
            this.EvenementSel = patc.EvenementSel;
            this.NomPatineur = patc.Patineur.Patineurs;
            this.CasquePatineur = patc.Casque;
            this.ClubPatineur = patc.Patineur.Club;
            if (this.EvenementSel == null)
            {

                EvenementPatineur ep; // = patc.Evenements.SingleOrDefault(z => z.Temps == patc.Temps);
                EvenementPatineur dep = patc.Evenements.LastOrDefault(z => z.Evenement == "DEP");
                EvenementPatineur fin = patc.Evenements.LastOrDefault(z => z.Evenement == "FIN");
                int idxd = patc.Evenements.IndexOf(dep);
                int idxf = patc.Evenements.IndexOf(fin);
                ep = dep;
                if (idxf > idxd)
                {
                    ep = patc.Evenements[idxf];
                }
                if (ep != null)
                {
                    this.EvenementSel = ep;
                }
            }
        }

        private void NotifierChangementPropriete(string nom)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(nom));
            }
        }

        [DataContract]
        public class InfoResultatData
        {
            public InfoResultatData()
            {
            }

            [DataMember(IsRequired = false)]
            public string TempsSauvegarde { get; set; }

            [DataMember(IsRequired = false)]
            public string NomPatineur { get; set; }
            [DataMember(IsRequired = false)]
            public string CasquePatineur { get; set; }
            [DataMember(IsRequired = false)]
            public string ClubPatineur { get; set; }

            [DataMember(IsRequired = false)]
            public string TempsPatineur { get; set; }

            [DataMember(IsRequired = false)]
            public ObservableCollection<EvenementPatineur> Evenements { get; set; }
            //private EvenementPatineur _evenementSel;

            [DataMember(IsRequired =false)]
            public EvenementPatineur EvenementSel { get; set; }
        }

    }
}
