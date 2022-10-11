using CustomInventorySize.API;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize.OpenMod.Services
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class TranslationsAdapter : ITranslationsAdapter
    {
        private readonly IStringLocalizer _translations;

        public TranslationsAdapter(IStringLocalizer translations)
        {
            _translations = translations;
        }

        public string this[string key] => _translations[key];

        public string this[string key, object arguments] => _translations[key, arguments];
    }
}
