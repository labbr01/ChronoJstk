using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChronoJstk
{
    [DataContract]
    class EpreuveTrace
    {
        [DataMember]
        public int Trace { get; set; }

        [DataMember]
        public string Epreuve { get; set; }
    }
}
