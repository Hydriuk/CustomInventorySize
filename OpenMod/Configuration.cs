using CustomInventorySize.API;
using CustomInventorySize.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize.OpenMod
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class Configuration : IConfigurationAdapter
    {
        public bool Enabled { get; set; } = false;
        public List<GroupSizes> Groups { get; set; } = new List<GroupSizes>();

        public Configuration(IConfiguration configurator)
        {
            configurator.Bind(this);
        }
    }
}
