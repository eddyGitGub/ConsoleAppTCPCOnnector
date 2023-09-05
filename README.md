# Send Message over SSL TCP Client
### This C# method provides functionality to send a message to a TCP server endpoint and receive the response using SSL certificate authentication.

### Usage
Call the PostMessageWithSSLAsync method passing in:

message - The string message to send

server - The server IP or hostname

port - The TCP port of the server

clientCertificate - X509 certificate for client authentication (optional)


How it works
The method:
- Creates a TcpClient and connects to the server endpoint
- Gets NetworkStream and converts to SslStream
- Authenticates as a client using the certificate
- Converts the message to bytes and prepends a length header
- Writes the message bytes asynchronously over SSL stream
- Reads the SSL stream response in chunks parsing the length header
- Converts the response bytes to a string
- Closes connections and returns response
This allows sending a message and getting the response securely over SSL.

### Requirements
- .NET 6+
- NuGet Package:
- 
System.Net.Security
