using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ChronoJstk
{
    [DataContract]
    public class JoindreSerieVaguesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public JoindreSerieVaguesViewModel(ObservableCollection<ProgrammeCourse> pcg,
            ObservableCollection<int> series,
            ObservableCollection<string> vagues,
            int blocSel,
            int serieSel,
            string vagueSel)
        {
            this.pcg = pcg;
            this.series = series;
            this.vagues = vagues;
            this._blocSel = blocSel;
            this._serieSel = serieSel;
            this._vagueSel = vagueSel;
            this.vagues1 = new ObservableCollection<string>();
            this.vagues2 = new ObservableCollection<string>();           
        }

        private ObservableCollection<ProgrammeCourse> pcg = null;
        public ObservableCollection<ProgrammeCourse> Pcg { get { return this.pcg; } set { this.pcg = value; } }

        [DataMember]
        public ObservableCollection<int> series { get; private set; }

        [DataMember]
        public ObservableCollection<string> vagues { get; private set; }

        [DataMember]
        public ObservableCollection<string> vagues1 { get; private set; }

        [DataMember]
        public ObservableCollection<string> vagues2 { get; private set; }
     
        private int _blocSel;
        [DataMember]
        public int blocSel
        {
            get { return this._blocSel; }
            set
            {
                this._blocSel = value;
                if (this.pcg != null)
                {
                    this.series.Clear();
                    pcg.Where(z => z.Bloc == this._blocSel).Select(z => z.Serie).Distinct().ToList().ForEach(z => this.series.Add(z));
                    this.serieSel = this.series.First();
                    this.NotifierChangementPropriete("blocSel");
                }
                //this.afficherMessageWeb(Chat.ChronoSignalR.TypeMessage.Bloc, this._blocSel.ToString());
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
                if (this.pcg != null)
                {
                    this.vagues.Clear();
                    this.NotifierChangementPropriete("serieSel");
                    ProgrammeCourse pcc = pcg.SingleOrDefault(z => z.Bloc == this._blocSel && z.Serie == this._serieSel);
                    this.vagues.Clear();
                    pcc.LVagues.ForEach(z => this.vagues.Add(z));
                    this.vagueSel = this.vagues.First();
                }
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
                this.NotifierChangementPropriete("_vagueSel");
            }
        }

        private int _serieSel1;
        [DataMember]
        public int serieSel1
        {
            get { return this._serieSel1; }
            set
            {
                this._serieSel1 = value;
                if (this.pcg != null)
                {
                    this.vagues1.Clear();
                    this.NotifierChangementPropriete("serieSel1");
                    ProgrammeCourse pcc = pcg.SingleOrDefault(z => z.Bloc == this._blocSel && z.Serie == this._serieSel1);
                    this.vagues1.Clear();
                    this.vagues1.Add("Aucun");
                    pcc.LVagues.ForEach(z => {
                        if (!(this.serieSel == this.serieSel1 && this.vagueSel == z)) { 
                        this.vagues1.Add(z);
                        }
                    });
                    if (this.vagues1.Count() > 0)
                    {
                        this.vagueSel1 = this.vagues1.First();
                    }
                    else
                    {
                        this.vagueSel1 = string.Empty;
                    }                    
                }
            }
        }
        private string _vagueSel1;

        [DataMember]
        public string vagueSel1
        {
            get { return this._vagueSel1; }
            set
            {
                this._vagueSel1 = value;
                this.NotifierChangementPropriete("_vagueSel1");
            }
        }

        private int _serieSel2;
        [DataMember]
        public int serieSel2
        {
            get { return this._serieSel2; }
            set
            {
                this._serieSel2 = value;
                if (this.pcg != null)
                {
                    this.vagues2.Clear();
                    this.NotifierChangementPropriete("serieSel2");
                    ProgrammeCourse pcc = pcg.SingleOrDefault(z => z.Bloc == this._blocSel && z.Serie == this._serieSel2);
                    this.vagues2.Clear();
                    this.vagues2.Add("Aucun");
                    pcc.LVagues.ForEach(z => 
                    {
                        if (!((this.serieSel == this.serieSel2 && this.vagueSel == z) || (this.serieSel1 == this.serieSel2 && this.vagueSel1 == z)))
                        {
                            this.vagues2.Add(z);
                        }
                    });                    
                    if (this.vagues2.Count() > 0)
                    { 
                        this.vagueSel2 = this.vagues2.First();
                    }
                    else
                    {
                        this.vagueSel2 = string.Empty;
                    }
                }
            }
        }
        private string _vagueSel2;

        [DataMember]
        public string vagueSel2
        {
            get { return this._vagueSel2; }
            set
            {
                this._vagueSel2 = value;
                this.NotifierChangementPropriete("_vagueSel2");
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
