using Packing.Model.Location;
using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Packing.Services.Location.Teleport.JsonParser
{
    public class CityDataResponse
    {
        readonly JsonDocument _json;

        public CityDataResponse(JsonDocument jsonDocument)
        {
            _json = jsonDocument;
        }

        public Result<LatLon, MessageError> CityLatLon()
        {
            if (_json.RootElement.TryGetProperty("location", out var node))
            {
                if (node.TryGetProperty("latlon", out node))
                {
                    if (node.TryGetProperty("latitude", out var latitude) && node.TryGetProperty("longitude", out var longitude))
                    {
                        if (latitude.TryGetDecimal(out decimal lat) && longitude.TryGetDecimal(out decimal lon))
                        {
                            return LatLon.Create(lat, lon);
                        }
                        return new MessageError("Couldn't parse lat & lon nodes. These were latitude: " + latitude.ToString() + " and longitude: " + longitude.ToString());
                    }
                    return new MessageError("Didnt' find latitude or longitude in node.");
                }
                return new MessageError("Didn't find latlon property in document.");
            }
            return new MessageError("Didn't find location property in document.");
        }
    }
}
