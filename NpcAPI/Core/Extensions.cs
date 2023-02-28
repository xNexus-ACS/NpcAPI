using Exiled.API.Features;
using Mirror;

namespace NpcAPI.Core
{
    public static class Extensions
    {
        public static void HideForPlayer(this NetworkIdentity identity, Player player)
        {
            var msg = new ObjectDestroyMessage { netId = identity.netId };
            player.Connection.Send(msg);
        }

        public static void ShowForPlayer(this NetworkIdentity identity, Player player)
            => player.Connection.Send(GetSpawnMessage(identity, player));
    
        public static void HideForAllPlayers(this NetworkIdentity identity)
        {
            var msg = new ObjectDestroyMessage { netId = identity.netId };
            NetworkServer.SendToAll(msg);
        }

        private static SpawnMessage GetSpawnMessage(NetworkIdentity identity, Player playerToReceive)
        {
            var writer = NetworkWriterPool.GetWriter();
            var writer2 = NetworkWriterPool.GetWriter();
            var isOwner = identity.connectionToClient == playerToReceive.Connection;
            var payload = NetworkServer.CreateSpawnMessagePayload(isOwner, identity, writer, writer2);
            var transform = identity.transform;
            var msg = new SpawnMessage
            {
                netId = identity.netId,
                isLocalPlayer = playerToReceive.NetworkIdentity == identity,
                isOwner = isOwner,
                sceneId = identity.sceneId,
                assetId = identity.assetId,
                position = transform.position,
                rotation = transform.rotation,
                scale = transform.localScale,
                payload = payload
            };
            return msg;
        }
    }
}