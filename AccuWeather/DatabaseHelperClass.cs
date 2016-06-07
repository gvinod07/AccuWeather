using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccuWeather
{
    class DatabaseHelperClass
    {
        SQLiteConnection dbConn;

        //Create Tabble 
        public async Task<bool> onCreate(string DB_PATH)
        {
            try
            {
                bool x = await CheckFileExists(DB_PATH);
                if (!x)
                {
                    using (dbConn = new SQLiteConnection(DB_PATH))
                    {
                        dbConn.CreateTable<Required>();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Insert(Required newforecast)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Insert(newforecast);
                });
            }
        }

        public ObservableCollection<Required> ReadForecasts()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Required> myCollection = dbConn.Table<Required>().ToList<Required>();
                ObservableCollection<Required> ForecastsList = new ObservableCollection<Required>(myCollection);
                return ForecastsList;
            }
        }



    }
}
