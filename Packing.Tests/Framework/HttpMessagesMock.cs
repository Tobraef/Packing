using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Packing.Tests.Framework
{
    class HttpMessagesMock
    {
        public HttpResponseMessage CreateOkMessage<T>(T jsonSerializable)
        {
            var msg = new HttpResponseMessage(HttpStatusCode.OK);
            var jsonString = JsonSerializer.Serialize(jsonSerializable);
            msg.Content = new StringContent(jsonString);
            return msg;
        }

        public HttpResponseMessage CreateNOKMessage()
            => new HttpResponseMessage(HttpStatusCode.BadRequest);
    }
}
