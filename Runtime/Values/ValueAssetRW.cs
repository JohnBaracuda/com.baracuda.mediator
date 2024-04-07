namespace Baracuda.Mediator.Values
{
    /// <summary>
    ///     Read & Write value asset.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value</typeparam>
    public abstract class ValueAssetRW<TValue> : ValueAssetRO<TValue>
    {
        public new abstract TValue Value { get; set; }

        public abstract void SetValue(TValue value);
    }
}