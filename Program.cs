using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace UtechChat
{
    class Program
    {
        static void Main()
        {
            Program gclass = new();
            IPAddress ip = IPAddress.Any;
            TcpListener server = new(ip, 5000);
            Console.WriteLine($"Server started on {ip}:5000");
            server.Start();

            NetworkStream[] sm = new NetworkStream[4096];
            int count = 0;

            while (true)
            {
                try
                {
                    TcpClient client = server.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    Console.WriteLine($"Client Nr: {count} Connected of Total: {sm.Length}");

                    count++;
                    if (count >= 4090) count = 0;
                    sm[count] = stream;

                    // Handle all the data
                    Task.Run(() => {
                        while (client.Connected)
                        {
                            byte[] buffer = new byte[2048];
                            stream.Read(buffer, 0, buffer.Length);
                            stream.Write(buffer, 0, buffer.Length);
                            gclass.senddata(buffer, sm);
                        }
                    });
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        // Send data back to all clients
        void senddata(byte[] msg, NetworkStream[] allstreams)
        {

            try
            {
                foreach(NetworkStream d in allstreams)
                {
                    if (d != null) if (d.Socket.Connected) d.Write(msg, 0, msg.Length);
                }
            } 
            catch
            {
                Console.WriteLine("Broken connection");
            }
        }
    }
}
