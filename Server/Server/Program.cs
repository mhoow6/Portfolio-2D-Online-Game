﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Google.Protobuf.Protocol;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();
        static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>(); // 주기적으로 돌고있는 타이머를 리스트에

        // 이렇게 할 경우 운이 나쁠 경우 tick 이후에 실행될 수 있는 점을 기억하자. 반응속도엔 손해를 봄
        static void TickRoom(Room room, int tick = 100)
        {
            var timer = new System.Timers.Timer();
            timer.Interval = tick; // 몇 틱마다 실행이 될지 설정
            timer.Elapsed += ((s, e) => { room.Update(); }); // 시간이 지났으면 무엇을 실행할 것인지
            timer.AutoReset = true; // 실행하고 자동 리셋
            timer.Enabled = true; // timer 실행시작

            _timers.Add(timer);
        }
        static void Main(string[] args)
        {
            // Data Load
            DataManager.Instance.LoadData();
            Console.WriteLine("Data Load Completed.");

            // TODO: 클라이언트에서 방 생성 요청 받아서 하기
            Room room = RoomManager.Instance.Add((int)MapId.Dungeon);
            Console.WriteLine("Room is Online..!");
            TickRoom(room, 50); // 50ms마다 실행
            

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
