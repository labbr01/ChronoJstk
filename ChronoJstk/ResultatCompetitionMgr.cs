using Newtonsoft.Json.Linq;
using PVModele;
using PVModele.Tables;
using ResultatPourWeb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChronoJstk
{
    class ResultatCompetitionMgr
    {
        private static ResultatCompetitionMgr _instance;
        private ResultatCompetitionMgr()
        {
        }

        public static ResultatCompetitionMgr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ResultatCompetitionMgr();
                }

                return _instance;
            }
        }

        public void EffacerResultats()
        {
            string ftpTravail = ParamCommuns.Instance.TravailFTP;
            string pathResultat = Path.Combine(ftpTravail, "ResultatsTravail");
            if (!Directory.Exists(pathResultat))
            {
                Directory.CreateDirectory(pathResultat);
            }
            else
            {
                System.IO.DirectoryInfo myDirInfo = new DirectoryInfo(pathResultat);
                foreach (FileInfo file in myDirInfo.GetFiles())
                {
                    file.Delete();
                }

            }
            string ResultatsPrecedents = Path.Combine(ftpTravail, "ResultatsPrecedents");
            if (!Directory.Exists(ResultatsPrecedents))
            {
                Directory.CreateDirectory(ResultatsPrecedents);
            }
            else
            {
                System.IO.DirectoryInfo myDirInfo = new DirectoryInfo(ResultatsPrecedents);
                foreach (FileInfo file in myDirInfo.GetFiles())
                {
                    file.Delete();
                }

            }

            string ResultatsFTP = Path.Combine(ftpTravail, "ResultatsFTP");
            if (!Directory.Exists(ResultatsFTP))
            {
                Directory.CreateDirectory(ResultatsFTP);
            }
            else
            {
                System.IO.DirectoryInfo myDirInfo = new DirectoryInfo(ResultatsFTP);
                foreach (FileInfo file in myDirInfo.GetFiles())
                {
                    file.Delete();
                }

            }
        }

        public List<string> ObtenirResultatCompetition()
        {
            string connectString = string.Format(@"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; User Id = admin; Password =; ", ParamCommuns.Instance.NomFichierPat);
            using (var db = new DBPatinVitesse(connectString))
            {
                string ftpUser = ParamCommuns.Instance.UsagerFTP;
                string ftpPasse = ParamCommuns.Instance.MotPasse;
                string ftpSite = ParamCommuns.Instance.SiteFTP;
                string ftpTravail = ParamCommuns.Instance.TravailFTP;
                int noCompe = ParamCommuns.Instance.NoCompetition;
                bool m = ParamCommuns.Instance.GroupesMixtes;
                
                PatineurCompe.DB = db;
                PublicationResultat pr = new PublicationResultat(noCompe,m, "json", ftpTravail, ftpSite, ftpUser, ftpPasse) ;
                JObject programme = null;
                programme = pr.InfoCompeVagues(db);
                return pr.CopierSiteFTP(programme);                
            }

        }
    }
}
