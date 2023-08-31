using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

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