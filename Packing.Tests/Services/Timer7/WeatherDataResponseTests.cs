using NUnit.Framework;
using Packing.Services.Weather.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Packing.Tests.Services.Timer7
{
    [TestFixture]
    class WeatherDataResponseTests
    {
        WeatherDataResponse sut;

        [Test]
        public void ShouldReturnRecordThatIsBetween10And18HourWhenAskingForSpecificDay()
        {
            var dateFrom = new DateTime(2000, 6, 15, 15, 0, 0);
            var dateFor = new DateTime(2000, 6, 17);
            var hoursDif = (int)Math.Round((dateFor - dateFrom).TotalHours) + 9;
            var tooEarly = new WeatherDataSeries { TimePoint = hoursDif };
            var mid = new WeatherDataSeries { TimePoint = hoursDif + 3 };
            var tooLate = new WeatherDataSeries { TimePoint = hoursDif + 10 };
            sut = new WeatherDataResponse
            {
                DataSeries = new List<WeatherDataSeries> { tooEarly, mid, tooLate }
            };

            var chosen = sut.WeatherForDay(dateFrom, dateFor);

            Assert.IsTrue(chosen);
            Assert.AreEqual(chosen.Get, mid);
        } 
    }
}
