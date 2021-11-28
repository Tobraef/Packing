using Packing.Model.Location;
using Packing.Services.Location.Teleport.JsonParser;
using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Packing.Services.Location
{
    public class TeleportLatLonService : ILatLonService
    {
        readonly HttpClient _http;

        public TeleportLatLonService(HttpClient http)
        {
            _http = http;
        }

        async Task<Result<JsonDocument, MessageError>> GetAndParse(string url)
        {
            var response = await _http.GetAsync(url);
            return await new RequestToJsonParser().RequestToJson(response);
        }

        async Task<Result<int, MessageError>> GetGeoId(City city)
        {
            var jsonDoc = await GetAndParse(@$"https://api.teleport.org/api/cities/?search={city.Name}");
            if (jsonDoc.IsErr)
                return jsonDoc.MapErr<int>();
            return new CitySearchResponse(jsonDoc.Get)
                .CitiesGeoIds()
                .Then<int>(geoIds => geoIds.First());
        }

        async Task<Result<LatLon, MessageError>> GetLanLon(int geoId)
        {
            var jsonDoc = await GetAndParse($@"https://api.teleport.org/api/cities/geonameid:{geoId}");
            if (jsonDoc)
            {
                return new CityDataResponse(jsonDoc.Get)
                    .CityLatLon();
            }
            return jsonDoc.MapErr<LatLon>();
        }

        public async Task<Result<LatLon, MessageError>> GetGeoLocationFor(City city)
        {
            var geoId = await GetGeoId(city);
            if (geoId)
            {
                return await GetLanLon(geoId.Get);
            }
            return geoId.MapErr<LatLon>();
        }
    }
}
