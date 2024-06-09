using Cysharp.Threading.Tasks;
using LightningStrikes.API;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using System;

namespace LightningStrikes.OpenMod.Services
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class ThreadManager : IThreadManager
    {
        public async void RunOnMainThread(Action action)
        {
            await UniTask.SwitchToMainThread();

            action();
        }
    }
}