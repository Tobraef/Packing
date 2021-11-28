using Packing.Model.Location;
using Packing.Model.Weather;
using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Packing.Services.Weather
{
    public interface IWeatherService
    {
        Task<Result<WeatherSummary, MessageError>> GetWeatherForDay(DateTime day, City location);
    }
}
