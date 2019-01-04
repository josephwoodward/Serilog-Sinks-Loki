using System;
using System.IO;
using Serilog;
using Serilog.Sinks.Loki;
using Xunit;

namespace erilog.Sinks.Loki.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                /*.WriteTo.Loki()*/
                .CreateLogger();
            
            log.Error("Something's wrong");

        }
    }
}