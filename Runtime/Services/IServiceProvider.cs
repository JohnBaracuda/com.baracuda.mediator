using System;
using System.Collections.Generic;

namespace Baracuda.Mediator.Services
{
    public interface IServiceProvider<out TProvider> where TProvider : IServiceProvider<TProvider>
    {
        IEnumerable<object> Services { get; }

        T Get<T>() where T : class;

        object Get(Type type);

        bool TryGet<T>(out T service) where T : class;

        bool TryGet(Type type, out object value);

        TProvider Register<T>(T service);

        TProvider Register(Type type, object service);

        void AddRegistrationCallback<T>(Action<T> callback, bool callRetroactively = true) where T : class;

        void RemoveRegistrationCallback<T>(Action<T> callback);
    }
}