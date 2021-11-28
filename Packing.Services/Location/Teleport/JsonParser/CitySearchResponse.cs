using Packing.Shared;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Packing.Services.Location
{
    public class CitySearchResponse
    {
        readonly JsonDocument _response;

        public CitySearchResponse(JsonDocument response)
        {
            _response = response;
        }

        public Result<IEnumerable<int>, MessageError> CitiesGeoIds()
        {
            var node = _response.RootElement;
            var embed = node.GetProperty("_embedded");
            if (embed.ValueKind == JsonValueKind.Undefined)
                return new MessageError("Couldn't find embed in given json.");
            var searchResults = embed.GetProperty(@"city:search-results");
            if (searchResults.ValueKind == JsonValueKind.Undefined)
                return new MessageError("Couldn't find city:search-results in given json.");
            if (searchResults.GetArrayLength() == 0)
                return new MessageError("Didn't find any city with a given name.");
            return GetGeoIdsFromSearchResults(searchResults);
        }

        private Result<IEnumerable<int>, MessageError> GetGeoIdsFromSearchResults(JsonElement searchResults)
        {
            var geoIds = searchResults
                .EnumerateArray()
                .Select(node =>
                {
                    if (node.TryGetProperty("_links", out node) &&
                        node.TryGetProperty("city:item", out node) &&
                        node.TryGetProperty("href", out node))
                    {
                        var href = node.GetString();
                        var match = Regex.Match(href, "(?<=geonameid:)[0-9]+");
                        if (match.Success)
                        {
                            return int.Parse(match.Value);
                        }
                    }
                    return (int?)null;
                })
                .Where(x => x != null)
                .Select(x => x ?? 0);
            if (!geoIds.Any())
                return new MessageError("Couldn't parse links json part.");
            return new Result<IEnumerable<int>, MessageError>(geoIds);
        }
    }
}
