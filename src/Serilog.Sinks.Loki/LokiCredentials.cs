namespace Serilog.Sinks.Loki
{
    public class NoAuthCredentials : LokiCredentials
    {
        public NoAuthCredentials(string url)
        {
            Url = url;
        }
    }
    
    public class BasicAuthCredentials : LokiCredentials
    {
        public BasicAuthCredentials(string url, string username, string password)
        {
            Url = url;
            Username = username;
            Password = password;
        }
        
        public string Username { get; }

        public string Password { get; }
    }
    
    public abstract class LokiCredentials
    {
        public string Url { get; protected set; } 
    }
}