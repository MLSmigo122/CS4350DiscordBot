using Microsoft.Extensions.DependencyInjection;
using CheckersDiscordBot.Modules;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Discord.WebSocket;
using Discord.Commands;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Discord;

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
                        if (data.Substring(0, 8) == "C32FA98A")
                        {
                            bool winner = false;
                            if (data[17] == '1')
                                winner = true;
                            else if (data[17] == '0')
                                winner = false;
                            else
                                Program.sendMsg("Unknown packet recieved");
                            Program.sendMsg((winner ? "white" : "red") + " wins!");
                        }
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

                //Build message
                string header = "6392F448";
                int size = (3+from.Length+to.Length);
                string size_hex = size.ToString("X8");
                string from_hex = Convert.ToHexString(System.Text.Encoding.ASCII.GetBytes(from.ToLower()));
                string to_hex = Convert.ToHexString(System.Text.Encoding.ASCII.GetBytes(to.ToLower()));
                string hex_message = ("0" + (team ? "1" : "0") + from_hex + "00" + to_hex + "00").PadRight(496, '0');

                Byte[] data = Convert.FromHexString(header+size_hex+hex_message);

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);
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

        public void SendJump(bool team, string from, string[] jumps)
        {
            try
            {
                Int32 port = 12683;
                string server = "127.0.0.1";

                using TcpClient client = new TcpClient(server, port);

                //Build message
                int num_jumps = jumps.Length;
                string jumps_str = string.Join("", jumps);
                string header = "394CEC41";
                int size = (7 + from.Length + jumps_str.Length);
                string size_hex = size.ToString("X8");
                string num_jumps_hex = num_jumps.ToString("X8");
                string from_hex = Convert.ToHexString(System.Text.Encoding.ASCII.GetBytes(from.ToLower()));
                string to_hex = Convert.ToHexString(System.Text.Encoding.ASCII.GetBytes(jumps_str.ToLower()));
                string hex_message = ("0" + (team ? "1" : "0") + num_jumps_hex + from_hex + "00" + to_hex + "00").PadRight(496, '0');

                Byte[] data = Convert.FromHexString(header + size_hex + hex_message);

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);
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

        public void SendReset()
        {
            try
            {
                Int32 port = 12683;
                string server = "127.0.0.1";

                using TcpClient client = new TcpClient(server, port);

                //Build message
                string header = "7CFC7CA4";
                string fill = ("").PadRight(504, '0');

                Byte[] data = Convert.FromHexString(header + fill);

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);
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
