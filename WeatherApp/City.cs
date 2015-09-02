using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
        [DataContract]
        class City
        {
            [DataMember]
            public int _id { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public string country { get; set; }
            [DataMember]
            public Coord coord { get; set; }

    
    }
}
