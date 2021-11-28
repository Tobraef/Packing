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
        readonly City randomCity = City.Create("Zgorzelec", Country.Create("Poland").Get).Get;
        const int TEMPERATURE = -10;
        const Precipitation.PrecipationType PREC_TYPE = Precipitation.PrecipationType.Snow;

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
                Init = "1",
                Product = "a",
                DataSeries = new List<WeatherDataSeries> { weatherData }
            });

        void SetupWebMessages(bool okLatLon, bool okResponse)
        {
            _latlonService
                .Setup(x => x.GetGeoLocationFor(It.Is<City>(x => x == randomCity)))
                .Returns(Task.FromResult(okLatLon ?
                    new Result<LatLon, MessageError>(new LatLon(15, 30)) :
                    new Result<LatLon, MessageError>(new MessageError("Bad request"))))
                .Verifiable();
            httpClient
                .SetupGet(x => x.Contains($"lon={30:0.00}&lat={15:0.00}"), okResponse ?
                OkResponseWith(new WeatherDataSeries
                {
                    TimePoint = 12,
                    CloudCover = 1,
                    Prec_Amount = 1,
                    Prec_Type = "snow",
                    Temp2m = -10,
                    Wind10m = new Wind10m { Speed = 1, Direction = "S" },
                }) :
                new HttpMessagesMock().CreateNOKMessage());
        }

        [Test]
        public async Task ShouldReturnMappedDataForValidLatLon()
        {
            SetupWebMessages(true, true);

            var weatherResult = await sut.GetWeatherForDay(randomDate, randomCity);

            Assert.IsTrue(weatherResult, weatherResult.IsErr ? weatherResult.Err.Message : "");

            var weather = weatherResult.Get;

            Assert.AreEqual(TEMPERATURE, weather.Temperature.Value);
            Assert.AreEqual(PREC_TYPE, weather.Precipitation.Type);
        }

        [Test]
        public async Task ShouldReturnErrorIfDidntReceiveLatLonOrOkReponseFromWeatherService()
        {
            SetupWebMessages(false, false);

            var weatherResult = await sut.GetWeatherForDay(randomDate, randomCity);

            _latlonService.Verify(x => x.GetGeoLocationFor(randomCity), Times.Once());
            Assert.IsFalse(weatherResult);

            SetupWebMessages(true, false);

            weatherResult = await sut.GetWeatherForDay(randomDate, randomCity);

            _latlonService.Verify(x => x.GetGeoLocationFor(randomCity), Times.Exactly(2));
            Assert.IsFalse(weatherResult);

            SetupWebMessages(true, true);

            weatherResult = await sut.GetWeatherForDay(randomDate, randomCity);

            _latlonService.Verify(x => x.GetGeoLocationFor(randomCity), Times.Exactly(3));
            Assert.IsTrue(weatherResult, weatherResult.IsErr ? weatherResult.Err.Message : "");
        }

        [Test]
        public async Task ShouldReturnErrorIfDateIsNotInRange()
        {
            SetupWebMessages(true, true);

            var weatherResult = await sut.GetWeatherForDay(randomDate.AddDays(7).AddHours(1), randomCity);

            Assert.IsFalse(weatherResult);
        }

        [Test]
        public async Task ShouldWorkInRealEnvironment()
        {
            var client = new HttpClient();
            sut = new Timer7WeatherService(new TeleportLatLonService(client), client, new DateTimeService());

            var weatherResult = await sut.GetWeatherForDay(DateTime.Now.AddDays(1), randomCity);

            Assert.IsTrue(weatherResult, weatherResult.IsErr ? weatherResult.Err.Message : "");

            var weather = weatherResult.Get;

            Assert.IsNotNull(weather);
        }
    }
}
