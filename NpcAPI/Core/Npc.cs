using System.Collections.Generic;
using Exiled.API.Features;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using UnityEngine;

namespace NpcAPI.Core
{
    public class Npc : MonoBehaviour
    {
        public Npc()
        {
        }

        private GameObject _npc;

        private FakeConnection FakeConnection { get; set; }
        
        private static List<int> _npcIds = new();
        private static List<string> _npcNames = new();

        public Npc(string name, string badge, string badgeColor, RoleTypeId roleType, Vector3 position, Vector3 rotation)
        {
            // Instantiating the player prefab and getting the ReferenceHub for the NPC
            _npc = Instantiate(NetworkManager.singleton.playerPrefab);
            var hub = _npc.GetComponent<ReferenceHub>();
            
            // Creating the Fake Connection
            FakeConnection = new FakeConnection(hub._playerId);
            NetworkServer.AddPlayerForConnection(FakeConnection, _npc);
            
            // Setting the Instance Mode to Unverified to allow the NPC to join the server
            hub.characterClassManager.InstanceMode = ClientInstanceMode.Unverified;
            
            // Setting the NPCs name, badge, badge color, role, position and rotation
            hub.nicknameSync.Network_myNickSync = name;
            hub.serverRoles.SetText(badge);
            hub.serverRoles.SetColor(badgeColor);
            hub.roleManager.ServerSetRole(roleType, RoleChangeReason.RemoteAdmin);
            hub.TryOverridePosition(position, rotation);
            
            hub.playerStats.GetModule<HealthStat>().CurValue = 100;
            hub.characterClassManager.GodMode = true;

            _npcIds.Add(FakeConnection.connectionId);
            _npcNames.Add(name);

            Log.Info($"Spawned NPC with name {name} and ID {FakeConnection.connectionId}");
        }

        public void Spawn()
            => NetworkServer.Spawn(_npc);

        public void Destroy()
        {
            FakeConnection.Disconnect();
            NetworkServer.UnSpawn(_npc);
            Destroy(this);
        }

        public static Npc Get(int id)
        {
            if (_npcIds.Contains(id))
            {
                var npcId = _npcIds.Find(x => x == id);
                return npcId != 0 ? new Npc {FakeConnection = new FakeConnection(new RecyclablePlayerId(npcId))} : null;
            }

            return null;
        }
        
        public static Npc Get(string name)
        {
            if (_npcNames.Contains(name))
            {
                var npcName = _npcNames.Find(x => x == name);
                return npcName != null ? Get(_npcIds[_npcNames.IndexOf(npcName)]) : null;
            }

            return null;
        }
    }
}