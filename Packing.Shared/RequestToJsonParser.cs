using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Packing.Shared
{
    public class RequestToJsonParser
    {
        async Task<Result<JsonDocument, MessageError>> ParseToJson(Stream responseStream)
        {
            try
            {
                return await JsonDocument.ParseAsync(responseStream);
            }
            catch (JsonException exception)
            {
                return new MessageError("Couldn't parse json from received response. Reason: " + exception.Message);
            }
        }

        async Task<Result<T, MessageError>> ParseToType<T>(Stream responseStream)
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<T>(responseStream);
            }
            catch (JsonException exception)
            {
                return new MessageError("Couldn't parse json from received response. Reason: " + exception.Message);
            }
        }

        Result<HttpResponseMessage, MessageError> ValidateResponse(HttpResponseMessage response)
        {
            if (response == null)
                return new MessageError("Didn't receive any result from teleport api on city search.");
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return new MessageError($"Didn't receive OK status from teleport api. Received {response.StatusCode}");
            return response;
        }

        async Task<Result<T, MessageError>> RequestToType<T>(HttpResponseMessage response, Func<Stream, Task<Result<T, MessageError>>> parsingFunction)
        {
            var validation = ValidateResponse(response);
            if (validation)
            {
                return await parsingFunction(await response.Content.ReadAsStreamAsync());
            }
            return await Task.FromResult(validation.MapErr<T>());
        }

        public Task<Result<JsonDocument, MessageError>> RequestToJson(HttpResponseMessage response)
            => RequestToType(response, s => ParseToJson(s));

        public Task<Result<T, MessageError>> RequestToJsonSerializableType<T>(HttpResponseMessage response)
            => RequestToType(response, s => ParseToType<T>(s));
    }
}
