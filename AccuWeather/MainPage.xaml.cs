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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

// API key : VGGxNAsElgGyskitNGuw4aSxz2hUcWl5 

namespace AccuWeather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public ObservableCollection<Forecasts> Forecast = new ObservableCollection<Forecasts>();

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
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you
            base.OnNavigatedTo(e);

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

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

            loading.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            loading1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            foreach(Forecasts f in obj)
            {
                if(f.Temperature.Unit == "F" || f.Temperature.Unit == "f" )
                {
                    f.Temperature.Unit = "C";
                    f.Temperature.Value = ((f.Temperature.Value - 32)* 5)/ 9;
                }
                string s = f.DateTime;
                f.DateTime = "Time : " + s.Substring(11,8);
                Forecast.Add(f);
            }

            ListV.ItemsSource = Forecast;
            ListV1.ItemsSource = Forecast;
       
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

        private void RefreshPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }


        private void RefreshPages(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void aboutOpen(object sender, RoutedEventArgs e)
        {
            
            Frame.Navigate(typeof(BlankPage1)); 
        }
    }
}
