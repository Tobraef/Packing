using Moq;
using NUnit.Framework;
using Packing.Model.Location;
using Packing.Model.Weather;
using Packing.Services.Date;
using Packing.Services.Location;
using Packing.Services.Weather;
using Packing.Services.Weather.Data;
using Packing.Shared;
using Packing.Tests.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Packing.Tests.Services.Timer7
{
    [TestFixture]
    class WeatherServiceTests
    {
        readonly Mock<ILatLonService> _latlonService = new Mock<ILatLonService>();
        readonly Mock<IDateTimeService> _dateService = new Mock<IDateTimeService>();
        HttpClientMock httpClient;
        Timer7WeatherService sut;

        readonly DateTime randomDate = new DateTime(2000, 6, 12);

        [SetUp]
        public void Setup()
        {
            httpClient = new HttpClientMock();
            _dateService
                .Setup(x => x.GetTodayDate())
                .Returns(randomDate);
            sut = new Timer7WeatherService(_latlonService.Object, httpClient.Instance, _dateService.Object);
        }

        HttpResponseMessage OkResponseWith(WeatherDataSeries weatherData)
            => new HttpMessagesMock().CreateOkMessage(new WeatherDataResponse
            {
                Init = 1,
                Product = "a",
                DataSeries = new List<WeatherDataSeries> { weatherData }
            });

        [Test]
        public async Task ShouldReturnMappedDataForValidLatLon()
        {
            var city = City.Create("Zgorzelec", Country.Create("Polska").Get).Get;
            _latlonService
                .Setup(x => x.GetGeoLocationFor(It.Is<City>(x => x == city)))
                .Returns(Task.FromResult(new Result<LatLon, MessageError>(new LatLon(15, 30))))
                .Verifiable();
            httpClient
                .SetupGet(x => x.Contains($"lon={30:0.00}&lat={15:0.00}"), OkResponseWith(new WeatherDataSeries
                {
                    TimePoint = 12,
                    CloudCover = 1,
                    Prec_Amount = 1,
                    Prec_Type = "snow",
                    Temp2m = -10,
                    Wind10m = new Wind10m { Speed = 1, Direction = "S" },
                }));

            var weatherResult = await sut.GetWeatherForDay(randomDate, city);

            _latlonService.Verify(x => x.GetGeoLocationFor(city), Times.Once());
            Assert.IsTrue(weatherResult, weatherResult.IsErr ? weatherResult.Err.Message : "");

            var weather = weatherResult.Get;

            Assert.AreEqual(-10, weather.Temperature.Value);
            Assert.AreEqual(Precipitation.PrecipationType.Snow, weather.Precipitation.Type);
        }

    }
}
