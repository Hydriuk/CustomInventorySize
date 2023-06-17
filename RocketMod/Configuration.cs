using CustomInventorySize.API;
using Rocket.API;

namespace CustomInventorySize.RocketMod
{
    public class Configuration : IConfigurationAdapter, IRocketPluginConfiguration
    {
        public bool Enabled { get; set; }

        public void LoadDefaults()
        {
            Enabled = true;
        }
    }
}