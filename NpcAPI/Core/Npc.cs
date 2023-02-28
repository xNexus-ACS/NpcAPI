using System.Collections.Generic;
using Exiled.API.Features;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace NpcAPI.Core
{
    public class Npc
    {
        public Npc()
        {
        }

        private GameObject _npc;

        private FakeConnection FakeConnection { get; set; }

        public static Dictionary<FakeConnection, ReferenceHub> Connections = new();
        public static Dictionary<int, ReferenceHub> ConnectionsIds = new();
        
        private Player Player { get; }

        public Npc(string name, string badge, string badgeColor, Vector3 position, Vector3 rotation, int id, RoleTypeId roleType = RoleTypeId.CustomRole)
        {
            // Instantiating the player prefab and getting the ReferenceHub for the NPC
            _npc = Object.Instantiate(NetworkManager.singleton.playerPrefab);

            var hub = _npc.GetComponent<ReferenceHub>();
            
            // Creating the Fake Connection
            FakeConnection = new FakeConnection(id);

            NetworkServer.AddPlayerForConnection(FakeConnection, _npc);
            
            // Setting the Instance Mode and ID to Unverified to allow the NPC to join the server
            try
            {
                hub.characterClassManager.UserId = $"npc{id}@server";
            }
            catch
            {
            }
            hub.characterClassManager.InstanceMode = ClientInstanceMode.Unverified;
            
            // Setting the NPCs name, badge, badge color, role, position and rotation
            hub.nicknameSync.SetNick(name);
            
            hub.serverRoles.SetText(badge);
            hub.serverRoles.SetColor(badgeColor);

            hub.roleManager.ServerSetRole(roleType, RoleChangeReason.RemoteAdmin);
            
            hub.TryOverridePosition(position, rotation);
            
            hub.characterClassManager.GodMode = true;
            
            Connections.Add(FakeConnection, hub);
            ConnectionsIds.Add(FakeConnection.connectionId, hub);

            Log.Info($"Spawned NPC with name {name} and ID {FakeConnection.connectionId}");
        }

        public void Spawn()
            => NetworkServer.Spawn(_npc);

        public void UnSpawn()
        {
            FakeConnection.Disconnect();
            NetworkServer.Destroy(_npc);
            Log.Info($"UnSpawned NPC with ID {FakeConnection.connectionId}");
        }
        
        public void ShowNpc() => Spawn();
        
        public void Disconnect() => UnSpawn();
        
        public void HideFromPlayer(Player player) => Player.NetworkIdentity.HideForPlayer(player);

        public void ShowPlayer(Player player) => Player.NetworkIdentity.ShowForPlayer(player);
        
        // create a Get method to get the NPC from the ID without creating a new NPC
        public static Npc Get(int id)
        {
            if (ConnectionsIds.TryGetValue(id, out ReferenceHub hub))
            {
                return ConnectionsIds.ContainsKey(id) ? new Npc() : null;
            }

            return null;
        }
    }
}