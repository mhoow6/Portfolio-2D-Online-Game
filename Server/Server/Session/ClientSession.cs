using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;

namespace Server
{
	public class ClientSession : PacketSession
	{
		public Player me;

		public int SessionId { get; set; }

		public void Send(IMessage packet)
		{
			// 패킷의 이름 추출 ex. S_Chat
			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);

			// string으로 enum 찾기
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];  // 헤더에 패킷의 총 사이즈랑 패킷의 종류를 붙여야되서 4byte 추가

			// [size(2)][packetId(2)][packet]
			Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort)); // size(2)를 sendBuffer에 추가
			Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));  // packetId(2)를 sendBuffer에 추가
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);  // packet을 sendBuffer에 추가

			Send(new ArraySegment<byte>(sendBuffer));
		}

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			// TODO: DB에서 긁어오자
			PlayerInfo playerStat = DataManager.Instance.GetPlayerData();

			// 오브젝트 기본 정보
			me = ObjectManager.Instance.Add<Player>((ObjectCode)playerStat.code);
			me.session = this;

			// 랜덤 스폰 장소
			Random rnd = new Random(System.Environment.TickCount);
			int rndIndex = -1;
			SpawnPosInfo pos = SpawnPosInfo.Zero;
			rndIndex = rnd.Next(0, DataManager.Instance.DungeonPlayerSpawnPosition.Count - 1);
			pos = DataManager.Instance.DungeonPlayerSpawnPosition[rndIndex];
			Vector2 position = new Vector2();
			position.X = pos.x;
			position.Y = pos.y;
			me.objectInfo.Position = position;

			// 방 번호
			Room room = RoomManager.Instance.Find(1);
			me.room = room;
			me.objectInfo.RoomId = me.room.roomId;

			// 기본 상태, 이동방향
			me.objectInfo.MoveDir = MoveDir.Up;
			me.objectInfo.State = State.Idle;

			// 게임에서 쓰일 스텟정보 초기화
			me.objectInfo.Stat = new StatInfo() { Hp = playerStat.hp, Movespeed = playerStat.movespeed, WeaponId = playerStat.weaponId };
			
			room.Push(room.C_EnterGame, me);
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			SessionManager.Instance.Remove(this);

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			
		}
	}
}
