using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using ProtoBuf;
using Snappy;

namespace ConsoleApp1
{
    [DataContract]
public sealed class PerfTest
{
    [DataMember(Order = 1)]
    public int Foo { get; set; }

    [DataMember(Order = 2)]
    public string Bar { get; set; }

    [DataMember(Order = 3)]
    public float Blip { get; set; }

    [DataMember(Order = 4)]
    public double Blop { get; set; }

}
    
    class Program
    {
        static void Main(string[] args)
        {

            var res = SnappyCodec.Compress(Encoding.ASCII.GetBytes(@"2018-12-19T23:53:56Z component time=\""2018-12-19T23:53:56.7235600Z\"" level=Error"));

            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:3100")
            };

            client.DefaultRequestHeaders.Accept.Clear();

            var byteArrayContent = new ByteArrayContent(res);

            var result = client.PostAsync("/api/prom/push", byteArrayContent).Result;
            Console.WriteLine(result.StatusCode);
            Console.WriteLine(result.Content.ReadAsStringAsync().Result);
        }
    }
}