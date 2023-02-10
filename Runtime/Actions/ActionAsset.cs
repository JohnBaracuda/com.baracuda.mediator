using Baracuda.Utilities.Inspector;
using UnityEngine;

namespace Baracuda.Mediator
{
    public abstract class ActionAsset : ScriptableObject
    {
        [Button]
        public abstract void Perform();
    }

    public abstract class ActionAsset<T> : ScriptableObject
    {
        [Button]
        public abstract void Perform(T value);
    }

    public abstract class ActionAsset<T1, T2> : ScriptableObject
    {
        [Button]
        public abstract void Perform(T1 value1, T2 value2);
    }

    public abstract class ActionAsset<T1, T2, T3> : ScriptableObject
    {
        [Button]
        public abstract void Perform(T1 value1, T2 value2, T3 value3);
    }

    public abstract class ActionAsset<T1, T2, T3, T4> : ScriptableObject
    {
        [Button]
        public abstract void Perform(T1 value1, T2 value2, T3 value3, T4 value4);
    }
}
