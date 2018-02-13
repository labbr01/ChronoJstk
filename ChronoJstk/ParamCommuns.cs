using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChronoJstk
{
    public class ParamCommuns
    {
        private static ParamCommuns _Instance = null;
        private ParamCommuns()
        {
        }

        public static ParamCommuns Instance { get
            {
                if (_Instance == null)
                {
                    _Instance = new ParamCommuns();
                }
                return _Instance;
            }
        }

        public string NomFichierPat { get; set; }
        public int NoCompetition { get; set; }
        public string NomCompetition { get; set; }
        public bool GroupesMixtes { get; set; }
        public string SignalRServer { get; set; }
        public string SignalRHub { get; set; }
        public string UsagerFTP { get; set; }
        public string SiteFTP { get; set; }
        public string TravailFTP { get; set; }
        //public string UrlHub { get; set; }
        public string RepCL { get; set; }
        public string RepLog { get; set; }
        public string UrlWeb { get; set; }
        //public string SignalRHub { get; set; }
        public string HostConfig { get; set; }
        public ModeDiffusion WebChrono { get; set; }
        public ModeDiffusion WebResultat { get; set; }
        public string MotPasse { get; set; }
        public int PortInput { get; set;  }
        public int PortConfig { get; set; }
        public string ChronistickInputExe { get; set; }

        public List<string> Comps { get; set; } = new List<string>();
        public List<int> NoComps { get; set; } = new List<int>();

        public ObservableCollection<ProgrammeCourse> Programmes { get; set; }
        public Dictionary<string, List<PatineurVague>> DescVagues { get; set; }
        public EtatEnum Etat { get; set; }

        public enum ModeDiffusion
        {
            Non = 0,
            Web = 1,
            BT = 2
        }

        public enum EtatEnum
        {
            Defaut = 0,
            EnCourse = 1,
            CourseTerminee = 2,
            ProgrammeCharge = 3            
        }
    }
}
