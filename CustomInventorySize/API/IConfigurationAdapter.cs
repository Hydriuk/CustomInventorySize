#if OPENMOD
using OpenMod.API.Ioc;
#endif

namespace CustomInventorySize.API
{
#if OPENMOD
    [Service]
#endif
    public interface IConfigurationAdapter
    {
        public bool Enabled { get; set; }
    }
}
