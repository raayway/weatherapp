using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace WeatherApp
{
    [DataContract]
    class Coord
    {
        [DataMember(Name = "lon")]
        public float Longitude { get; set; }
        [DataMember(Name = "lat")]
        public float Latitude { get; set; }
    }
}
