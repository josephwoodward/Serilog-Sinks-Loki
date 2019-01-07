rm -rf ./package

cd ./src/

dotnet pack ./Serilog.Sinks.Loki/Serilog.Sinks.Loki.csproj -o ../../package/ -c release