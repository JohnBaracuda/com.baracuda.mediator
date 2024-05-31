using Baracuda.Bedrock.Mediator;
using System;

namespace Baracuda.Bedrock.Values
{
    /// <summary>
    ///     ReadOnly value asset.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value</typeparam>
    public abstract class ValueAssetRO<TValue> : MediatorAsset
    {
        public TValue Value => GetValue();

        public abstract TValue GetValue();

        public abstract event Action<TValue> Changed;

        public override string ToString()
        {
            return Value?.ToString() ?? base.ToString();
        }

        public static explicit operator TValue(ValueAssetRO<TValue> valueAssetRO)
        {
            return valueAssetRO.Value;
        }
    }
}