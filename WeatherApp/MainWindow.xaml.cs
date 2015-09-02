using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeatherNet;
using WeatherNet.Clients;
using MahApps.Metro.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using GeoDataSource;
namespace WeatherApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public SmtpClient client;
        public MailMessage message;
        public MailAddress from;
        public MailAddress to;
        public NetworkCredential credentials;
        List<City> CityList;
        Weather weather;
        StreamReader reader;
        public MainWindow()
        {
            InitializeComponent();

            CheckInternetConnection();

            

           // MessageBox.Show(CitiesList.Count.ToString());


            //eadSettingsDataXML();

           Init();


        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            citiesList.Items.Clear();
            try
            {

                if (tbSearch.Text != string.Empty)
                {
                    

                    try
                    {
                        

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
                else
                    MessageBox.Show("Введите город");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }



        //public void WriteSettingsDataXML()
        //{

        //    try
        //    {
        //        if(tbCityToDisplay.Text != "Город" | tbCountryToDisplay.Text != "Страна" )
        //        {
        //            var path = "userSettings.xml";

        //            XDocument doc = new XDocument();
        //            XElement settings = new XElement("settings");
        //            doc.Add(settings);

        //            XElement location = new XElement("location");

        //            XElement notification = new XElement("notification");
        //            XElement notificationIsEnable = new XElement("isenabled");
        //            if (cbNotification.IsChecked == true)
        //            {
        //                notificationIsEnable.Value = "true";

        //                XElement tempDifference = new XElement("temperature");
        //                tempDifference.Value = numericTemperature.Value.ToString();

        //                XElement windDifference = new XElement("wind");
        //                windDifference.Value = numericWind.Value.ToString();

        //                XElement humidityDifference = new XElement("humidity");
        //                humidityDifference.Value = numericHumidity.Value.ToString();

        //                XElement email = new XElement("email");
        //                email.Value = tbEmail.Text;


        //                notification.Add(email, tempDifference, windDifference, humidityDifference);
        //            }
                        
        //            else
        //                notificationIsEnable.Value = "false";


        //            XElement city = new XElement("city");
        //            city.Value = tbCityToDisplay.Text;

        //            XElement country = new XElement("country");
        //            country.Value = tbCountryToDisplay.Text;

                    

        //            notification.Add(notificationIsEnable);

        //            location.Add(city);
        //            location.Add(country);

        //            doc.Root.Add(location);
                    
        //            doc.Root.Add(notification);
        //            doc.Save(path);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //public void ReadSettingsDataXML()
        //{
        //    try
        //    {
        //        var path = "userSettings.xml";
        //        XDocument doc = XDocument.Load(path);

        //        var country = doc.Element("settings").Element("location").Element("country").Value;
        //        tbCountryToDisplay.Clear();
        //        tbCountryToDisplay.Text = country;

        //        var city = doc.Element("settings").Element("location").Element("city").Value;
        //        tbCityToDisplay.Clear();
        //        tbCityToDisplay.Text = city;

                

        //        var notificationIsEnabled = doc.Element("settings").Element("notification").Element("isenabled").Value;
        //        if (notificationIsEnabled == "true")
        //        {
        //            cbNotification.IsChecked = true;
        //            tbEmail.IsEnabled = true;
        //            var email = doc.Element("settings").Element("notification").Element("email").Value;
        //            tbEmail.Clear();
        //            tbEmail.Text = email;

        //            var temperatureDifference = doc.Element("settings").Element("notification").Element("temperature").Value;
        //            numericTemperature.Value = Convert.ToDouble(temperatureDifference);

        //            var humidityDifference = doc.Element("settings").Element("notification").Element("humidity").Value;
        //            numericHumidity.Value = Convert.ToDouble(humidityDifference);

        //            var windDifference = doc.Element("settings").Element("notification").Element("wind").Value;
        //            numericWind.Value = Convert.ToDouble(windDifference);

        //        }
        //        else
        //            cbNotification.IsChecked = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private void btnUpdate_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        string city, country;
        //        int comaIndex = lblLocation.Content.ToString().IndexOf(',');
        //        city = lblLocation.Content.ToString().Substring(comaIndex + 1);
        //        country = lblLocation.Content.ToString().Remove(comaIndex);
        //        //GetCurrentWeatherForCurrentCity(city.Replace(" ", ""), country);
        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //    }
           
        //}

        private void btnCurrentCity_Click(object sender, RoutedEventArgs e)
        {
            //GetCurrentWeatherForCurrentCity(tbCityToDisplay.Text, tbCountryToDisplay.Text);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        void Init()
        {
            try
            {
                
                tbSearch.TextChanged += (sender, e) =>
                {
                    
                    citiesList.Visibility = Visibility.Visible;
                    citiesList.Items.Clear();
                    if (tbSearch.Text.Count<Char>() >= 3)
                    {
                        citiesList.Items.Clear();
                        CityList.FindAll(item => item.name.IndexOf(tbSearch.Text, StringComparison.InvariantCultureIgnoreCase) >= 0).ForEach(city => citiesList.Items.Add(city.name + ", " + city.country));
                    }
                    if (citiesList.Items.Count == 0)
                        citiesList.Visibility = Visibility.Collapsed;

                };

                tbSearch.LostFocus += (sender, e) =>
                {
                    citiesList.Visibility = Visibility.Collapsed;
                };

                tbSearch.GotFocus += (sender, e) =>
                {
                    if (tbSearch.Text == string.Empty)
                        citiesList.Visibility = Visibility.Collapsed;
                };

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
        private void citiesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
                citiesList.Visibility = Visibility.Collapsed;
                string location = citiesList.SelectedValue.ToString();


                weather = new Weather();

                string city, country;
                int comaIndex = location.IndexOf(',');
                country = location.Substring(comaIndex + 1);
                city = location.Remove(comaIndex);


                City cityFound = CityList.FirstOrDefault<City>(c => c.name == city | c.country == country);

                weather.GetWeatherById(cityFound._id);

                lblLocation.Text = weather.City + ", " + weather.Country;
                lblMinMaxTemp.Content = weather.MinTemperature + "/" + weather.MaxTemperature;
                lblHumidity.Content = weather.Humidity;
                lblDescription.Content = weather.Sky;
                lblPressure.Content = weather.Pressure;
                lblSunsetSunrise.Content = "Рассвет: " + weather.Sunrise + " Закат: " + weather.Sunset;
                lblWind.Content = "Ветер: " + weather.WindSpeed;
                lblCurrentTemp.Content = weather.Temperature;
                btnMap.Visibility = Visibility.Visible;

            
        }

        void ShowMap(string location)
        {
            string city, country;
            int comaIndex = location.IndexOf(',');
            country = location.Substring(comaIndex + 1);
            city = location.Remove(comaIndex);

            City cityFound = CityList.FirstOrDefault<City>(c => c.name == city | c.country == country);

            var lat = Convert.ToInt32(cityFound.coord.lat);
            var lon = Convert.ToInt32(cityFound.coord.lon);

            browser.Navigate("http://openweathermap.org/Maps?zoom=12&lat=" + lat.ToString() + "&lon=" + lon.ToString() + "&layers=B0FTTFF");
        }


        private void citiesList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           string location = citiesList.SelectedItem.ToString();

            string city, country;
            int comaIndex = location.IndexOf(',');
            country = location.Substring(comaIndex + 1);
            city = location.Remove(comaIndex + 1);

            //City cityFound = CitiesList.First<City>(c => c.Name == city | c.Country == country);

            //GetWeatherForCityId(cityFound.Id);

            citiesList.Visibility = Visibility.Collapsed;
        }

        private void citiesList_GotFocus(object sender, RoutedEventArgs e)
        {
            citiesList.Visibility = Visibility.Visible;
        }

        private void citiesList_LostFocus(object sender, RoutedEventArgs e)
        {
            citiesList.Visibility = Visibility.Collapsed;
        }

        void CheckInternetConnection()
        {
            try
            {
                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("http://google.com");
                HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
            }
            catch (WebException)
            {
                MessageBox.Show("Подключение к интернету отсутсвует");
            }
        }


     
        

        private void tbSearch_TextInput(object sender, TextCompositionEventArgs e)
        {
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            reader = new StreamReader("city.list.json");
            CityList = new List<City>();
            while (!reader.EndOfStream)
            {
                CityList.Add(JsonHelper.JsonDeserialize<City>(reader.ReadLine()));
            }
        }

        private void btnMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowMap(lblLocation.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
