using System;
using Mirror;

namespace NpcAPI.Core
{
    public class FakeConnection : NetworkConnectionToClient
    {
        public RecyclablePlayerId PlayerID { get; }

        public FakeConnection(RecyclablePlayerId id) : base(id.Value, false, 0f)
        {
            PlayerID = id;
        }

        public override string address => "npc";

        public override void Send(ArraySegment<byte> segment, int channelId = 0) { }
    }
}