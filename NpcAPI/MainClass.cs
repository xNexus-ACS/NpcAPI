using System;
using Exiled.API.Features;

namespace NpcAPI
{
    public class MainClass : Plugin<Config>
    {
        public override string Name { get; } = "NpcAPI";
        public override string Author { get; } = "xNexusACS";
        public override string Prefix { get; } = "NpcAPI";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(6, 0, 0);
        
        public static MainClass Instance;
        
        public override void OnEnabled()
        {
            Instance = this;
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            Instance = null;
            base.OnDisabled();
        }
    }
}