using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	class SessionManager
	{
		static SessionManager _session = new SessionManager();
		public static SessionManager Instance { get { return _session; } }

		int _sessionId = 0;
		public Dictionary<int, ClientSession> Sessions { get; private set; } = new Dictionary<int, ClientSession>();
		public Dictionary<int, ClientSession> LobbySessions { get; private set; } = new Dictionary<int, ClientSession>();

		object _lock = new object();

		public ClientSession Generate()
		{
			lock (_lock)
			{
				int sessionId = ++_sessionId;

				ClientSession session = new ClientSession();
				session.SessionId = sessionId;
				Sessions.Add(sessionId, session);
				LobbySessions.Add(sessionId, session);

				Console.WriteLine($"Session Generated. (SessionId: {sessionId})");

				return session;
			}
		}

		public ClientSession Find(int id)
		{
			lock (_lock)
			{
				ClientSession session = null;
				Sessions.TryGetValue(id, out session);
				return session;
			}
		}

		public void Remove(ClientSession session)
		{
			lock (_lock)
			{
				Sessions.Remove(session.SessionId);

				ClientSession client = null;
				if (LobbySessions.TryGetValue(session.SessionId, out client) == true)
                {
					LobbySessions.Remove(client.SessionId);
                }
			}
		}

		public void OutLobby(ClientSession session)
        {
			lock (_lock)
            {
				LobbySessions.Remove(session.SessionId);
			}				
        }
	}
}
