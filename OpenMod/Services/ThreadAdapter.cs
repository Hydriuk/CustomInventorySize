using CustomInventorySize.API;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using System;

namespace CustomInventorySize.OpenMod.Services
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class ThreadAdapter : IThreadAdapter
    {
        public async void RunOnMainThread(Action action)
        {
            await UniTask.SwitchToMainThread();

            action();
        }
    }
}