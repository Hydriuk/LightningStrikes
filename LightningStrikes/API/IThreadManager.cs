#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;

namespace LightningStrikes.API
{
#if OPENMOD
    [Service]
#endif
    public interface IThreadManager
    {
        void RunOnMainThread(Action action);
    }
}