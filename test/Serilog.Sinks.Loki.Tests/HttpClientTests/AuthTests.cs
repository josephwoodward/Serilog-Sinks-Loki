using Serilog.Sinks.Loki.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.Loki.Tests.HttpClientTests
{
    public class AuthTests : IClassFixture<HttpClientTestFixture>
    {
        private readonly TestHttpClient _client;

        public AuthTests()
        {
            _client = new TestHttpClient();
        }

        [Fact]
        public void BasicAuthHeaderIsCorrect()
        {
            // Arrange
            var credentials = new BasicAuthCredentials("http://test:80", "Walter", "White");
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LokiHttp(credentials, httpClient: _client)
                .CreateLogger();

            // Act
            log.Error("Something's wrong");
            log.Dispose();

            // Assert
            var auth = _client.Client.DefaultRequestHeaders.Authorization;
            auth.ShouldSatisfyAllConditions(
                () => auth.Scheme.ShouldBe("Basic"),
                () => auth.Parameter.ShouldBe("V2FsdGVyOldoaXRl")
            );
        }

        [Fact]
        public void NoAuthHeaderIsCorrect()
        {
            // Arrange
            var credentials = new NoAuthCredentials("http://test:80");
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LokiHttp(credentials, httpClient: _client)
                .CreateLogger();

            // Act
            log.Error("Something's wrong");
            log.Dispose();

            // Assert
            _client.Client.DefaultRequestHeaders.Authorization.ShouldBeNull();
        }
    }
}