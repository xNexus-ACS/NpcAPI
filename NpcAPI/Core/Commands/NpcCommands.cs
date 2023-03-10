using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Mirror;
using PlayerRoles;

namespace NpcAPI.Core.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class NpcCommands : ParentCommand
    {
        public override string Command { get; } = "npc";
        
        // A little bit of trolling here KEKW
        public override string[] Aliases { get; } = null;
        
        public override string Description { get; } = "NpcAPI Commands";
        
        public NpcCommands() => LoadGeneratedCommands();
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Spawn());
            RegisterCommand(new UnSpawn());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "NpcAPI Commands:\n" +
                       "spawn - Spawn an NPC\n" +
                       "unspawn - Destroy an NPC";
            return false;
        }
    }

    public class Spawn : ICommand
    {
        public string Command { get; } = "spawn";
        public string[] Aliases { get; } = null;
        public string Description { get; } = "Spawn an NPC";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("npc.spawn"))
            {
                response = "You don't have permission to use this command";
                return false;
            }

            Player player = Player.Get(sender);
            
            if (arguments.Count < 4)
            {
                response = "Usage: npc spawn <name> <badge> <badgeColor> <id> <roleType>";
                return false;
            }
            
            var name = arguments.At(0);
            var badge = arguments.At(1);
            var badgeColor = arguments.At(2);
            var id = int.Parse(arguments.At(3));

            Npc npc = new Npc(name, badge, badgeColor, player.Position, player.Rotation, id, RoleTypeId.ClassD);
            npc.Spawn();
            
            response = "Spawned NPC";
            return true;
        }
    }

    public class UnSpawn : ICommand
    {
        public string Command { get; } = "unspawn";
        public string[] Aliases { get; } = null;
        public string Description { get; } = "Destroy an NPC";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("npc.unspawn"))
            {
                response = "You don't have permission to use this command";
                return false;
            }
            
            if (arguments.Count < 1)
            {
                response = "Usage: npc unspawn <id>";
                return false;
            }

            int id = int.Parse(arguments.At(0));
            if (Npc.ConnectionsIds.TryGetValue(id, out ReferenceHub hub))
            {
                Npc.Connections.Remove(Npc.Connections.FirstOrDefault(s => s.Value == hub).Key);
                Npc.ConnectionsIds.Remove(id);
                NetworkServer.Destroy(hub.gameObject);
            }
            response = "Destroyed";
            return true;
        }
    }
}