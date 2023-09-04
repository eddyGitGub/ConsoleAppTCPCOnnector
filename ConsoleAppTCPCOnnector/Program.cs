// See https://aka.ms/new-console-template for more information
using ConsoleAppTCPCOnnector;
using imohsenb.iso8583.builders;
using imohsenb.iso8583.entities;
using imohsenb.iso8583.enums;
using imohsenb.iso8583.interfaces;
using imohsenb.iso8583.utils;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

//Console.WriteLine("Hello, World!");
//var url = ""
//var client = new SslTcpClient("example.com", 443);
//client.Connect();
class Program
{
    static async Task Main()
    {


        ISOMessage isoMessage = ISOMessageBuilder.Packer(VERSION.V1987)
                        .NetworkManagement()
                        .MTI(MESSAGE_FUNCTION.Request, MESSAGE_ORIGIN.Acquirer)
                        .ProcessCode("930000")
                        .SetField(FIELDS.F11_STAN, "385629")
                        .SetField(FIELDS.F22_EntryMode, "1234")
                        .SetField(FIELDS.F24_NII_FunctionCode, "4321")
                        .SetField(FIELDS.F25_POS_ConditionCode, "12")
                        .SetField(FIELDS.F41_CA_TerminalID, "12345678")
                        .SetField(FIELDS.F42_CA_ID, "123456789876543")
                                        .GenerateMac(data =>
                                        {
                                            return StringUtil.HexStringToByteArray("AB6A53FC655F1487");
                                        })
                        .SetHeader("1234567890")
                        .Build();

        //var client1 = new SslTcpClient("arca-pos.qa.arca-payments.network", 11000);
        //client1.Connect22("ISO0150000220000001744121234567890123456781905311240400000000000123");
        ////client.Connect();
        //client.SendMessage("ISO0150000220000001744121234567890123456781905311240400000000000123");
        var tcpConnect = new TCPConnector();
        var message = isoMessage.ToString();
        string response = await TCPConnector.PostMessageWithSSLAsync(message, "arca-pos.qa.arca-payments.network", 11000);
        //string response = await tcpConnect.PostMessageAndGetResponseAsync("Hello", "arca-pos.qa.arca-payments.network", 11000);
    }
}