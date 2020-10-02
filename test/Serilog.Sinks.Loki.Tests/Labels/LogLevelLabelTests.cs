using System.Linq;
using Newtonsoft.Json;
using Serilog.Sinks.Loki.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.Loki.Tests.Labels
{
    public class LogLevelTests
    {
        private readonly TestHttpClient _client;
        private readonly BasicAuthCredentials _credentials;

        public LogLevelTests()
        {
            _client = new TestHttpClient();
            _credentials = new BasicAuthCredentials("http://test:80", "Walter", "White");
        }
        
        [Fact]
        public void VerboseLabelIsSet()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.LokiHttp(_credentials, httpClient: _client)
                .CreateLogger();
            
            // Act
            log.Verbose("Verbose Level");
            log.Dispose();
            
            // Assert
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
            response.Streams.First().Labels.ShouldBe("{level=\"trace\"}");
        }
        
        [Fact]
        public void DebugLabelIsSet()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LokiHttp(_credentials, httpClient: _client)
                .CreateLogger();
            
            // Act
            log.Debug("Debug Level");
            log.Dispose();
            
            // Assert
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
            response.Streams.First().Labels.ShouldBe("{level=\"debug\"}");
        }
        
        [Fact]
        public void InformationLabelIsSet()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LokiHttp(_credentials, httpClient: _client)
                .CreateLogger();
            
            // Act
            log.Information("Information Level");
            log.Dispose();
            
            // Assert
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
            response.Streams.First().Labels.ShouldBe("{level=\"info\"}");
        }

        [Fact]
        public void ErrorLabelIsSet()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.LokiHttp(_credentials, httpClient: _client)
                .CreateLogger();
            
            // Act
            log.Error("Error Level");
            log.Dispose();
            
            // Assert
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
            response.Streams.First().Labels.ShouldBe("{level=\"error\"}");
        }

        [Fact]
        public void FatalLabelIsSet()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Fatal()
                .WriteTo.LokiHttp(_credentials, httpClient: _client)
                .CreateLogger();
            
            // Act
            log.Fatal("Fatal Level");
            log.Dispose();
            
            // Assert
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
            response.Streams.First().Labels.ShouldBe("{level=\"critical\"}");
        }
    }
}