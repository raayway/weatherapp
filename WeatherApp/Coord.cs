using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    [DataContract]
    class Coord
    {
        [DataMember]
        public float lon { get; set; }
        [DataMember]
        public float lat { get; set; }
    }
}
