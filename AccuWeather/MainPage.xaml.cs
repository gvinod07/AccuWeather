using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Popups;
using System.Collections.ObjectModel;
using Windows.Phone.UI.Input;
using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

// API key : VGGxNAsElgGyskitNGuw4aSxz2hUcWl5 

namespace AccuWeather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public ObservableCollection<Required> Forecast = new ObservableCollection<Required>();

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you
            base.OnNavigatedTo(e);

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;



            //Database Loading starts here
            DatabaseHelperClass db = new DatabaseHelperClass();
            Forecast = db.ReadForecasts();

           

            loading.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            loading1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            foreach(Required f in Forecast)
            {
                string s = f.DateTime;
                f.DateTime = "Time : " + s.Substring(11,8);
            }

            ListV.ItemsSource = Forecast;
            ListV1.ItemsSource = Forecast;

            var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text01);
            var TileAttrib = tileXml.GetElementsByTagName("text");
            TileAttrib[0].AppendChild(tileXml.CreateTextNode("Stay Tuned.."));
            TileAttrib[1].AppendChild(tileXml.CreateTextNode(Forecast[0].DateTime));
            TileAttrib[2].AppendChild(tileXml.CreateTextNode(Forecast[0].Temp_Value.ToString() + " °F"));
            TileAttrib[3].AppendChild(tileXml.CreateTextNode(Forecast[0].Temp_Value.ToString() + " mi/h"));

            var d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour + 1, DateTime.Now.Minute, DateTime.Now.Second);

            var tileNotification = new Windows.UI.Notifications.TileNotification(tileXml);
            tileNotification.ExpirationTime = d;

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.CanGoBack)
                     {
                         e.Handled = true;
                         Frame.GoBack();
                     }

        }

        public async Task<Tuple<string, string>> GetLocAsync()
        {

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string lat, lon;

            if (localSettings.Values.ContainsKey("LocationConsent") != true)
            {
                // User has opted in or out of Location
                 return Tuple.Create("", "");
            }

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                lat = geoposition.Coordinate.Point.Position.Latitude.ToString("0.00");
                 
                lon = geoposition.Coordinate.Point.Position.Longitude.ToString("0.00");

                return Tuple.Create(lat, lon);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return Tuple.Create("", "");
            }
        }

        public async Task<string> GetLocKeyAsync(string lat, string lon)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync("http://dataservice.accuweather.com/locations/v1/cities/geoposition/search?apikey=VGGxNAsElgGyskitNGuw4aSxz2hUcWl5&q=" + lat + "%2C" + lon);

                    JsonObject obj = JsonObject.Parse(response);
                    string loc = obj["Key"].GetString();

                    return loc;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<List<Forecasts>> GetForecastAsync(string key)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    var response = await client.GetStringAsync("http://dataservice.accuweather.com/forecasts/v1/hourly/12hour/" + key + "?apikey=VGGxNAsElgGyskitNGuw4aSxz2hUcWl5&details=true");
                    List<Forecasts> obj = new List<Forecasts>();
                    obj = JsonConvert.DeserializeObject<List<Forecasts>>(response);
                    return obj;

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        private async void RefreshPage(object sender, RoutedEventArgs e)
        {

            bool isConnected = NetworkInterface.GetIsNetworkAvailable();
            ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
                NetworkConnectivityLevel connection = InternetConnectionProfile.GetNetworkConnectivityLevel();
                if (connection == NetworkConnectivityLevel.None || connection == NetworkConnectivityLevel.LocalAccess)
                {
                    isConnected = false;
                }


            if (!isConnected)
            {
                await new MessageDialog("No internet connection is avaliable. Please check your connection and try again").ShowAsync();
            }
            else
            {

                DatabaseHelperClass db = new DatabaseHelperClass();

                loading.Visibility = Windows.UI.Xaml.Visibility.Visible;
                loading1.Visibility = Windows.UI.Xaml.Visibility.Visible;


                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.ContainsKey("LocationConsent"))
                {
                    // User has opted in or out of Location

                }
                else
                {
                    var dialog = new MessageDialog("We need your current location.");
                    dialog.Title = "Please";
                    dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                    dialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });
                    var res = await dialog.ShowAsync();


                    if ((int)res.Id == 0)
                    {
                        localSettings.Values["LocationConsent"] = true;
                    }
                    else
                    {
                        localSettings.Values["LocationConsent"] = false;
                    }

                }

                Tuple<string, string> pos = await GetLocAsync();

                string loc_key = await GetLocKeyAsync(pos.Item1, pos.Item2);

                List<Forecasts> obj = new List<Forecasts>();
                obj = await GetForecastAsync(loc_key);

                foreach (Forecasts f in obj)
                {
                    Required r = new Required();
                    r.DateTime = f.DateTime;
                    r.EpochDate = f.EpochDateTime;
                    r.Temp_Value = f.Temperature.Value;
                    r.Wind_Value = f.Wind.Speed.Value;

                    db.Insert(r);
                }

                Forecast.Clear();
                Forecast = db.ReadForecasts();

                foreach (Required f in Forecast)
                {
                    string s = f.DateTime;
                    f.DateTime = "Time : " + s.Substring(11, 8);
                }

                loading.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                loading1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                ListV.ItemsSource = Forecast;
                ListV1.ItemsSource = Forecast;
            }
        }

        private void aboutOpen(object sender, RoutedEventArgs e)
        {
            
            Frame.Navigate(typeof(BlankPage1)); 
        }

        private void PrimaryTile(object sender, RoutedEventArgs e)
        {
            

        }
    }
}
