using NUnit.Framework;
using Packing.Services.Location.Teleport.JsonParser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Packing.Tests.Services.Teleport
{
    [TestFixture]
    class CityDataResponseTests
    {
        CityDataResponse sut;

        [Test]
        [TestCase("{\"_links\":{\"city: admin1_division\":{\"href\":\"https://api.teleport.org/api/countries/iso_alpha2:TR/admin1_divisions/geonames:07/\",\"name\":\"Antalya\"},\"city:alternate-names\":{\"href\":\"https://api.teleport.org/api/cities/geonameid:324190/alternate_names/\"},\"city:country\":{\"href\":\"https://api.teleport.org/api/countries/iso_alpha2:TR/\",\"name\":\"Turkey\"}," +
            "\"city:timezone\":{\"href\":\"https://api.teleport.org/api/timezones/iana:Europe%2FIstanbul/\",\"name\":\"Europe/Istanbul\"},\"curies\":[{\"href\":\"https://developers.teleport.org/api/resources/Location/#!/relations/{rel}/\",\"name\":\"location\",\"templated\":true},{\"href\":\"https://developers.teleport.org/api/resources/City/#!/relations/{rel}\",\"name\":\"city\",\"templated\":true},{\"href\":\"https://developers.teleport.org/api/resources/UrbanArea/#!/relations/{rel}/\",\"name\":\"ua\",\"templated\":true}," +
            "{\"href\":\"https://developers.teleport.org/api/resources/Country/#!/relations/{rel}/\",\"name\":\"country\",\"templated\":true},{\"href\":\"https://developers.teleport.org/api/resources/Admin1Division/#!/relations/{rel}/\",\"name\":\"a1\",\"templated\":true},{\"href\":\"https://developers.teleport.org/api/resources/Timezone/#!/relations/{rel}/\",\"name\":\"tz\",\"templated\":true}]," +
            "\"self\":{\"href\":\"https://api.teleport.org/api/cities/geonameid:324190/\"}},\"full_name\":\"Alanya, Antalya, Turkey\",\"geoname_id\":324190,\"location\":{\"geohash\":\"swqzbhtdcbepys96ubex\",\"latlon\":" +
            "{\"latitude\":36.54375,\"longitude\":31.99982}},\"name\":\"Alanya\",\"population\":112969}", 36.54375, 31.99982)]
        public void ShouldReturnValidLatLonOnCorrectInput(string json, decimal lat, decimal lon)
        {
            sut = new CityDataResponse(JsonDocument.Parse(json));

            var result = sut.CityLatLon();

            if (!result)
            {
                Assert.IsTrue(result, result.Err.Message);
            }

            var values = result.Get;

            Assert.AreEqual(lat, values.Latitude);
            Assert.AreEqual(lon, values.Longtitude);
        }
    }
}
