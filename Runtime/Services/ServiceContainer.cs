using Baracuda.Utilities;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Baracuda.Mediator.Services
{
    public class ServiceContainer : IServiceProvider<ServiceContainer>
    {
        private readonly Dictionary<Type, object> _services = new();

        private readonly Dictionary<Type, List<Delegate>> _registrationCallbacks = new();

        public IEnumerable<object> Services => _services.Values;

        [PublicAPI]
        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var value))
            {
                return value as T;
            }

            throw new ArgumentException($"ServiceContainer.Get: Service of type {type.FullName} not registered");
        }

        [PublicAPI]
        public object Get(Type type)
        {
            if (_services.TryGetValue(type, out var value))
            {
                return value;
            }

            throw new ArgumentException($"ServiceContainer.Get: Service of type {type.FullName} not registered");
        }

        [PublicAPI]
        public bool TryGet<T>(out T service) where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var obj))
            {
                service = obj as T;
                return true;
            }

            service = null;
            return false;
        }

        [PublicAPI]
        public bool TryGet(Type type, out object value)
        {
            if (_services.TryGetValue(type, out value))
            {
                return true;
            }

            return false;
        }

        [PublicAPI]
        public ServiceContainer Register<T>(T service)
        {
            if (!_services.TryAdd(typeof(T), service))
            {
                Debug.LogError(nameof(ServiceContainer),
                    $"ServiceContainer.Register: Service of type {typeof(T).FullName} is already registered!");
            }

            if (_registrationCallbacks.TryGetValue(typeof(T), out var callbacks))
            {
                foreach (var callback in callbacks)
                {
                    var action = (Action<T>) callback;
                    action.Invoke(service);
                }
            }

            return this;
        }

        [PublicAPI]
        public ServiceContainer Remove<T>(T service)
        {
            _services.TryRemove(typeof(T));
            _registrationCallbacks.TryRemove(typeof(T));
            return this;
        }

        [PublicAPI]
        public ServiceContainer Remove<T>()
        {
            _services.TryRemove(typeof(T));
            _registrationCallbacks.TryRemove(typeof(T));
            return this;
        }

        [PublicAPI]
        public ServiceContainer Register(Type type, object service)
        {
            if (!type.IsInstanceOfType(service))
            {
                throw new ArgumentException(
                    "ServiceContainer.Register: Type of service does not match type of service interface",
                    nameof(service));
            }

            if (!_services.TryAdd(type, service))
            {
                Debug.LogError(nameof(ServiceContainer),
                    $"ServiceContainer.Register: Service of type {type.FullName} is already registered!");
            }

            if (_registrationCallbacks.TryGetValue(type, out var callbacks))
            {
                var dynamicArguments = new[] {service};
                foreach (var callback in callbacks)
                {
                    callback.DynamicInvoke(dynamicArguments);
                }
            }

            return this;
        }

        [PublicAPI]
        public void AddRegistrationCallback<T>(Action<T> callback, bool callRetroactively = true) where T : class
        {
            if (_registrationCallbacks.TryGetValue(typeof(T), out var list))
            {
                list.Add(callback);
            }
            else
            {
                _registrationCallbacks.Add(typeof(T), new List<Delegate> {callback});
            }

            if (callRetroactively && TryGet<T>(out var service))
            {
                callback(service);
            }
        }

        [PublicAPI]
        public void RemoveRegistrationCallback<T>(Action<T> callback)
        {
            if (_registrationCallbacks.TryGetValue(typeof(T), out var list))
            {
                list.Remove(callback);
            }
        }
    }
}