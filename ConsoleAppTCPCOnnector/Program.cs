// See https://aka.ms/new-console-template for more information
using ConsoleAppTCPCOnnector;
using System.Net.Security;
using System.Net.Sockets;

//Console.WriteLine("Hello, World!");
//var url = ""
//var client = new SslTcpClient("example.com", 443);
//client.Connect();
class Program
{
    static void Main()
    {
        
        var client = new SslTcpClient("arca-pos.qa.arca-payments.network", 11000);
        client.Connect();
    }
}