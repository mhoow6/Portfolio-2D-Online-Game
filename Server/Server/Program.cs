using System;
using System.Net;
using System.Threading;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();
        static void Main(string[] args)
        {
            // Data Load
            DataManager.Instance.LoadData();
            Console.WriteLine("Data Load Completed.");

            // Server Setup
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress hostIp = hostEntry.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(hostIp, 7080);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
