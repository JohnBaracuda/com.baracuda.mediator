using System;
using UnityEngine;

namespace Baracuda.Mediator
{
    /// <summary>
    /// Proxy variable that either points to a <see cref="ValueAsset{TValue}"/> or a locally serialized value.
    /// </summary>
    [Serializable]
    public sealed class Variable<T>
    {
        [SerializeField] private bool byReference;
        [SerializeField] private ValueAsset<T> reference;
        [SerializeField] private T value;

        /// <summary>
        /// Access the contained value.
        /// </summary>
        public T Value
        {
            get => byReference ? reference.Value : value;
            set
            {
                if (byReference)
                {
                    reference.Value = value;
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

        public static implicit operator T(Variable<T> variable)
        {
            return variable.Value;
        }
    }
}
