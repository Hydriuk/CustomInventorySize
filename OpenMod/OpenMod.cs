using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("CustomInventorySize", DisplayName = "CustomInventorySize")]

namespace CustomInventorySize.OpenMod
{
    public class MyOpenModPlugin : OpenModUnturnedPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly ILogger<MyOpenModPlugin> m_Logger;

        public MyOpenModPlugin(
            IConfiguration configuration,
            IStringLocalizer stringLocalizer,
            ILogger<MyOpenModPlugin> logger,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_Configuration = configuration;
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
        }

        protected override async UniTask OnLoadAsync()
        {
            // await UniTask.SwitchToMainThread(); uncomment if you have to access Unturned or UnityEngine APIs
            m_Logger.LogInformation("Hello World!");

            // await UniTask.SwitchToThreadPool(); // you can switch back to a different thread
        }

        protected override async UniTask OnUnloadAsync()
        {
            // await UniTask.SwitchToMainThread(); uncomment if you have to access Unturned or UnityEngine APIs
            m_Logger.LogInformation(m_StringLocalizer["plugin_events:plugin_stop"]);
        }
    }
}

[Service]
public interface test
{

}