namespace Baracuda.Mediator
{
    public abstract class BroadcastAsset : EventAsset, IBroadcast, IReceiver
    {
        public void Raise()
        {
            Event.Raise();
        }
    }

    public abstract class BroadcastAsset<T> : EventAsset<T>, IBroadcast<T>
    {
        public void Raise(T value)
        {
            Event.Raise(value);
        }
    }

    public abstract class BroadcastAsset<T1, T2> : EventAsset<T1, T2>, IBroadcast<T1, T2>
    {
        public void Raise(T1 value1, T2 value2)
        {
            Event.Raise(value1, value2);
        }
    }

    public abstract class BroadcastAsset<T1, T2, T3> : EventAsset<T1, T2, T3>, IBroadcast<T1, T2, T3>
    {
        public void Raise(T1 value1, T2 value2, T3 value3)
        {
            Event.Raise(value1, value2, value3);
        }
    }

    public abstract class BroadcastAsset<T1, T2, T3, T4> : EventAsset<T1, T2, T3, T4>, IBroadcast<T1, T2, T3, T4>
    {
        public void Raise(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            Event.Raise(value1, value2, value3, value4);
        }
    }
}