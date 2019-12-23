using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    class Program
    {
        
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;

        static void Main(string[] args)
        {
            client = new TcpClient();
            try
            {
                client.Connect(host, port); 
                stream = client.GetStream(); 

                string message = "1";
                byte[] data = Encoding.Unicode.GetBytes(message);
               
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); 
                Console.WriteLine("Добро пожаловать");
                Console.WriteLine("Ожидаем второго игрока");
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
       
        static void SendMessage()
        {
         

            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
        // получение сообщений
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                    string message = builder.ToString();

                    Console.WriteLine(message);//вывод сообщения
                    if (message == "Противник вышел")
                    {
                       
                        Disconnect();
                    }
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {

            Console.ReadLine();
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Console.ReadLine();
            Environment.Exit(0); //завершение процесса
        }
    }
}