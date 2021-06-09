using System.Linq;
using Newtonsoft.Json;
using Serilog.Sinks.Loki.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.Loki.Tests.Content
{
    public class ContentTests : IClassFixture<HttpClientTestFixture>
    {
        private readonly HttpClientTestFixture _httpClientTestFixture;
        private readonly TestHttpClient _client;
        private readonly LokiSinkConfiguration _configuration;

        public ContentTests(HttpClientTestFixture httpClientTestFixture)
        {
            _httpClientTestFixture = httpClientTestFixture;
            _client = new TestHttpClient();
            _configuration = new LokiSinkConfiguration
            {
                LokiUrl = "http://test:80",
                LokiUsername = "Walter",
                LokiPassword = "White",
                HttpClient = _client
            };
        }

        [Fact]
        public void QuotedContentStringsAreSimplified()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LokiHttp(() => _configuration)
                .CreateLogger();

            // Act
            log.Error("Data with quotes: {data}", "Text \"with\" quotes.");
            log.Dispose();

            // Assert
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
            response.Streams.First().Entries.First().Line.ShouldStartWith("Data with quotes: Text \"with\" quotes.\n");
        }

        [Fact]
        public void UnquotedWhitespaceFieldsAreNotSimplified()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LokiHttp(() => _configuration)
                .CreateLogger();

            // Act
            log.Error("Data without quotes: {data}", "Text without quotes.");
            log.Dispose();

            // Assert
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
            response.Streams.First().Entries.First().Line.ShouldContain("data=\"Text without quotes.\"");
        }

        [Fact]
        public void QuotedWhitespaceFieldsAreNotSimplified()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LokiHttp(() => _configuration)
                .CreateLogger();

            // Act
            log.Error("Data with quotes: {data}", "Text \"with\" quotes.");
            log.Dispose();

            // Assert
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
            response.Streams.First().Entries.First().Line.ShouldContain("data=\"Text \\\"with\\\" quotes.\"");
        }

        [Fact]
        public void UnquotedNonWhitespaceFieldsAreSimplified()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LokiHttp(() => _configuration)
                .CreateLogger();

            // Act
            log.Error("Data without quotes: {data}", "TextWithoutQuotes");
            log.Dispose();

            // Assert
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
            response.Streams.First().Entries.First().Line.ShouldContain("data=TextWithoutQuotes");
        }

        [Fact]
        public void QuotedNonWhitespaceFieldsAreSimplified()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LokiHttp(() => _configuration)
                .CreateLogger();

            // Act
            log.Error("Data with quotes: {data}", "\"TextWithQuotes\"");
            log.Dispose();

            // Assert
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
            response.Streams.First().Entries.First().Line.ShouldContain("data=\"TextWithQuotes\"");
        }
    }
}