namespace Baracuda.Mediator.Pooling
{
    public interface IPoolObject
    {
        public void OnGetFromPool();

        public void OnReleaseToPool();
    }
}