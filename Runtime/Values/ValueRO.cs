using Baracuda.Mediator.ValueAssets;
using System;
using UnityEngine;

namespace Baracuda.Mediator
{
    [Serializable]
    public struct ValueRO<T>
    {
        public T Value => byReference ? reference.Value : value;

        [SerializeField] private bool byReference;
        [SerializeField] private IValueAsset<T> reference;
        [SerializeField] private T value;

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}