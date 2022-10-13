#if OPENMOD
using OpenMod.API.Ioc;
#endif

namespace CustomInventorySize.API
{
#if OPENMOD
    [Service]
#endif
    public interface ITranslationsAdapter
    {
        string this[string key] { get; }

        string this[string key, object arguments] { get; }
    }
}