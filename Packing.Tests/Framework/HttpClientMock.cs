using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace Packing.Tests.Framework
{
    class HttpClientMock
    {
        readonly Mock<HttpMessageHandler> _mock = new Mock<HttpMessageHandler>();

        public HttpClient Instance => new HttpClient(_mock.Object);

        public void SetupGet(Func<string, bool> urlValidator, HttpResponseMessage returns)
        {
            _mock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", 
                    ItExpr.Is<HttpRequestMessage>(x => urlValidator(x.RequestUri.AbsoluteUri) && x.Method.Method == "GET"), 
                    ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(returns))
                .Verifiable();
        }
    }
}
