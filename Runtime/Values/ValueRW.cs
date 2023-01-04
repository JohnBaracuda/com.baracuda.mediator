using Baracuda.Mediator.ValueAssets;
using System;
using UnityEngine;

namespace Baracuda.Mediator
{
    [Serializable]
    public struct ValueRW<T>
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
    }
}
