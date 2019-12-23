using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

using System.Reflection;
using System.IO;



namespace FinalGame21
{
    class Program
    {
        static TcpClient Pleer1;
        static TcpClient Pleer2;
        static TcpListener tcpListener;
        static void Main(string[] args)
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();

                Console.WriteLine("Ожидание подключений...");
                while (true)
                {
                    Pleer1 = tcpListener.AcceptTcpClient();
                    Console.WriteLine("Pleer1 join");

                    Pleer2 = tcpListener.AcceptTcpClient();
                    Console.WriteLine("Pleer2 join");
                    ClientObject clientObject = new ClientObject(Pleer1, Pleer2);

                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

                if (Pleer1 != null)
                    Pleer1.Close();
                if (Pleer2 != null)
                    Pleer2.Close();
            }


        }


    }
    public class ClientObject
    {
        public TcpClient Plr1;
        public TcpClient Plr2;

        public NetworkStream stream1;
        public NetworkStream stream2;

        public ClientObject(TcpClient Pleer1, TcpClient Pleer2)
        {
            Plr1 = Pleer1;
            Plr2 = Pleer2;
        }

        int summ1 = 0;
        int summ2 = 0;
        string message1;
        string message2;
        static Random rnda = new Random();
        static Random rndb = new Random();
        List<int> pleer2 = new List<int>();
        List<int> pleer1 = new List<int>();
        List<string> pleer1mast = new List<string>();
        List<string> pleer2mast = new List<string>();
        List<int> CardOff = new List<int>();
        List<string> MastOff = new List<string>();
        List<int> chislo = new List<int>() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        List<string> mast = new List<string>() { "H", "D", "C", "S" };
        int pleerwin1 = 0;
        int pleerwin2 = 0;
        int Ncard;
        int Card;
        int Mcard;
        string mst;
        string cardoff;

        List<string> COOF = new List<string>();
        public void Process()
        {



            //game = new Game();
            newstream(Plr1, Plr2);
            string message1;
            string message2;
            byte[] data = new byte[64];
            message1 = "Игрок 1. Инструкция: Отправить 1 для подтверждения. 2 или другую клавишу для отказа";
            //data = Encoding.Unicode.GetBytes(message);
            //stream1.Write(data, 0, data.Length);
            message2 = "Игрок 2. Инструкция: Отправить 1 для подтверждения. 2 или другую клавишу для отказа";
            //data = Encoding.Unicode.GetBytes(message2);
            //stream2.Write(data, 0, data.Length);
            SendMes(message1, message2);
            //Thread Game21Start = new Thread(new ThreadStart(game.StartGame));
            //Game21Start.Start();
            string cardoff;

            message1 = "";
            message2 = "";
            pleer1.Clear();
            pleer1mast.Clear();
            pleer2.Clear();
            pleer2mast.Clear();
            CardOff.Clear();
            MastOff.Clear();
            COOF.Clear();
            summ1 = 0;
            summ2 = 0;
            int newcard = 0;
            for (int summcard = 0; summcard <= 3;)
            {
                int Ncard = rnda.Next(chislo.Count);
                int Card = chislo[Ncard];
                int Mcard = rndb.Next(mast.Count);
                string mst = mast[Mcard];
                cardoff = Card + mst;
                //Console.WriteLine("Random 1 test: {0} {1}", cardoff, COOF.Contains(cardoff));
                if (!(COOF.Contains(cardoff)))
                {
                    COOF.Add(cardoff);
                    summcard++;
                    if (newcard == 0)
                    {
                        AddCardP1(Card, mst);
                        Console.WriteLine("Pleer1 Card: {0} {1}", Card, mst);
                        newcard = 1;
                        message1 += Card + mst + " ";
                        summ1 += Card;
                    }
                    else
                    {
                        AddCardP2(Card, mst);
                        Console.WriteLine("Pleer2 Card: {0} {1}", Card, mst);
                        newcard = 0;
                        message2 += Card + mst + " ";
                        summ2 += Card;
                    }

                }
            }
            CardOff.ForEach(Console.Write);
            Console.WriteLine();
            MastOff.ForEach(Console.Write);
            Console.WriteLine();
            SendMes(message1, message2);
            play();
        }
        public void play()
        {
            int x, y;
            string message, message2;
            
            do
            {
                x = pleer1.Count;
                y = pleer1mast.Count;
                message = "Еще? 1 - да, 2 - хватит";
                SendMes(message, "00");
                message = GetMes1();
                if (message == "1")
                {
                    Card = Random(1);
                    Console.WriteLine("Карта игрока 1: {0} {1} ", pleer1[x], pleer1mast[y]);
                    summ1 += Card;
                    


                    if (summ1 >= 22)
                    {
                        message = "Больше 21: " + summ1 + ". Ожидаем второго игрока";
                       SendMes("00", message);
                        message2 = morethan21(message);
                        summ1 = 0;
                    }

                }
                else
                {
                    message = "У Вас: " + summ1 + ". Ожидаем второго игрока";
                    message = morethan21(message);
                }
            }
            while (message != "2");

            do
            {
                x = pleer2.Count;
                y = pleer2mast.Count;
                message2 = "Еще? 1 - да, 2 - хватит";
                SendMes("00", message2);
                message2 = GetMes2();
                if (message2 == "1")
                {
                    Card = Random(2);
                    Console.WriteLine("Карта игрока 2: {0} {1}", pleer2[x - 1], pleer2mast[y - 1]);
                    summ2 += Card;
                    
                    if (summ2 >= 22)
                    {

                        message2 = "Больше 21: " + summ2 + ".";
                        message = morethan21(message2);
                        summ2 = 0;
                    }

                }
                else
                {
                    message2 = "У Вас: " + summ2 + ".";
                    message2 = morethan21(message2);
                }
            }
            while (message2 != "2");
            Itog(summ1, summ2);
        }
        public string morethan21(string mess)
        {
            String messagemore = mess;
            Console.WriteLine(messagemore);
            messagemore = "2";
            return messagemore;
        }

        public void Itog(int sum1, int sum2)
        {
            int summa1 = sum1;
            int summa2 = sum2;
            if (summa1 > summa2)
            {
                pleerwin1++;
                message1 = "Игрок 1 выйиграл. Счет: " + pleerwin1 + ":" + pleerwin2;
                SendMes(message1, message1);
            }
            else if (summa1 < summa2)
            {
                pleerwin2++;
                message1 = "Игрок 2 выйиграл. Счет: " + pleerwin1 + ":" + pleerwin2;
                SendMes(message1, message1);
            }
            else
            {
                message1 = "Ничья";
                SendMes(message1, message1);
            }
            Restart();
        }
        public void Restart()
        {
            message1 = "Еще раз? 1- Да, 2 - Нет";
            SendMes(message1, "00");
            message1 = GetMes1();
            message2 = "Еще раз? 1- Да, 2 - Нет";
            SendMes("00", message2);
            message2 = GetMes2();
            if ((message1 == "1") && (message2 == "1"))
            {
                Process();
            }
            else
            {
                message1 = "Противник вышел";
                Console.WriteLine(message1);
                SendMes(message1, message1);
                Disconnect();
            }
        }
        protected void AddCardP1(int addcard, string addcard2)
        {
            CardOff.Add(addcard);
            MastOff.Add(addcard2);
            pleer1.Add(addcard);
            pleer1mast.Add(addcard2);
        }
        public int Random(int a)
        {
            do
            {
                Ncard = rnda.Next(chislo.Count);
                Card = chislo[Ncard];
                Mcard = rndb.Next(mast.Count);
                mst = mast[Mcard];
                cardoff = Card + mst;
                //Console.WriteLine("Random 2 test: {0} {1}", cardoff, COOF.Contains(cardoff));
            } while (COOF.Contains(cardoff));
            COOF.Add(cardoff);
            if (a == 1)
            {
                AddCardP1(Card, mst);
                CardOff.ForEach(Console.Write);
                Console.WriteLine();
                MastOff.ForEach(Console.Write);
                Console.WriteLine();
                message1 = Card + mst;
                SendMes(message1, "00");
            }
            else
            {
                AddCardP2(Card, mst);
                CardOff.ForEach(Console.Write);
                Console.WriteLine();
                MastOff.ForEach(Console.Write);
                Console.WriteLine();
                message2 = Card + mst;
                SendMes("00", message2);
            }
            return Card;
        }
        protected void AddCardP2(int addcard, string addcard2)
        {
            CardOff.Add(addcard);
            MastOff.Add(addcard2);
            pleer2.Add(addcard);
            pleer2mast.Add(addcard2);
        }
        public void newstream(TcpClient str1, TcpClient str2)
        {
            stream1 = str1.GetStream();
            stream2 = str2.GetStream();
        }
        public void SendMes(string mes1, string mes2)
        {
            byte[] data1 = new byte[128];
            try
            {
                string message1 = mes1;
                string message2 = mes2;
                if (message1 != "00")
                {
                    Console.WriteLine("Отправка сообщения первому игроку: {0}", message1);
                    data1 = Encoding.Unicode.GetBytes(message1);
                    stream1.Write(data1, 0, data1.Length);
                }
                if (message2 != "00")
                {
                    Console.WriteLine("Отправка сообщения второму игроку: {0}", message2);
                    data1 = Encoding.Unicode.GetBytes(message2);
                    stream2.Write(data1, 0, data1.Length);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                data1 = Encoding.Unicode.GetBytes("Противник вышел");
                try
                {
                    stream1.Write(data1, 0, data1.Length);
                }
                catch (Exception ex2)
                {
                    //Console.WriteLine(ex2.Message);
                    stream1.Close();
                    Plr1.Close();
                }
                try
                {
                    stream2.Write(data1, 0, data1.Length);
                    stream2.Close();
                    Plr2.Close();
                }
                catch (Exception ex3)
                {
                    //Console.WriteLine(ex3.Message);
                }
                Disconnect();
            }

        }

        public void stop()
        {
            Console.ReadLine();
        }
        public string GetMes1()
        {

            StringBuilder builder = new StringBuilder();
            byte[] data2 = new byte[128];
            try
            {

                SendMes("00", "Ожидаем второго игрока.");
                int bytes = 0;
                do
                {
                    bytes = stream1.Read(data2, 0, data2.Length);
                    builder.Append(Encoding.Unicode.GetString(data2, 0, bytes));
                }
                while (stream1.DataAvailable);
                return builder.ToString();

            }

            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                //Console.WriteLine("Error 1");
                message1 = "Противник вышел";
                SendMes(message1, message1);
                Disconnect();
                return null;
            }

        }

        public void Disconnect()
        {
            message1 = "null";
            message2 = "null";

            stream1.Close();
            stream2.Close();
            Plr1.Close();
            Plr2.Close();
            stop();
        }
        public string GetMes2()
        {
            byte[] data2 = new byte[128];
            StringBuilder builder2 = new StringBuilder();
            int bytes1 = 0;
            try
            {
                SendMes("Ожидаем второго игрока.", "00");

                do
                {
                    bytes1 = stream2.Read(data2, 0, data2.Length);
                    builder2.Append(Encoding.Unicode.GetString(data2, 0, bytes1));
                }
                while (stream2.DataAvailable);
                return builder2.ToString();
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                //Console.WriteLine("Error 2");
                message1 = "Противник вышел";
                SendMes(message1, message1);
                Disconnect();
                return null;
            }

        }
    }
}

   
 



    
