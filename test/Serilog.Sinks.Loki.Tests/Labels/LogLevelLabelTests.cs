using System.Linq;
using Newtonsoft.Json;
using Serilog.Sinks.Loki.Labels;
using Serilog.Sinks.Loki.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.Loki.Tests.Labels
{
    public class LogLevelTests
    {
        private readonly TestHttpClient _client;

        public LogLevelTests()
        {
            _client = new TestHttpClient();
        }
        
        [Fact]
        public void NoLabelIsSet()
        {
            // Arrange
            var provider = new DefaultLogLabelProvider(new LokiLabel[0], new string[0]); // Explicitly NOT include level
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.LokiHttp(() => new LokiSinkConfiguration
                {
                    LokiUrl = "http://test:80",
                    LogLabelProvider = provider,
                    HttpClient = _client
                })
                .CreateLogger();
            
            // Act
            log.Fatal("Fatal Level");
            log.Dispose();
            
            // Assert
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
            response.Streams.First().Labels.ShouldBe("{}");
        }
        
        [Fact]
        public void VerboseLabelIsSet()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.LokiHttp(() => new LokiSinkConfiguration
                {
                    LokiUrl = "http://test:80",
                    HttpClient = _client
                })
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
                .WriteTo.LokiHttp(() => new LokiSinkConfiguration
                {
                    LokiUrl = "http://test:80",
                    HttpClient = _client
                })
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
                .WriteTo.LokiHttp(() => new LokiSinkConfiguration
                {
                    LokiUrl = "http://test:80",
                    HttpClient = _client
                })
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
                .WriteTo.LokiHttp(() => new LokiSinkConfiguration
                {
                    LokiUrl = "http://test:80",
                    HttpClient = _client
                })
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
                .WriteTo.LokiHttp(() => new LokiSinkConfiguration
                {
                    LokiUrl = "http://test:80",
                    HttpClient = _client
                })
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