namespace Baracuda.Mediator.ValueObjects.Abstraction
{
    public interface __IValueObject
    {
#if UNITY_EDITOR
        public object GetRawValue();
        public void ResetValue();
#endif
    }
}