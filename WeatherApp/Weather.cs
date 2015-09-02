using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace WeatherApp
{
     class  Weather
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Temperature { get; set; }
        public string MaxTemperature { get; set; }
        public string MinTemperature { get; set; }
        public string Pressure { get; set; }
        public string Humidity { get; set; }
        public string WindSpeed { get; set; }

        public string Sky { get; set; }
        public DateTime Sunset { get; set; }
        public DateTime Sunrise { get; set; }



 
        public void GetWeatherById(int id)
        {

                var path = "http://api.openweathermap.org/data/2.5/weather?id=" + id.ToString() + "&APPID=4355fd6b34d30979fb0e05c52642bf31" + "&mode=xml" + "&units=metric" + "&lang=ru";
                XDocument doc = XDocument.Load(path);


                City = doc.Element("current").Element("city").Attribute("name").Value;

                Country = doc.Element("current").Element("city").Element("country").Value;

                var sunrise = doc.Element("current").Element("city").Element("sun").Attribute("rise").Value;
                var sunset = doc.Element("current").Element("city").Element("sun").Attribute("set").Value;

                Sunrise = Convert.ToDateTime(sunrise).ToLocalTime();
                Sunset = Convert.ToDateTime(sunset).ToLocalTime();

                Temperature = doc.Element("current").Element("temperature").Attribute("value").Value;

                MaxTemperature = doc.Element("current").Element("temperature").Attribute("max").Value;

                MinTemperature = doc.Element("current").Element("temperature").Attribute("min").Value;

                Humidity = doc.Element("current").Element("humidity").Attribute("value").Value + doc.Element("current").Element("humidity").Attribute("unit").Value;

                Pressure = doc.Element("current").Element("pressure").Attribute("value").Value + doc.Element("current").Element("pressure").Attribute("unit").Value;

                WindSpeed = doc.Element("current").Element("wind").Element("speed").Attribute("value").Value;

                Sky = doc.Element("current").Element("weather").Attribute("value").Value;
            

           
        }
    }
}
