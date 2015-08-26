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
        List<City> CitiesList;
        StreamReader reader;
        public MainWindow()
        {
            InitializeComponent();

            CheckInternetConnection();

            reader = new StreamReader("city.list.json");
            CitiesList = new List<City>();

            while (!reader.EndOfStream)
            {
                CitiesList.Add(JsonHelper.JsonDeserialize<City>(reader.ReadLine()));
            }

           // MessageBox.Show(CitiesList.Count.ToString());

            Init();

            ClientSettings.ApiUrl = "http://api.openweathermap.org/data/2.5";
            ClientSettings.ApiKey = "4355fd6b34d30979fb0e05c52642bf31";

            ReadSettingsDataXML();

            if (tbCountryToDisplay.Text != string.Empty | tbCountryToDisplay.Text != "Страна"
                | tbCityToDisplay.Text != string.Empty | tbCityToDisplay.Text != "Город")
            {
                GetCurrentWeatherForCurrentCity(tbCityToDisplay.Text, tbCountryToDisplay.Text);
                btnCurrentCity.Content = tbCityToDisplay.Text;
            }

            
        }

        public void GetCurrentWeatherForCurrentCity(string city, string country)
        {
            try
            {
                var weather = CurrentWeather.GetByCityName(city, country, "ru", "metric");
                lblCurrentTemp.Content = weather.Item.Temp.ToString() + "°C";
                lblDate.Content = weather.Item.Date.ToShortDateString();
                lblDescription.Content = weather.Item.Description.ToString();
                lblHumidity.Content = "Влажность: " + weather.Item.Humidity.ToString() + "%";
                lblLocation.Content = weather.Item.Country.ToString() + ", " + weather.Item.City.ToString();
                lblMinMaxTemp.Content = "Min: " + weather.Item.TempMin.ToString() + "°C" + " Max: " + weather.Item.TempMax.ToString() + "°C";
                lblTime.Content = weather.Item.Date.ToLocalTime().ToShortTimeString();
                lblWind.Content = "Ветер: " + weather.Item.WindSpeed.ToString() + " м/с";

                Uri uri = new Uri("http://openweathermap.org/img/w/" + weather.Item.Icon + ".png");
                BitmapImage bitmap = new BitmapImage(uri);
                imgWeather.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        public void GetCurrentWeatherByName()
        {
            try
            {
                string city, country;
                int comaIndex = tbSearch.Text.IndexOf(',');
                country = tbSearch.Text.Substring(comaIndex + 1);
                city = tbSearch.Text.Remove(comaIndex + 1);
                
                

                var weather = CurrentWeather.GetByCityName(city, country, "ru", "metric");
                lblCurrentTemp.Content = weather.Item.Temp.ToString() + "°C";
                lblDate.Content = weather.Item.Date.ToShortDateString();
                lblDescription.Content = weather.Item.Description.ToString();
                lblHumidity.Content = "Влажность: " + weather.Item.Humidity.ToString() + "%";
                lblLocation.Content = weather.Item.Country.ToString() + ", " + weather.Item.City.ToString();
                lblMinMaxTemp.Content = "Min: " + weather.Item.TempMin.ToString() + "°C" + " Max: " + weather.Item.TempMax.ToString() + "°C";
                lblTime.Content = weather.Item.Date.ToLocalTime().ToShortTimeString();
                lblWind.Content = "Ветер: " + weather.Item.WindSpeed.ToString() + " м/с";

                

                Uri uri = new Uri("http://openweathermap.org/img/w/" + weather.Item.Icon + ".png");
                BitmapImage bitmap = new BitmapImage(uri);
                imgWeather.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                citiesList.Visibility = Visibility.Collapsed;
                if (tbSearch.Text != string.Empty)
                {
                    
                    GetCurrentWeatherByName();
                    tbSearch.Clear();
                }
                else
                    MessageBox.Show("Введите город");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            settingsFlyout.IsOpen = true;
            
            settingsFlyout.Focus();
            

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            settingsFlyout.IsOpen = false;
            
            btnCurrentCity.Content = tbCityToDisplay.Text;
            GetCurrentWeatherForCurrentCity(tbCityToDisplay.Text, tbCountryToDisplay.Text);

            string city, country;
            int comaIndex = lblLocation.Content.ToString().IndexOf(',');
            city = lblLocation.Content.ToString().Substring(comaIndex + 1);
            country = lblLocation.Content.ToString().Remove(comaIndex);
            tbCityToDisplay.Text = city.Replace(" ", "");
            tbCountryToDisplay.Text = country;
            btnCurrentCity.Content = city.Replace(" ", "");

            WriteSettingsDataXML();
        }

        private void tbCityToDisplay_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbCityToDisplay.Text == "Город")
                tbCityToDisplay.Clear();
        }

        private void tbCountryToDisplay_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbCountryToDisplay.Text == "Страна")
                tbCountryToDisplay.Clear();
        }

        private void tbCityToDisplay_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbCityToDisplay.Text == string.Empty)
                tbCityToDisplay.Text = "Город";
        }

        private void tbCountryToDisplay_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbCountryToDisplay.Text == string.Empty)
                tbCountryToDisplay.Text = "Страна";
        }

        public void WriteSettingsDataXML()
        {

            try
            {
                if(tbCityToDisplay.Text != "Город" | tbCountryToDisplay.Text != "Страна" )
                {
                    var path = "userSettings.xml";

                    XDocument doc = new XDocument();
                    XElement settings = new XElement("settings");
                    doc.Add(settings);

                    XElement location = new XElement("location");

                    XElement notification = new XElement("notification");
                    XElement notificationIsEnable = new XElement("isenabled");
                    if (cbNotification.IsChecked == true)
                    {
                        notificationIsEnable.Value = "true";

                        XElement tempDifference = new XElement("temperature");
                        tempDifference.Value = numericTemperature.Value.ToString();

                        XElement windDifference = new XElement("wind");
                        windDifference.Value = numericWind.Value.ToString();

                        XElement humidityDifference = new XElement("humidity");
                        humidityDifference.Value = numericHumidity.Value.ToString();

                        XElement email = new XElement("email");
                        email.Value = tbEmail.Text;


                        notification.Add(email, tempDifference, windDifference, humidityDifference);
                    }
                        
                    else
                        notificationIsEnable.Value = "false";


                    XElement city = new XElement("city");
                    city.Value = tbCityToDisplay.Text;

                    XElement country = new XElement("country");
                    country.Value = tbCountryToDisplay.Text;

                    

                    notification.Add(notificationIsEnable);

                    location.Add(city);
                    location.Add(country);

                    doc.Root.Add(location);
                    
                    doc.Root.Add(notification);
                    doc.Save(path);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        public void ReadSettingsDataXML()
        {
            try
            {
                var path = "userSettings.xml";
                XDocument doc = XDocument.Load(path);

                var country = doc.Element("settings").Element("location").Element("country").Value;
                tbCountryToDisplay.Clear();
                tbCountryToDisplay.Text = country;

                var city = doc.Element("settings").Element("location").Element("city").Value;
                tbCityToDisplay.Clear();
                tbCityToDisplay.Text = city;

                

                var notificationIsEnabled = doc.Element("settings").Element("notification").Element("isenabled").Value;
                if (notificationIsEnabled == "true")
                {
                    cbNotification.IsChecked = true;
                    tbEmail.IsEnabled = true;
                    var email = doc.Element("settings").Element("notification").Element("email").Value;
                    tbEmail.Clear();
                    tbEmail.Text = email;

                    var temperatureDifference = doc.Element("settings").Element("notification").Element("temperature").Value;
                    numericTemperature.Value = Convert.ToDouble(temperatureDifference);

                    var humidityDifference = doc.Element("settings").Element("notification").Element("humidity").Value;
                    numericHumidity.Value = Convert.ToDouble(humidityDifference);

                    var windDifference = doc.Element("settings").Element("notification").Element("wind").Value;
                    numericWind.Value = Convert.ToDouble(windDifference);

                }
                else
                    cbNotification.IsChecked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tbEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text == "E-mail")
                tbEmail.Clear();
        }

        private void tbEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text == string.Empty)
                tbEmail.Text = "E-mail";
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string city, country;
                int comaIndex = lblLocation.Content.ToString().IndexOf(',');
                city = lblLocation.Content.ToString().Substring(comaIndex + 1);
                country = lblLocation.Content.ToString().Remove(comaIndex);
                GetCurrentWeatherForCurrentCity(city.Replace(" ", ""), country);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
        }

        private void btnCurrentCity_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentWeatherForCurrentCity(tbCityToDisplay.Text, tbCountryToDisplay.Text);
        }

        public void SendWeatherNotify()
        {
            try
            {
                client = new SmtpClient();
                client.Port = 465;
                client.Host = "smtp.gmail.com";
                credentials = new NetworkCredential("weathertesapp123@gmail.com", "weather123");
                client.Credentials = credentials;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                from = new MailAddress("weathertestapp123@gmail.com", "Weather Notification");
                to = new MailAddress(tbEmail.Text);
                message = new MailMessage(from, to);
                message.Subject = "Current weather";
                message.Body = String.Format("Current location: {0} Time: {1} Temperature: {2}", lblLocation.Content.ToString(), lblTime.ToString(), lblCurrentTemp.Content.ToString());
                client.Send(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        //private void btnTest_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        SendWeatherNotify();
        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //    }
            
        //}

        private void cbNotification_IsCheckedChanged(object sender, EventArgs e)
        {
            if(cbNotification.IsChecked == true)
            {
                tbEmail.IsEnabled = true;
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        void Init()
        {
            tbSearch.TextChanged += (sender, e) =>
            {
                citiesList.Visibility = Visibility.Visible;
                citiesList.Items.Clear();
                //if(citiesList.Items.Count < 15)
                //    CitiesList.FindAll(item => item.Name.Contains(tbSearch.Text)).ForEach(city => citiesList.Items.Add(string.Format("{0}, {1}", city.Name, city.Country)));
                
                //foreach (var city in CitiesList.FindAll(item => item.Name.Contains(tbSearch.Text)))
                //{
                //    if (citiesList.Items.Count < 55)
                //        citiesList.Items.Add(string.Format("{0}, {1}", city.Name, city.Country));
                //}

                

                foreach (var city in CitiesList)
                {
                    if(city.Name.StartsWith(tbSearch.Text) || city.Name.Contains(tbSearch.Text))
                    {
                        if(citiesList.Items.Count < 5)
                            citiesList.Items.Add(string.Format("{0}, {1}", city.Name, city.Country));
                    }
                }



                if (citiesList.Items.Count == 0) citiesList.Visibility = Visibility.Collapsed;

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

        private void citiesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string location = citiesList.SelectedItem.ToString();

                string city, country;
                int comaIndex = location.IndexOf(',');
                country = location.Substring(comaIndex + 1);
                city = location.Remove(comaIndex + 1);

                City cityFound = CitiesList.Single<City>(c => c.Name == city | c.Country == country);

                GetWeatherForCityId(cityFound.Id);

                citiesList.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }

        void GetWeatherForCityId(int id)
        {
            var path = "http://api.openweathermap.org/data/2.5/weather?id=" + id.ToString() + "&APPID=4355fd6b34d30979fb0e05c52642bf31" + "&mode=xml" + "&units=metric" + "&lang=ru";
            XDocument doc = XDocument.Load(path);

            var city = doc.Element("current").Element("city").Attribute("name").Value;
            var country = doc.Element("current").Element("city").Element("country").Value;
            lblLocation.Content = string.Format("{0}, {1}", city, country);

            var sunrise = doc.Element("current").Element("city").Element("sun").Attribute("rise").Value;
            var sunset = doc.Element("current").Element("city").Element("sun").Attribute("set").Value;
            lblSunsetSunrise.Content = string.Format("Рассвет: {0} Закат: {0}", 
                Convert.ToDateTime(sunrise).ToLocalTime(),
                Convert.ToDateTime(sunset).ToLocalTime());

            var temperature = doc.Element("current").Element("temperature").Attribute("value").Value;
            lblCurrentTemp.Content = temperature + "°C";

            lblMinMaxTemp.Content = string.Format("Min: {0}°C  Max: {1}°C",
                doc.Element("current").Element("temperature").Attribute("min").Value,
                doc.Element("current").Element("temperature").Attribute("max").Value);

            lblHumidity.Content = string.Format("Влажность: {0}{1}",
                doc.Element("current").Element("humidity").Attribute("value").Value,
                doc.Element("current").Element("humidity").Attribute("unit").Value
                );

            lblPressure.Content = string.Format("Давление: {0}{1}",
                doc.Element("current").Element("pressure").Attribute("value").Value,
                doc.Element("current").Element("pressure").Attribute("unit").Value
                );

            lblWind.Content = string.Format("Ветер: {0} м/с, {1}, {2}",
                doc.Element("current").Element("wind").Element("speed").Attribute("value").Value,
                doc.Element("current").Element("wind").Element("direction").Attribute("value".ToLower()).Value,
                doc.Element("current").Element("wind").Element("speed").Attribute("name".ToLower()).Value
                );

            lblDescription.Content = doc.Element("current").Element("clouds").Attribute("name").Value;


            Uri uri = new Uri("http://openweathermap.org/img/w/" 
                + doc.Element("current").Element("weather").Attribute("icon").Value
                + ".png");
            BitmapImage bitmap = new BitmapImage(uri);
            imgWeather.Source = bitmap;

            lblTime.Content = Convert.ToDateTime(doc.Element("current").Element("lastupdate").Attribute("value").Value).ToLocalTime();
            lblDate.Content = Convert.ToDateTime(doc.Element("current").Element("lastupdate").Attribute("value").Value).ToShortDateString();



        }

        private void citiesList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           string location = citiesList.SelectedItem.ToString();

            string city, country;
            int comaIndex = location.IndexOf(',');
            country = location.Substring(comaIndex + 1);
            city = location.Remove(comaIndex + 1);

            City cityFound = CitiesList.First<City>(c => c.Name == city | c.Country == country);

            GetWeatherForCityId(cityFound.Id);

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
            catch (WebException ex)
            {
                MessageBox.Show("Подключение к интернету отсутсвует");
            }
        }
    }
}
