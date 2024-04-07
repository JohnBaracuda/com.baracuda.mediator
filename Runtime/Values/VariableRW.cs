using System;
using UnityEngine;

namespace Baracuda.Mediator.Values
{
    /// <summary>
    ///     Proxy variable that either points to a <see cref="ValueAsset{TValue}" /> or a locally serialized value.
    /// </summary>
    [Serializable]
    public struct VariableRW<T>
    {
        [SerializeField] private bool byReference;
        [SerializeField] private ValueAssetRW<T> reference;
        [SerializeField] private T value;

        /// <summary>
        ///     Access the contained value.
        /// </summary>
        public T Value
        {
            get => byReference ? reference.Value : value;
            set
            {
                if (byReference)
                {
                    reference.SetValue(value);
                }
                else
                {
                    this.value = value;
                }
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator T(VariableRW<T> variableRW)
        {
            return variableRW.Value;
        }
    }
}