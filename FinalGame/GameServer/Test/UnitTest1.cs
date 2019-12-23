using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Sockets;
using FinalGame21;
namespace Test
{
    [TestClass]
    public class GameTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            TcpListener tcpListener;
            tcpListener = new TcpListener(IPAddress.Any, 8888);
            tcpListener.Start();
            TcpClient Pleer1;
            TcpClient Pleer2;
            Pleer1 = tcpListener.AcceptTcpClient();
            Pleer2 = tcpListener.AcceptTcpClient();
            NetworkStream stream1 = Pleer1.GetStream();
            NetworkStream stream2 = Pleer1.GetStream();
            ClientObject TestGame1 = new ClientObject(Pleer1, Pleer2);
            string message = "2";
            string mass = "Больше";
            string returnmess;
            // Act
            returnmess = TestGame1.morethan21(mass);
            // Assert
            Assert.AreEqual(message, returnmess, "Сообение отправлено.Код получен");
        }
    }
}





