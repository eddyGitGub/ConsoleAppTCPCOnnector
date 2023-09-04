using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System;

namespace ConsoleAppTCPCOnnector
{
    public class TCPConnector
    {
        public static async Task PostMessageAsync(string message, string server, int port)
        {
            // Create TCPClient 
            using TcpClient client = new TcpClient();

            // Connect to server endpoint
            await client.ConnectAsync(server, port);

            // Get Network Stream
            using NetworkStream stream = client.GetStream();

            // Convert message to bytes
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // Prepend length of message
            byte[] lenBytes = BitConverter.GetBytes(messageBytes.Length);
            byte[] sendBytes = lenBytes.Concat(messageBytes).ToArray();

            // Send message
            await stream.WriteAsync(sendBytes, 0, sendBytes.Length);

            // Close client
            client.Close();
        }



        public static async Task<string> PostMessageAndGetResponseAsync(string message, string server, int port)
        {
            using TcpClient client = new TcpClient();
            await client.ConnectAsync(server, port);

            using NetworkStream stream = client.GetStream();

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] lenBytes = BitConverter.GetBytes(messageBytes.Length);

            await stream.WriteAsync(lenBytes, 0, lenBytes.Length);
            await stream.WriteAsync(messageBytes, 0, messageBytes.Length);

            // Read response length
            byte[] respLenBytes = new byte[4];
            await stream.ReadAsync(respLenBytes, 0, 4);
            int respLength = BitConverter.ToInt32(respLenBytes, 0);

            // Read response body
            byte[] respBytes = new byte[respLength];
            await stream.ReadAsync(respBytes, 0, respLength);

            string response = Encoding.UTF8.GetString(respBytes);

            return response;
        }


        public static async Task<string> PostMessageWithSSLAsync(string message, string server, int port)
        {
            using TcpClient client = new();
            await client.ConnectAsync(server, port);

            using SslStream sslStream = new SslStream(client.GetStream());

            // Authenticate certificate
            await sslStream.AuthenticateAsClientAsync(server, null,
              SslProtocols.Tls12, false);

            // Send message
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] packet = PreparePacket(messageBytes);
            await sslStream.WriteAsync(packet);

            // Read response

            byte[] header = new byte[2];
            await sslStream.ReadAsync(header.AsMemory(0, 2));

            int length = (header[0] << 8) | header[1];
            byte[] body = new byte[length];

            int bytesRead = await sslStream.ReadAsync(body.AsMemory(0, length));

            // Convert to string
            string response = Encoding.UTF8.GetString(body, 0, bytesRead);

            return response;
        }

        private static byte[] PreparePacket(byte[] data)
        {
            byte[] packet = new byte[data.Length + 2];
            packet[0] = (byte)(data.Length >> 8);
            packet[1] = (byte)data.Length;
            Array.Copy(data, 0, packet, 2, data.Length);
            return packet;
        }
    }
}
