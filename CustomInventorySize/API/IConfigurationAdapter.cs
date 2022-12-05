using CustomInventorySize.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CustomInventorySize.API
{
#if OPENMOD
    [Service]
#endif
    public interface IConfigurationAdapter
    {
        public bool Enabled { get; set; }
        public List<GroupSizes> Groups { get; set; }
    }
}
