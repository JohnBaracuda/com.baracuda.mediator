using System;

namespace Baracuda.Mediator
{
    public readonly struct LambdaHandle : IDisposable
    {
        private readonly Action dispose;

        public LambdaHandle(Action dispose)
        {
            this.dispose = dispose;
        }

        public void Dispose()
        {
            dispose();
        }
    }
}