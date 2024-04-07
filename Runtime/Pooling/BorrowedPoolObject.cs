namespace Baracuda.Mediator.Pooling
{
    public readonly struct BorrowedPoolObject<T> where T : class
    {
        public readonly float ReleaseTimeStamp;
        public readonly T Object;

        public BorrowedPoolObject(float releaseTimeStamp, T tracked)
        {
            ReleaseTimeStamp = releaseTimeStamp;
            Object = tracked;
        }
    }
}