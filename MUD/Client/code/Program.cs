using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace Server
{
    class Program
    {
        static void clientReceive(Object o)
        {
            bool bQuit = false;

            Socket morseClient = (Socket)o;

            while (bQuit == false)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = morseClient.Receive(buffer);

                    if (result > 0)
                    {
                        ASCIIEncoding encoder = new ASCIIEncoding();
                        String recdMsg = encoder.GetString(buffer, 0, result);

                        Console.WriteLine(recdMsg);
                    }

                }
                catch (Exception)
                {
                    bQuit = true;
                    Console.WriteLine("Lost server!");
                }

            }
        }

        static void Main(string[] args)
        {
            Socket chatClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            bool bConnected = false;
            while (bConnected == false)
            {

                try
                {
                    chatClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
                    bConnected = true;
                }
                catch (System.Exception ex)
                {
                    Thread.Sleep(1000);
                }
            }

            Thread myThread = new Thread(clientReceive);
            myThread.Start(chatClient);
            

            bool bQuit = false;

            Console.WriteLine("Client");

            while (!bQuit)
            {
                Console.Write("Enter Msg: ");
                String Msg = Console.ReadLine();
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(Msg);

                try
                {
                    int bytesSent = chatClient.Send(buffer);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
