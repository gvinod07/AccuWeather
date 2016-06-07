using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccuWeather
{
    public class Temperature
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class RealFeelTemperature
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class WetBulbTemperature
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class DewPoint
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class Speed
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class Direction
    {
        public int Degrees { get; set; }
        public string Localized { get; set; }
        public string English { get; set; }
    }

    public class Wind
    {
        public Speed Speed { get; set; }
        public Direction Direction { get; set; }
    }

    public class Speed2
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class WindGust
    {
        public Speed2 Speed { get; set; }
    }

    public class Visibility
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class Ceiling
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class TotalLiquid
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class Rain
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class Snow
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class Ice
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class Forecasts
    {
        public string DateTime { get; set; }

        [SQLite.PrimaryKey]
        public int EpochDateTime { get; set; }
        public int WeatherIcon { get; set; }
        public string IconPhrase { get; set; }
        public bool IsDaylight { get; set; }
        public Temperature Temperature { get; set; }
        public RealFeelTemperature RealFeelTemperature { get; set; }
        public WetBulbTemperature WetBulbTemperature { get; set; }
        public DewPoint DewPoint { get; set; }
        public Wind Wind { get; set; }
        public WindGust WindGust { get; set; }
        public int RelativeHumidity { get; set; }
        public Visibility Visibility { get; set; }
        public Ceiling Ceiling { get; set; }
        public int UVIndex { get; set; }
        public string UVIndexText { get; set; }
        public int PrecipitationProbability { get; set; }
        public int RainProbability { get; set; }
        public int SnowProbability { get; set; }
        public int IceProbability { get; set; }
        public TotalLiquid TotalLiquid { get; set; }
        public Rain Rain { get; set; }
        public Snow Snow { get; set; }
        public Ice Ice { get; set; }
        public int CloudCover { get; set; }
        public string MobileLink { get; set; }
        public string Link { get; set; }
    }

    public class Required
    {
        [SQLite.PrimaryKey]
        public int EpochDate { get; set; }
        public string DateTime { get; set; }
        public double Temp_Value { get; set; }
        public double Wind_Value { get; set; }
    }
}
