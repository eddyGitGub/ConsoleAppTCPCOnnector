using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ConsoleAppTCPCOnnector
{
    public class SslTcpClient
    {
        private readonly string _server;
        private readonly int _port;

        public SslTcpClient(string server, int port)
        {
            _server = server;
            _port = port;
        }

        public void Connect()
        {
            Console.WriteLine($"Connecting to server {_server} on port {_port}...");

            using var client = new TcpClient(_server, _port);
            Console.WriteLine("Client connected.");

            // Create an SSL stream that will close the client's stream.
            using var sslStream = new SslStream(
                client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null
            );

            try
            {
                sslStream.AuthenticateAsClient(_server);
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                if (e.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {e.InnerException.Message}");
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                client.Close();
                return;
            }

            // At this point, the SSL handshake is complete and you're connected securely!
            Console.WriteLine("SSL handshake completed successfully.");

            // TODO: Read/Write from/to the sslStream as required.
        }

        public async void SendMessage(string iso8583Message)
        {
            Console.WriteLine($"Connecting to server {_server} on port {_port}...");
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(_server);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            var ipEndPoint = new IPEndPoint(ipAddress, _port);
            //using var client = new TcpClient(, );
            using TcpClient client = new();
            await client.ConnectAsync(ipEndPoint);
            await using NetworkStream stream = client.GetStream();
            Console.WriteLine("Client connected.");

            using var sslStream = new SslStream(client.GetStream(), false, ValidateServerCertificate, null);

            try
            {
                sslStream.AuthenticateAsClient(_server, null, SslProtocols.Tls12, false);
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                if (e.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {e.InnerException.Message}");
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                client.Close();
                return;
            }
            try
            {


                // Convert ISO 8583 message to byte array and send
                var messageBytes = Encoding.ASCII.GetBytes(iso8583Message);
                sslStream.Write(messageBytes, 0, messageBytes.Length);
                sslStream.Flush();

                Console.WriteLine("Message sent.");

                // Read the response
                var buffer = new byte[4096];  // Buffer size can be adjusted
                int bytesRead = sslStream.Read(buffer, 0, buffer.Length);
                var response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                Console.WriteLine("Received response: " + response);

                //return response;
                // You might also want to read a response from the server here, depending on your requirements.

              
            }
            catch (Exception e)
            {

            }
        }
        public static bool ValidateServerCertificate(
          object sender,
          X509Certificate certificate,
          X509Chain chain,
          SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine($"Certificate error: {sslPolicyErrors}");
            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
    }
}