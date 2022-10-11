using CustomInventorySize.API;
using Rocket.Core.Utils;
using System;

namespace CustomInventorySize.RocketMod.Services
{
    public class ThreadAdapter : IThreadAdapter
    {
        public void RunOnMainThread(Action action) => TaskDispatcher.QueueOnMainThread(action);
    }
}