using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    class JsonHelper
    {
        public static string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return jsonString;
        }

        //public static City JsonDeserialize(string jsonString)
        //{
        //    //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
        //    //MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
        //    //T obj = (T)ser.ReadObject(ms);
        //    //return obj;

        //    //DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(City));
        //    //string fileContent = System.IO.File.ReadLines(jsonString);
        //   // City city = (City)json.ReadObject(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(fileContent)));

        //    //return city;
        //}
    }
}
