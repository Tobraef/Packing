using System;
using System.Collections.Generic;
using System.Text;

namespace Packing.Model.Weather
{
    public class WeatherSummary
    {
        public DateTime Date { get; }

        public Temperature Temperature { get; }

        public Clouds Clouds { get; }

        public Wind Wind { get; }

        public Precipitation Precipitation { get; }

        public WeatherSummary(Temperature temperature, Clouds clouds, Wind wind, Precipitation percipitation, DateTime date)
        {
            Temperature = temperature;
            Clouds = clouds;
            Wind = wind;
            Precipitation = percipitation;
            Date = date;
        }
    }
}
