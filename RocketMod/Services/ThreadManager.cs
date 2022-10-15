using LightningStrikes.API;
using Rocket.Core.Utils;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace LightningStrikes.RocketMod.Services
{
    public class ThreadManager : MonoBehaviour, IThreadManager
    {
        public void Execute<T>(Action<T> action, float delay, T arg)
        {
            TaskDispatcher.RunAsync(async () =>
            {
                await Task.Delay((int)(delay * 1000));

                action(arg);
            });
        }
    }
}