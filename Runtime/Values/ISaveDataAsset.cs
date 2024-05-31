namespace Baracuda.Bedrock.Values
{
    public interface ISaveDataAsset<TValue> : IValueAsset<TValue>
    {
    }

    public interface ISaveDataAsset
    {
        public void ResetPersistentData();
    }
}