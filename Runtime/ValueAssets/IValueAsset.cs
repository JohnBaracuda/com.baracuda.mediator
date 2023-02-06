namespace Baracuda.Mediator
{
    public interface IValueAsset<TValue>
    {
        public TValue Value { get; set; }
        public IReceiver<TValue> Changed { get; }
    }
}