using CustomInventorySize.API;
using Rocket.Core.Utils;
using System;

namespace CustomInventorySize.RocketMod.Services
{
    public class ThreadManager : IThreadManager
    {
        public void RunOnMainThread(Action action) => TaskDispatcher.QueueOnMainThread(action);
    }
}