using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChronoJstk
{
    [DataContract]
    public class EvenementPatineur
    {
        public EvenementPatineur()
        {
        }
    [DataMember]
        public int NbTour { get; set; }
        [DataMember(IsRequired = false)]
        public string Evenement { get; set; }
        [DataMember(IsRequired = false)]
        public string Temps { get; set; }
        [DataMember(IsRequired = false)]
        public TimeSpan TempsElapse { get; set; }
        [DataMember(IsRequired = false)]
        public DateTime HeureEvenement { get; set; }

        public string DernierTour(TimeSpan ts)
        {
            return string.Format("{0:00}m:{1:00}.{2:00}s", ts.Minutes, ts.Seconds, System.Convert.ToInt32(ts.Milliseconds / 100));
        }

        public string DernierTemps()
        {
            return string.Format("{0:00}m:{1:00}.{2:00}s", this.TempsElapse.Minutes, this.TempsElapse.Seconds, System.Convert.ToInt32(this.TempsElapse.Milliseconds / 100));
        }


        public string TempsEtTours
        {
            get
            {
                if (Evenement == "DEPART")
                {
                    return string.Format("DEP:{0}tr:{1}h:{2}m:{3}.{4}s", this.NbTour, this.HeureEvenement.Hour, this.HeureEvenement.Minute, this.HeureEvenement.Second, System.Convert.ToInt32(this.HeureEvenement.Millisecond/100));
                }
                else if (Evenement == "FIN")
                {
                    return string.Format("FIN:{0}tr:{1}h:{2}m:{3}.{4}s", this.NbTour, this.HeureEvenement.Hour, this.HeureEvenement.Minute, this.HeureEvenement.Second, System.Convert.ToInt32(this.HeureEvenement.Millisecond / 100));
                }
                else if (Evenement == "REPRISE")
                {
                    return string.Format("REP:{0}tr:{1}h:{2}m:{3}.{4}s", this.NbTour, this.HeureEvenement.Hour, this.HeureEvenement.Minute, this.HeureEvenement.Second, System.Convert.ToInt32(this.HeureEvenement.Millisecond / 100));
                }
                else if (Evenement == "TOUR")
                {
                    return string.Format("TOUR:{0}tr:{1:00}m:{2:00}.{3:00}s", this.NbTour, this.TempsElapse.Minutes, this.TempsElapse.Seconds, System.Convert.ToInt32(this.TempsElapse.Milliseconds / 100));
                }
                else
                {
                    return string.Format("{0}:{1}", this.NbTour, this.Temps);
                }                
            }
        }
    }
}
