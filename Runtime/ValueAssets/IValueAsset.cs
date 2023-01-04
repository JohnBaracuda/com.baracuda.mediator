using UnityEngine;

namespace Baracuda.Mediator.ValueAssets
{
    /// <summary>
    /// Scriptable object holding a arg that can be accessed and set.
    /// Object can be subscribed to receive a callback when the arg is changed.
    /// The arg of this object will reset to its inspector arg after runtime.
    /// </summary>
    public abstract class IValueAsset<TValue> : ScriptableObject
    {
        public abstract TValue Value { get; set; }
    }
}