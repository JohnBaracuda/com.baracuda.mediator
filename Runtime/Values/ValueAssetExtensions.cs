using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Baracuda.Bedrock.Values
{
    public static class ValueAssetExtensions
    {
        public static void SetValueDefaultIfEquals<TValue>(this ValueAsset<TValue> valueAsset, TValue other)
        {
            if (EqualityComparer<TValue>.Default.Equals(valueAsset.Value, other))
            {
                valueAsset.SetValue(default);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFalse(this ValueAssetRW<bool> boolAsset)
        {
            boolAsset.SetValue(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetTrue(this ValueAssetRW<bool> boolAsset)
        {
            boolAsset.SetValue(true);
        }
    }
}