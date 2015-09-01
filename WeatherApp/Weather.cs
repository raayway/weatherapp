using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    class Weather
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Temperature { get; set; }
        public string MaxTemperature { get; set; }
        public string MinTemperature { get; set; }
        public string Pressure { get; set; }
        public string WindSpeed { get; set; }
        public string WindDescription { get; set; }
        public string WindDirection { get; set; }
        public string Sky { get; set; }
        public string LastUpdTime { get; set; }
        public string LastUpdDate { get; set; }
    }
}
