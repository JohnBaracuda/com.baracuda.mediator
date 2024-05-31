using System.Collections.Generic;

namespace Baracuda.Bedrock.Values
{
    public static class ValueAssetExtensions
    {
        public static void SetValueDefaultIfEquals<TValue>(this ValueAsset<TValue> valueAsset, TValue other)
        {
            if (EqualityComparer<TValue>.Default.Equals(valueAsset.Value, other))
            {
                valueAsset.SetValue(default(TValue));
            }
        }

        public static void SetValueNullIfEquals<TValue>(this ValueAsset<TValue> valueAsset, TValue other)
            where TValue : class
        {
            if (EqualityComparer<TValue>.Default.Equals(valueAsset.Value, other))
            {
                valueAsset.SetValue(null);
            }
        }
    }
}