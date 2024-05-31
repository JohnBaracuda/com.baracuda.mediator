using System;

namespace Baracuda.Bedrock.Locks
{
    public interface ILock
    {
        /// <summary>
        ///     Event is invoked when the first instance is added.
        /// </summary>
        event Action FirstAdded;
        /// <summary>
        ///     Event is invoked when all instances were removed.
        /// </summary>
        event Action LastRemoved;

        /// <summary>
        ///     Returns true if any object is currently registered.
        /// </summary>
        bool HasAny();

        /// <summary>
        ///     Returns true if the passed object is registered.
        /// </summary>
        bool IsObjectRegistered(object instance);

        /// <summary>
        ///     AddSingleton a new object to the list of locks. An object can only be added once as a lock!
        /// </summary>
        /// <returns>true if the object was added, false if it was already added</returns>
        bool Add(object lockInstance);

        /// <summary>
        ///     Remove an object from the list of locks. An object can only be added once as a lock!
        /// </summary>
        /// <returns>true if the object was removed, false if it was not an active lock</returns>
        bool Remove(object instance);

        /// <summary>
        ///     Remove all providing objects and release the lock.
        /// </summary>
        /// <param name="discrete">When true, no events are raised.</param>
        /// <returns>the amount of removed provider</returns>
        int ReleaseAll(bool discrete = false);
    }
}