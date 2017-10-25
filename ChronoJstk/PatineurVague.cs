using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChronoJstk
{
    [DataContract]
    public class PatineurVague
    {
        [DataMember (IsRequired =false)]
        public int Epreuve { get; set; }
        [DataMember(IsRequired = false)]
        public string Groupe { get; set; }
        [DataMember(IsRequired = false)]
        public string Vague { get; set; }
        [DataMember(IsRequired = false)]
        public int Casque { get; set; }
        [DataMember(IsRequired = false)]
        public string Patineurs { get; set; }
        [DataMember(IsRequired = false)]
        public string Club { get; set; }
        [DataMember(IsRequired = false)]
        public int Rang { get; set; }
        [DataMember(IsRequired = false)]
        public string Temps { get; set; }
        [DataMember(IsRequired = false)]
        public int Points { get; set; }
        [DataMember(IsRequired = false)]
        public string Commentaire { get; set; }
        [DataMember(IsRequired = false)]
        public DateTime Date { get; set; }
    }
}
