//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;

//namespace ChronoStick_Input.Model
//{
//    [DataContract]
//    public class Evenement
//    {
//        [DataMember]
//        public int NoPatineur { get; set; }

//        [DataMember]
//        public TypeEvenement Action { get; set; }

//        [DataMember]
//        public DateTime Heure { get; set; }

//        [DataMember]
//        public OrigineEvenement Origine { get; set; }

//        [DataMember]
//        public bool Doublon { get; set; }

//        public string Affichage { get {  return string.Format("{0}, {1}, {2}, {3}, {4} ", NoPatineur, Action, Heure, Origine, Doublon); } }
//    }
//}
