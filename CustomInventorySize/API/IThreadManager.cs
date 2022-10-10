#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomInventorySize.API
{
#if OPENMOD
    [Service]
#endif
    public interface IThreadManager
    {
        void RunOnMainThread(Action action);
    }
}
