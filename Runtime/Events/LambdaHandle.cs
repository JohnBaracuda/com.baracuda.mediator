using System;

namespace Baracuda.Bedrock.Events
{
    public readonly struct LambdaHandle : IDisposable
    {
        private readonly Action _dispose;

        public LambdaHandle(Action dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            _dispose();
        }
    }
}