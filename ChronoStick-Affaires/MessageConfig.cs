using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChronoStick_Affaires
{
    [DataContract]
    public class MessageConfig
    {
        [DataMember]
        public TypeMessageConfig TypeMessage { get; set; }

        [DataMember]
        public int ValeurMessage { get; set; }
     }
}
