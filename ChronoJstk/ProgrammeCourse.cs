using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChronoJstk
{
    [DataContract]
    public class ProgrammeCourse
    {
        [DataMember]
        public string TypeCourse { get; set; }
        [DataMember]
        public int Bloc { get; set; }
        [DataMember]
        public string Epreuve { get; set; }
        [DataMember]
        public int Serie { get; set; }
        [DataMember]
        public string De { get; set; }
        [DataMember]
        public List<string> LVagues { get; set; }
        [DataMember]
        public string A { get; set; }
        [DataMember]
        public int Trace { get; set; }
        [DataMember]
        public double NbTour { get; set; }
    }
}
