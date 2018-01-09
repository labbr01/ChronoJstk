using System;
using System.Collections.Generic;
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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static ChronoJstk.Resultats.InfoResultatPatineur;

namespace ChronoJstk.Resultats
{
    /// <summary>
    /// Logique d'interaction pour Resultats.xaml
    /// </summary>    
    public partial class Resultats : Window
    {
        //private List<PatineurCourse> lpatc = new List<PatineurCourse>();
        //private List<InfoResultatPatineur> lipatc = new List<InfoResultatPatineur>();
        public Resultats()
        {
            InitializeComponent();
            this.ResultatObj = new ResultatData();
            this.Cancel.Visibility = Visibility.Hidden;
        }

        public void CalculerRang()
        {
            int i = 1;
            foreach (InfoResultatData ird in LInfoResultatPatineur.OrderBy(z => z.EvenementSel.TempsElapse))
            {
                // Index 0 doit avoir le rang 1!
                ird.RangPatineur = ird.RangPossibles.IndexOf(i) + 1;
                ird.RangTemps[ird.RangPatineur] = ird.TempsPatineur;
                ird.RangIRP[ird.RangPatineur] = ird;
                i++;
            }

         
        }

        public void AjouterResultatPatineur(PatineurCourse patc, InfoResultatData irpx, int nbPatVag)
        {
            this.ResultatObj.Titre = (string.Format("Heure {2}: Série {0}, Vague {1} : {0}{1}.cl", patc.Serie, patc.Vague, DateTime.Now));
            this.Title = this.ResultatObj.Titre;
            InfoResultatPatineur irp = null;
            if (irpx != null)
            {
                irp = new InfoResultatPatineur(patc, nbPatVag);
                irp.InfoResultatDataElement = irpx;
                irp.PropertyChanged += Irp_PropertyChanged;
            }
            else
            {
                irp = new InfoResultatPatineur(patc, nbPatVag);
                irp.PropertyChanged += Irp_PropertyChanged;
            }
             
            LInfoResultatPatineur.Add(irp.InfoResultatDataElement);
            if (!this.LPatCourse.Contains(patc)) { 
                this.LPatCourse.Add(patc);
            }
            //this.lpatc.Add(patc);
            SpEvenements.Children.Add(irp);

            //if (!string.IsNullOrEmpty(irp.InfoResultatDataElement.TempsSauvegarde)) { 
            //    irp.TempsPatineur = irp.InfoResultatDataElement.TempsSauvegarde;
            //}
        }

        private void Irp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public ResultatData ResultatObj { get; set; }

        public void ResultatInit(ResultatData r)
        {
            List<InfoResultatData> oldList = new List<InfoResultatData>(LInfoResultatPatineur);            

            LInfoResultatPatineur.Clear();
            this.ResultatObj = r;
            int i = 0;
            int npPatVag = this.ResultatObj.LPatCourse.Count();
            foreach (PatineurCourse pc in this.ResultatObj.LPatCourse)
            {
                this.AjouterResultatPatineur(pc, oldList[i], npPatVag);
                i += 1;
            }

            // Calculer le rang du patineur dans la liste
            this.CalculerRang();


            this.Cancel.Visibility = Visibility.Visible;
        }

        public string Titre { get { return this.Title; } set { this.Title = value; this.ResultatObj.Titre = value; } }

        [DataMember]
        public List<InfoResultatData> LInfoResultatPatineur { get { return ResultatObj.LInfoResultatPatineur; } set { this.ResultatObj.LInfoResultatPatineur = value; } }

        [DataMember]
        public List<PatineurCourse> LPatCourse { get { return ResultatObj.LPatCourse; } set { ResultatObj.LPatCourse = value; } }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// Créer un patineur
        /// </summary>
        /// <param name="nomFich"></param>
        public void CreerPat(string nomFich)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("----\t\t\t");
            int i = 1;
            foreach (InfoResultatData pc in this.LInfoResultatPatineur)
            {
                string tempx = pc.TempsPatineur.Replace(",", ".");
                if (!tempx.Contains(":") && tempx.Length == 6) {
                    tempx = tempx.Substring(0, 2) + ":" + tempx.Substring(2, 2) + "." + tempx.Substring(3, 2);
                }
                sb.AppendLine(string.Format("   {0}\t     {0}    \t{1}", i, tempx));
                pc.TempsSauvegarde = tempx;
                i += 1;
            }

            using (System.IO.StreamWriter sr = new System.IO.StreamWriter(nomFich))
            {
                sr.Write(sb.ToString());
            }
        }

        public void CreerLogSerieVague()
        {
            int serie = this.LPatCourse.First().Serie;
            string vague = this.LPatCourse.First().Vague;
            string dirLog = System.IO.Path.Combine(ParamCommuns.Instance.RepLog, ParamCommuns.Instance.NomCompetition.Replace("/", "_").Replace("\\", "_").Replace(" ", "_"));
            if (!System.IO.Directory.Exists(dirLog))
            {
                System.IO.Directory.CreateDirectory(dirLog);
            }

            string nomFichBkp = System.IO.Path.Combine(dirLog, serie + vague + "_Data.json");

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
                        var serializer = new DataContractJsonSerializer(typeof(Resultats.ResultatData), Settings);
                        serializer.WriteObject(writer, this.ResultatObj);
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
        }

    [DataContract]
        public class ResultatData
        {
            public ResultatData()
            {
                this.LPatCourse = new List<PatineurCourse>();
                this.LInfoResultatPatineur = new List<InfoResultatData>();
            }
            [DataMember(IsRequired = false)]
            public string Titre { get; set; }

            [DataMember(IsRequired = false)]
            public List<InfoResultatData> LInfoResultatPatineur { get; set; }

            [DataMember(IsRequired = false)]
            public List<PatineurCourse> LPatCourse { get; set; }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
