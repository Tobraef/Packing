using NUnit.Framework;
using Packing.Model.Weather;
using Packing.Services.Weather.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Packing.Tests.Services.Timer7
{
    [TestFixture]
    class WeatherDataSeriesTests
    {
        WeatherDataSeries sut;

        readonly DateTime randomDate = new DateTime(2000, 6, 15);

        [Test]
        public void ShouldReturnHoursOffsetDateForSuppliedTimePoint()
        {
            sut = new WeatherDataSeries { TimePoint = 5 };

            var date = sut.CreateDate(randomDate);

            Assert.IsTrue(date);
            Assert.AreEqual(date.Get.Day, 15);

            sut.TimePoint = 25;

            date = sut.CreateDate(randomDate);

            Assert.IsTrue(date);
            Assert.AreEqual(date.Get.Day, 16);
        }

        [Test]
        public void ShouldReturnErrorOnNoTimePoint()
        {
            sut = new WeatherDataSeries();

            var date = sut.CreateDate(randomDate);

            Assert.IsFalse(date);
        }

        [Test]
        public void ShouldReturnMappedValueOnCorrectInput()
        {
            sut = new WeatherDataSeries
            {
                TimePoint = 24,
                Temp2m = 20,
                CloudCover = 5,
                Prec_Amount = 5,
                Prec_Type = "snow",
                Wind10m = new Wind10m { Speed = 5 },
            };

            var mappedResult = sut.IntoWeatherSummary(randomDate);

            Assert.IsTrue(mappedResult);

            var mapped = mappedResult.Get;

            Assert.AreEqual(mapped.Date, randomDate.AddHours(24));
            Assert.AreEqual(mapped.Temperature.Value, 20);
            Assert.AreEqual(mapped.Clouds, Clouds.Medium);
            Assert.AreEqual(mapped.Precipitation.Type, Precipitation.PrecipationType.Snow);
            Assert.AreEqual(mapped.Precipitation.Amount, Precipitation.PrecipationAmount.Medium);
            Assert.AreEqual(mapped.Wind, Wind.Strong);
        }

        [Test]
        public void ShouldReturnErrorWhenAnyDataIsInvalid()
        {
            sut = new WeatherDataSeries
            {
                TimePoint = 24,
                Temp2m = null,
                CloudCover = 5,
                Prec_Amount = 5,
                Prec_Type = "snow",
                Wind10m = new Wind10m { Speed = 5 },
            };

            var mappedResult = sut.IntoWeatherSummary(randomDate);

            Assert.IsFalse(mappedResult);
        }
    }
}
