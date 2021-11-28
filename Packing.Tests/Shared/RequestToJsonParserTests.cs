using NUnit.Framework;
using Packing.Services.Weather.Data;
using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Packing.Tests.Shared
{
    [TestFixture]
    class RequestToJsonParserTests
    {
        RequestToJsonParser sut = new RequestToJsonParser();

        class InnerType
        { 
            public string Data { get; set; }
            public int? DataI { get; set; }
        }

        class SomeType
        {
            public int? Integer { get; set; }
            public string Stringer { get; set; }
            public List<InnerType> Values { get; set; }
        }

        [Test]
        public async Task ParseToRandomType()
        {
            var content = new StringContent("{\"integer\":3, \"stringer\": \"abc\", \"values\":[{\"data\": \"cba\", \"datai\": 2}]}");
            var message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = content;

            var result = await sut.RequestToJsonSerializableType<SomeType>(message);

            Assert.IsTrue(result, result.IsErr ? result.Err.Message : "");

            var value = result.Get;

            Assert.AreEqual(3, value.Integer);
            Assert.AreEqual("abc", value.Stringer);
            Assert.AreEqual(2, value.Values[0].DataI);
        }

        [Test]
        public async Task ParseToWeatherDataResponse()
        {
            var content = new StringContent("{ \"product\" : \"civil\" , \"init\" : \"2021112806\" , \"dataseries\" : [ { \"timepoint\" : 3, \"cloudcover\" : 2, \"lifted_index\" : 10, \"prec_type\" : \"none\", \"prec_amount\" : 0, \"temp2m\" : 25, \"rh2m\" : \"33 % \", \"wind10m\" : { \"direction\" : \"N\", \"speed\" : 3 }, \"weather\" : \"clearday\" }]}");
            var message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = content;

            var result = await sut.RequestToJsonSerializableType<WeatherDataResponse>(message);

            Assert.IsTrue(result, result.IsErr ? result.Err.Message : "");
            Assert.AreEqual(2, result.Get.DataSeries[0].CloudCover);
        }
    }
}
