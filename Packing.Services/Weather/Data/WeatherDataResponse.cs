using Packing.Model.Weather;
using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Packing.Services.Weather.Data
{
    public class WeatherDataResponse
    {
        public string? Product { get; set; }
        public int Init { get; set; }
        public List<WeatherDataSeries>? DataSeries { get; set; }

        public Result<WeatherDataSeries, MessageError> WeatherForDay(DateTime today, DateTime day)
        {
            var weatherForDay = DataSeries
                .FirstOrDefault(x => x.CreateDate(today)
                    .Then<bool>(d => d.Month == day.Month && d.Day == day.Day && d.Hour > 10 && d.Hour < 18).Get);
            if (weatherForDay == null)
                return new MessageError($"Couldn't find date suitable for {day}");
            return weatherForDay;
        }
    }
}
