using Packing.Model.Weather;
using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Packing.Services.Weather.Data
{
    public class WeatherDataSeries
    {
        public int? TimePoint { get; set; }
        public int? CloudCover { get; set; }
        public int? Lifted_Index { get; set; }
        public string? Prec_Type { get; set; }
        public int? Prec_Amount { get; set; }
        public int? Temp2m { get; set; }
        public string? Rh2m { get; set; }
        public Wind10m? Wind10m { get; set; }
        public string? Weather { get; set; }

        public Result<DateTime, MessageError> CreateDate(DateTime offset)
            => TimePoint == null ?
            new MessageError("Didn't receive timepoint.") :
            new Result<DateTime, MessageError>(offset.AddHours(TimePoint.Value));

        Result<Clouds, MessageError> CreateCloudCover()
            => CreateIf<int, Clouds>(CloudCover, nameof(CloudCover), c => c switch
            {
                _ when c < 1 => null,
                _ when c == 1 => Clouds.None,
                _ when c <= 3 => Clouds.Light,
                _ when c <= 6 => Clouds.Medium,
                _ when c <= 9 => Clouds.Strong,
                _ => null,
            });

        Result<Wind, MessageError> CreateWind()
            => CreateIfRef<Wind10m, Wind>(Wind10m, "Wind", w => w switch
            {
                { Speed: var s } when s < 1 => null,
                { Speed: var s } when s == 1 => Wind.Calm,
                { Speed: var s } when s <= 3 => Wind.Moderate,
                { Speed: var s } when s <= 5 => Wind.Strong,
                { Speed: var s } when s <= 8 => Wind.Gale,
                _ => null,
            });

        Result<Precipitation, MessageError> CreatePercipitation()
        {
            MessageError? sumary = null;
            var type = CreateIfRef<string, Precipitation.PrecipationType>(Prec_Type, "Precipitaion type", p => p switch
            {
                "snow" => Precipitation.PrecipationType.Snow,
                "rain" => Precipitation.PrecipationType.Rain,
                "frzr" => Precipitation.PrecipationType.Rain,
                "icep" => Precipitation.PrecipationType.Snow,
                "none" => Precipitation.PrecipationType.None,
                _ => null,
            });
            if (!type)
                sumary = type.Err;
            var amount = CreateIf<int, Precipitation.PrecipationAmount>(Prec_Amount, "Precipitation amount", a => a switch
            {
                _ when a < 0 => null,
                _ when a == 1 => Precipitation.PrecipationAmount.None,
                _ when a <= 3 => Precipitation.PrecipationAmount.Light,
                _ when a <= 6 => Precipitation.PrecipationAmount.Medium,
                _ when a <= 9 => Precipitation.PrecipationAmount.Strong,
                _ => null,
            });
            if (!amount)
                sumary?.Chain(amount.Err);
            if (sumary == null)
                return Precipitation.Create(type.Get, amount.Get);
            return sumary;
        }

        Result<U, MessageError> CreateIf<T, U>(T? value, string name, Func<T, U?> map) 
            where T: struct
            where U: struct
        {
            if (value == null)
                return new MessageError($"Didn't receive {name}.");
            var mapped = map(value.Value);
            if (mapped == null)
                return new MessageError($"Value received in {name} is {value.Value} which is not valid.");
            return mapped.Value;
        }

        Result<U, MessageError> CreateIfRef<T, U>(T? value, string name, Func<T, U?> map)
            where T : class
            where U : struct
        {
            if (value == null)
                return new MessageError($"Didn't receive {name}.");
            var mapped = map(value);
            if (mapped == null)
                return new MessageError($"Value received in {name} is {value} which is not valid.");
            return mapped.Value;
        }

        public Result<WeatherSummary, MessageError> IntoWeatherSummary(DateTime offset)
        {
            MessageError summary = new MessageError(string.Empty);
            var date = CreateDate(offset);
            if (!date)
                summary.Chain(date.Err);
            var temperature = Temperature.Create(Temp2m ?? -500);
            if (!temperature)
                summary.Chain(temperature.Err);
            var cloudCover = CreateCloudCover();
            if (!cloudCover)
                summary.Chain(cloudCover.Err);
            var precipitation = CreatePercipitation();
            if (!precipitation)
                summary.Chain(precipitation.Err);
            var clouds = CreateCloudCover();
            if (!clouds)
                summary.Chain(clouds.Err);
            var wind = CreateWind();
            if (!wind)
                summary.Chain(wind.Err);
            if (!string.IsNullOrEmpty(summary.Message))
                return summary;
            return new WeatherSummary(temperature.Get, clouds.Get, wind.Get, precipitation.Get, date.Get);
        }
    }
}
