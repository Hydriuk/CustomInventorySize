using CustomInventorySize.API;
using Hydriuk.UnturnedModules.Adapters;
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
    internal class ChatMessenger : IChatMessenger
    {
        private readonly ITranslationAdapter _translations;
        private readonly IThreadAdapter _threads;

        public ChatMessenger(ITranslationAdapter translations, IThreadAdapter threads)
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