using Serilog.Sinks.Loki.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.Loki.Tests.HttpClientTests
{
    public class RequestUriTests
    {
        private readonly TestHttpClient _client;

        public RequestUriTests()
        {
            _client = new TestHttpClient();
        }
        
        [Theory]
        [InlineData("http://test:80")]
        [InlineData("http://test:80/")]
        public void RequestUriIsCorrect(string address)
        {
            // Arrange
            var credentials = new NoAuthCredentials(address);
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LokiHttp(credentials, httpClient: _client)
                .CreateLogger();
            
            // Act
            log.Error("Something's wrong");
            log.Dispose();

            // Assert
            _client.RequestUri.ShouldBe(LokiRouteBuilder.BuildPostUri(credentials.Url));
        }
    }
}