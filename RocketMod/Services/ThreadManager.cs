using LightningStrikes.API;
using Rocket.Core.Utils;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace LightningStrikes.RocketMod.Services
{
    public class ThreadManager : MonoBehaviour, IThreadManager
    {
        public void RunOnMainThread(Action action)
        {
            TaskDispatcher.QueueOnMainThread(action);
        }
    }
}