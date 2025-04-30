using Discord.API;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CheckersDiscordBot.Services
{
    public class TCPHandler
    {
        public void runListener()
        {
            TcpListener server = null;

            try
            {
                Int32 port = 12684;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                string data = null;
                Encoding enc = Encoding.ASCII;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    using TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        //data = enc.GetString(bytes, 0, i);
                        data = Convert.ToHexString(bytes);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = enc.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        public void SendMove(bool team, string from, string to)
        {
            try
            {
                Int32 port = 12683;
                string server = "127.0.0.1";

                using TcpClient client = new TcpClient(server, port);

                Byte[] byte_msg = new byte[256];

                //string msg = "6392F4480000000701643600633500".PadRight(512, '0');
                //string msg = "6392F44800000007006733006834".PadRight(512, '0');

                string header = "6392F448";

                int size = (3+from.Length+to.Length);
                string size_hex = size.ToString("X8");

                //Console.WriteLine("Size: {0}", size_hex);

                string from_hex = Convert.ToHexString(System.Text.Encoding.ASCII.GetBytes(from));

                string to_hex = Convert.ToHexString(System.Text.Encoding.ASCII.GetBytes(to));

                string hex_message = ("0" + (team ? "1" : "0") + from_hex + "00" + to_hex).PadRight(496, '0');

                Console.WriteLine("Test: {0}", header+size_hex+hex_message);

                //Console.WriteLine("Sending: {0}", msg);

                Byte[] data = Convert.FromHexString(header+size_hex+hex_message);//System.Text.Encoding.ASCII.GetBytes(msg);

                Console.WriteLine("Sending: {0}", Convert.ToHexString(data));

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", data);


                // Receive the server response.

                // Buffer to store the response bytes.
                //data = new Byte[256];

                //// String to store the response ASCII representation.
                //string responseData = string.Empty;

                //// Read the first batch of the TcpServer response bytes.
                //Int32 bytes = stream.Read(data, 0, data.Length);
                //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                //Console.WriteLine("Received: {0}", responseData);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}
