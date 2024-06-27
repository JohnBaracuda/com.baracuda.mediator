using System;
using UnityEngine;

namespace Baracuda.Bedrock.Values
{
    /// <summary>
    ///     Proxy variable that either points to a <see cref="ValueAsset{TValue}" /> or a locally serialized value.
    /// </summary>
    [Serializable]
    public struct VariableRO<T>
    {
        [SerializeField] private bool byReference;
        [SerializeField] private ValueAssetRO<T> reference;
        [SerializeField] private T value;

        /// <summary>
        ///     Access the contained value.
        /// </summary>
        public T Value => byReference ? reference.Value : value;

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator T(VariableRO<T> variableRO)
        {
            return variableRO.Value;
        }

        public static implicit operator VariableRO<T>(T value)
        {
            return new VariableRO<T>
            {
                value = value,
                byReference = false
            };
        }
    }
}