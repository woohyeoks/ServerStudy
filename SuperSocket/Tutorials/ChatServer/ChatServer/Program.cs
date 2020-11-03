using System;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello ChatServer!");

            var serverOption = new ChatServerOption();
            serverOption.Name = "ChatServer";
            serverOption.MaxConnectionNumber = 256;
            serverOption.Port = 8888;
            serverOption.MaxRequestLength = 1024;
            serverOption.ReceiveBufferSize = 16384;
            serverOption.SendBufferSize = 16384;
            serverOption.RoomMaxCount = 100;
            serverOption.RoomMaxUserCount = 4;
            serverOption.RoomStartNumber = 0;

            var server = new MainServer();
            server.InitConfig(serverOption);
            server.CreateStartServer();

            MainServer.MainLogger.Info("Press q to shut down the server");

            while (true)
            {
                System.Threading.Thread.Sleep(50);

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.KeyChar == 'q')
                    {
                        Console.WriteLine("Server Terminate ~~~");
                        server.StopServer();
                        break;
                    }
                }

            }
        }
    }
}
