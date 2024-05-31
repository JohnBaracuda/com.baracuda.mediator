namespace Baracuda.Bedrock.Values
{
    public interface IValueAsset<TValue>
    {
        public TValue Value { get; set; }

        public TValue GetValue();

        public void SetValue(TValue value);
    }
}