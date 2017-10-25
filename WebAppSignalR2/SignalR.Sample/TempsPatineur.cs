using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppSignalR2.SignalR.Sample
{
    public class TempsPatineur // TempsPatineur est comparable à stock
    {
        public int NoCasque { get; set; }
        public string Patineur { get; set; }
        public string Club { get; set; }
        public string Temps { get; set; }
        public string DernierTour { get; set; }
        public int NbTour { get; set; }
    }
}