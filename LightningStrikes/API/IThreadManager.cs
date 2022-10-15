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
        /// <summary>
        /// Executes an action, with a delay
        /// </summary>
        /// <typeparam name="T"> Any object </typeparam>
        /// <param name="action"> Action to execute </param>
        /// <param name="delay"> Delay in seconds before executing the action </param>
        /// <param name="arg"> Parameter of the action </param>
        void Execute<T>(Action<T> action, float delay, T arg);
    }
}