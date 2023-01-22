using System;
using UnityEngine;

namespace Baracuda.Mediator
{
    [Serializable]
    public struct Variable<T>
    {
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

        [SerializeField] private bool byReference;
        [SerializeField] private IValueAsset<T> reference;
        [SerializeField] private T value;

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator T(Variable<T> var)
        {
            return var.Value;
        }
    }
}
