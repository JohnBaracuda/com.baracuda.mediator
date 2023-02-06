using System;
using UnityEngine;

namespace Baracuda.Mediator
{
    [Serializable]
    public struct VariableRO<T>
    {
        public T Value => byReference ? reference.Value : value;

        [SerializeField] private bool byReference;
        [SerializeField] private ValueAsset<T> reference;
        [SerializeField] private T value;

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator T(VariableRO<T> var)
        {
            return var.Value;
        }
    }
}