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
            this.InfoResultatDataElement = new InfoResultatData();
            this.InfoResultatDataElement.PropertyChanged += InfoResultatDataElement_PropertyChanged;
        }

        public InfoResultatPatineur(PatineurCourse patc, int nbPatVag)
        {
            InitializeComponent();
            this.DataContext = this;
            this.InfoResultatDataElement = new InfoResultatData();
            this.InfoResultatDataElement.PropertyChanged += InfoResultatDataElement_PropertyChanged;
            this.RangPossibles = new ObservableCollection<int>();
            for (int i = 1; i <= nbPatVag; i++)
            {
                this.RangPossibles.Add(i);
            }            
            this.AjouterResultatPatineur(patc);            
            this.NotifierChangementPropriete("NomPatineur");
            this.NotifierChangementPropriete("CasquePatineur");
            this.NotifierChangementPropriete("ClubPatineur");            
        }

        private void InfoResultatDataElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(e.PropertyName));
                this.PropertyChanged(this, new PropertyChangedEventArgs("RangPatineur"));
            }
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

        public int RangPatineur
        {
            get
            {
                return InfoResultatDataElement.RangPatineur;
            }
            set
            {
                InfoResultatDataElement.RangPatineur = value;
                this.NotifierChangementPropriete("RangPatineur");
            }
        }

        public ObservableCollection<int> RangPossibles
        {
            get
            {
                return InfoResultatDataElement.RangPossibles;
            }
            set
            {
                InfoResultatDataElement.RangPossibles = value;
                this.NotifierChangementPropriete("RangPossibles");
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
        public class InfoResultatData : INotifyPropertyChanged
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

            private int _RangPatineur = int.MinValue;
            [DataMember(IsRequired = false)]
            public int RangPatineur
            {
                get
                { return this._RangPatineur; }
                set
                { 
                    int ancien = this._RangPatineur;
                    this._RangPatineur = value;
                    if (ancien != int.MinValue && value != ancien)
                    {
                        InfoResultatData ancient = this.RangIRP[ancien];
                        InfoResultatData Deplace = this.RangIRP[ancien];
                        //string previous = string.Empty;
                        if (ancien < this._RangPatineur) {
                            
                            for (int i = ancien ; i < this._RangPatineur; i++)
                            {
                                this.RangIRP[i] = this.RangIRP[i + 1];
                                this.RangIRP[i]._RangPatineur = i;
                                if (i == ancien) {
                                  //  previous = this.RangIRP[i].TempsPatineur;
                                  //this.RangIRP[i].TempsPatineur = ancient.TempsPatineur;
                                    this.RangIRP[i].TempsPatineur = RangTemps[i];    
                                }
                                else
                                {
                                    //this.RangIRP[i].TempsPatineur = previous;
                                    this.RangIRP[i].TempsPatineur = RangTemps[i];
                                }

                                //if (this.RangIRP[i].PropertyChanged != null) { this.RangIRP[i].PropertyChanged(this, new PropertyChangedEventArgs("RangPatineur")); }
                                //if (this.RangIRP[i].PropertyChanged != null) { this.RangIRP[i].PropertyChanged(this, new PropertyChangedEventArgs("Temps")); }
                                if (this.RangIRP[i]._RangPatineur == Deplace.RangPatineur)
                                {
                                    break;
                                }
                            }
                            this.RangIRP[this._RangPatineur] = Deplace;
                            this.TempsPatineur = this.RangTemps[this._RangPatineur];
                            //if (this.PropertyChanged != null) { this.PropertyChanged(this, new PropertyChangedEventArgs("TempsPatineur")); }
                        }
                        if (ancien > this._RangPatineur)
                        {
                            for (int i = ancien ; i > this._RangPatineur; i--)
                            {
                                this.RangIRP[i] = this.RangIRP[i - 1];
                                this.RangIRP[i]._RangPatineur = i;
                                this.RangIRP[i].TempsPatineur = this.RangTemps[this.RangIRP[i]._RangPatineur];
                                //if (this.RangIRP[i].PropertyChanged != null) { this.RangIRP[i].PropertyChanged(this, new PropertyChangedEventArgs("RangPatineur")); }
                                //if (this.RangIRP[i].PropertyChanged != null) { this.RangIRP[i].PropertyChanged(this, new PropertyChangedEventArgs("Temps")); }
                            }

                            this.RangIRP[this._RangPatineur] = Deplace;
                            //this.RangIRP[this._RangPatineur].TempsPatineur = previous;
                            this.RangIRP[this._RangPatineur].TempsPatineur = this.RangTemps[this._RangPatineur];
                            //if (this.RangIRP[this._RangPatineur].PropertyChanged != null) { this.RangIRP[this._RangPatineur].PropertyChanged(this, new PropertyChangedEventArgs("Temps")); }
                        }
                    }

                    //if (this.PropertyChanged != null) { this.PropertyChanged(this, new PropertyChangedEventArgs("RangPatineur")); }

                    foreach (InfoResultatData o in this.RangIRP.Values)
                    {
                        o.PropertyChanged(o, new PropertyChangedEventArgs("TempsPatineur"));
                        o.PropertyChanged(o, new PropertyChangedEventArgs("RangPatineur"));
                    }
                }
            }

           
            public ObservableCollection<int> RangPossibles { get; set; }

            private static Dictionary<int, string> _rangTemps = new Dictionary<int, string>();
            private static Dictionary<int, InfoResultatData> _rangIRP = new Dictionary<int, InfoResultatData>();

            public event PropertyChangedEventHandler PropertyChanged;

            public  Dictionary<int, string> RangTemps { get { return _rangTemps; } }
            public Dictionary<int, InfoResultatData> RangIRP { get { return _rangIRP; } }

            [DataMember(IsRequired = false)]
            public ObservableCollection<EvenementPatineur> Evenements { get; set; }
            //private EvenementPatineur _evenementSel;

            [DataMember(IsRequired =false)]
            public EvenementPatineur EvenementSel { get; set; }
        }

    }
}
