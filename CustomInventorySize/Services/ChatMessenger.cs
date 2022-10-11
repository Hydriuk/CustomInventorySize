using CustomInventorySize.API;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using UnityEngine;

namespace CustomInventorySize.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class ChatMessenger : IChatMessenger
    {
        private readonly ITranslationsAdapter _translations;
        private readonly IThreadAdapter _threads;

        public ChatMessenger(ITranslationsAdapter translations, IThreadAdapter threads)
        {
            _translations = translations;
            _threads = threads;
        }

        public void WarnInventoryItemDropped(Player player)
        {
            _threads.RunOnMainThread(() =>
                ChatManager.serverSendMessage(
                    _translations["InventoryItemDropped"],
                    Color.yellow,
                    toPlayer: player.channel.owner
                )
            );
        }

        public void WarnStorageItemDropped(Player player)
        {
            _threads.RunOnMainThread(() =>
                ChatManager.serverSendMessage(
                    _translations["StorageItemDropped"],
                    Color.yellow,
                    toPlayer: player.channel.owner
                )
            );
        }
    }
}