using Packing.Model.Location;
using Packing.Model.Weather;
using Packing.Services.Date;
using Packing.Services.Location;
using Packing.Services.Weather.Data;
using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Packing.Services.Weather
{
    public class Timer7WeatherService : IWeatherService
    {
        readonly ILatLonService _latlonService;
        readonly IDateTimeService _dateService;
        readonly HttpClient _httpClient;

        public Timer7WeatherService(ILatLonService latlonService, HttpClient httpClient, IDateTimeService dateService)
        {
            _latlonService = latlonService;
            _httpClient = httpClient;
            _dateService = dateService;
        }

        string WeatherDataUrl(LatLon latLon)
            => @$"http://www.7timer.info/bin/api.pl?lon={latLon.Longtitude:0.00}&lat={latLon.Latitude:0.00}&product=civil&output=json";

        bool DateWithinRange(DateTime day)
        {
            var today = _dateService.GetTodayDate();
            var daysDifference = day - today;
            return daysDifference.TotalDays < 7;
        }

        public async Task<Result<WeatherSummary, MessageError>> GetWeatherForDay(DateTime day, City location)
        {
            if (!DateWithinRange(day))
                return new MessageError("Date not within service range.");
            var latLonResult = await _latlonService.GetGeoLocationFor(location);
            if (latLonResult)
            {
                var latLon = latLonResult.Get;
                var response = await _httpClient.GetAsync(WeatherDataUrl(latLon));
                var weatherRaw = await new RequestToJsonParser()
                    .RequestToJsonSerializableType<WeatherDataResponse>(response);
                if (weatherRaw)
                {
                    var today = _dateService.GetTodayDate();
                    return weatherRaw.Get
                        .WeatherForDay(today, day)
                        .Then(w => w.IntoWeatherSummary(today));
                }
                return weatherRaw.MapErr<WeatherSummary>();
            }
            return latLonResult.MapErr<WeatherSummary>();
        }
    }
}
