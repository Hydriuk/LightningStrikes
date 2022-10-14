using Cysharp.Threading.Tasks;
using LightningStrikes.API;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightningStrikes.OpenMod.Services
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class ThreadManager : IThreadManager
    {
        public async void Execute<T>(Action<T> action, float delay, T arg)
        {
            await UniTask.Delay((int)(delay * 1000));

            action(arg);
        }
    }
}
