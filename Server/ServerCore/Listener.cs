using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            #region SocketType.Stream 설명
            /* 
             * 스트림소켓은 양방향으로 바이트 스트림을 전송 할 수 있는 연결 지향형 소켓
             * 오류수정, 전송처리, 흐름제어 등을 보장해 주며 송신된 순서에 따른 중복되지 않은 데이터를 수신
             * 각 메시지를 보내기 위해 별도의 연결을 맺는 행위를 하므로 약간의 오버헤드가 존재
             */
            #endregion
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            // 클라이언트가 서버를 찾는 데 사용할 수 있는 주소를 설정
            _listenSocket.Bind(endPoint);

            #region 클라이언트가 서비스를 요청하기를 기다립니다.
            // backlog는 들어오는 연결 수를 큐안에 얼만큼 집어넣을 건지 정하는 수 (대기수)
            #endregion
            _listenSocket.Listen(backlog);

            // 클라이언트 요청을 register 만큼 받아들인다.
            for (int i = 0; i < register; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();

                // 콜백함수는 별도의 쓰레드풀에서 쓰레드를 가져와서 클라이언트가 Connect될때까지 기다리다 호출한다.
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccepts(args);
            }
        }

        void RegisterAccepts(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);

            // pending(기다림)이 없다면 바로 OnAcceptCompleted 호출
            if (pending == false)
                OnAcceptCompleted(null, args);
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                // Accept가 완료되었으니, 인자로 넘겨받은 세션을 실행시킨다
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            // A 클라이언트가 Accept가 완료되었으니 B 클라이언트를 Accept하러 가겠다.
            RegisterAccepts(args);
        }
    }
}
