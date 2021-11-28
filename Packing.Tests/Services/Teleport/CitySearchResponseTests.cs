using NUnit.Framework;
using Packing.Services.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Packing.Tests.Services.Teleport
{
    [TestFixture]
    class CitySearchResponseTests
    {
        CitySearchResponse sut;

        [Test]
        public void ShouldReturnGeoIdWithValidResponse()
        {
            var json = JsonDocument.Parse(
"{\"_embedded\": {\"city:search-results\": [{\"_links\": {\"city:item\": {\"href\": \"https://api.teleport.org/api/cities/geonameid:324190/\"}},\"matching_alternate_names\":[{\"name\": \"Alanya\"},{\"name\": \"alanya\"}],\"matching_full_name\": \"Alanya, Antalya, Turkey\"}]}}");
            sut = new CitySearchResponse(json);

            var result = sut.CitiesGeoIds().Get.Single();

            Assert.AreEqual(324190, result);
        }

        [Test]
        public void ShouldReturnAllGeoIds()
        {
            var json = JsonDocument.Parse(
"{\"_embedded\": {\"city:search-results\": " +
"[{\"_links\": {\"city:item\": {\"href\": \"https://api.teleport.org/api/cities/geonameid:324190/\"}},\"matching_alternate_names\":[{\"name\": \"Alanya\"},{\"name\": \"alanya\"}],\"matching_full_name\": \"Alanya, Antalya, Turkey\"}," +
"{\"_links\": {\"city:item\": {\"href\": \"https://api.teleport.org/api/cities/geonameid:1234567890/\"}},\"matching_alternate_names\":[{\"name\": \"Alanya\"},{\"name\": \"alanya\"}],\"matching_full_name\": \"Alanya, Antalya, Turkey\"}]}}");
            sut = new CitySearchResponse(json);

            var result = sut.CitiesGeoIds().Get.ToArray();

            Assert.AreEqual(324190, result[0]);
            Assert.AreEqual(1234567890, result[1]);
        }

        [Test]
        public void ShouldReturnErrorOnNoFoundItems()
        {
            var json = JsonDocument.Parse(
"{\"_embedded\": {\"city:search-results\": []}}");
            sut = new CitySearchResponse(json);

            var result = sut.CitiesGeoIds().IsErr;

            Assert.IsTrue(result);
        }
    }
}
