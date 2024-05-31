using System;

namespace Baracuda.Bedrock.Values
{
    public interface IPropertyAsset<TValue> : IValueAsset<TValue>
    {
        void BindGetter(Func<TValue> getter);

        void ReleaseGetter(Func<TValue> getter);

        void BindSetter(Action<TValue> setter);

        void ReleaseSetter(Action<TValue> setter);
    }
}