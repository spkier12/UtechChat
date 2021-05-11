using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace UtechChat
{
    class Program
    {
        public NetworkStream[] sm = new NetworkStream[4096];
        public int smcount;
        static void Main()
        {
            Program gclass = new();
            IPAddress ip = IPAddress.Any;
            TcpListener server = new(ip, 5000);
            server.Start();
            Console.WriteLine($"Server started on {ip}:5000");

            while (true)
            {
                try
                {
                    TcpClient client = server.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    Console.WriteLine("Client connected");

                    gclass.smcount++;
                    if (gclass.smcount >= 4090) gclass.smcount = 0;
                    gclass.sm[gclass.smcount] = stream;

                    // Handle all the data
                    Task.Run(() => gclass.handledata(stream, client, gclass.sm));
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        // Handle all the heavy workflow on a diffrent thread
        void handledata(NetworkStream stream, TcpClient client, NetworkStream[] allstreams)
        {
            while (client.Connected)
            {
                // store stream to string
                byte[] buffer = new byte[2048];
                stream.Read(buffer, 0, buffer.Length);
                stream.Write(buffer, 0, buffer.Length);
                senddata(buffer, allstreams);
            }
        }

        // Send data back to all clients
        void senddata(byte[] msg, NetworkStream[] allstreams)
        {
            Program gclass = new();
            Console.WriteLine("Network stream: " + allstreams[0]);
            try
            {
                foreach(NetworkStream d in allstreams)
                {
                    if (d != null)
                    {
                        if (d.CanWrite) d.Write(msg, 0, msg.Length);
                    }
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
