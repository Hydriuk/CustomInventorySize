using CustomInventorySize.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;

namespace CustomInventorySize.OpenMod
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class Configuration : IConfigurationAdapter
    {
        public bool Enabled { get; set; } = false;

        public Configuration(IConfiguration configurator)
        {
            configurator.Bind(this);
        }
    }
}