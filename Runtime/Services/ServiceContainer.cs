using Baracuda.Bedrock.Injection;
using Baracuda.Utilities;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Baracuda.Bedrock.Services
{
    public class ServiceContainer
    {
        #region Fields

        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, Delegate> _transientServices = new();
        private readonly Dictionary<Type, Delegate> _lazyServices = new();
        private readonly HashSet<Type> _registeredServiceTypes = new();

        private readonly LogCategory _category = nameof(ServiceContainer);

        #endregion


        #region Public API: Get

        [PublicAPI]
        public IEnumerable<object> GetAllServices()
        {
            return _services.Values;
        }

        [PublicAPI]
        public T Resolve<T>() where T : class
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif

            var type = typeof(T);
            if (_services.TryGetValue(type, out var value))
            {
                return value as T;
            }

            if (_lazyServices.TryGetValue(type, out var lazyFunc))
            {
                var service = lazyFunc.CastExplicit<Func<T>>()();
                _services.Add(type, service);
                _lazyServices.Remove(type);
                return service;
            }

            if (_transientServices.TryGetValue(type, out var transientFunc))
            {
                return transientFunc.CastExplicit<Func<T>>()();
            }

            Debug.LogWarning(_category, $"Service of type {type.FullName} not registered");
            return null;
        }

        [PublicAPI]
        public object Resolve(Type type)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif

            if (_services.TryGetValue(type, out var value))
            {
                return value;
            }

            if (_lazyServices.TryGetValue(type, out var lazyFunc))
            {
                var service = lazyFunc.DynamicInvoke();
                Inject.Dependencies(service);
                _services.Add(type, service);
                _lazyServices.Remove(type);
                return service;
            }

            if (_transientServices.TryGetValue(type, out var transientFunc))
            {
                return transientFunc.DynamicInvoke();
            }

            Debug.LogWarning(_category, $"Service of type {type.FullName} not registered");
            return null;
        }

        #endregion


        #region Public API: Try Get

        [PublicAPI]
        public bool TryResolve<T>(out T service) where T : class
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif
            var type = typeof(T);
            if (_services.TryGetValue(type, out var value))
            {
                service = value as T;
                return true;
            }

            if (_lazyServices.TryGetValue(type, out var lazyFunc))
            {
                var result = lazyFunc.CastExplicit<Func<T>>()();
                Inject.Dependencies(result);
                _services.Add(type, result);
                _lazyServices.Remove(type);
                service = result;
                return true;
            }

            if (_transientServices.TryGetValue(type, out var transientFunc))
            {
                service = transientFunc.CastExplicit<Func<T>>()();
                return true;
            }

            service = default(T);
            return false;
        }

        [PublicAPI]
        public bool TryResolve(Type type, out object service)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif
            if (_services.TryGetValue(type, out var value))
            {
                service = value;
                return true;
            }

            if (_lazyServices.TryGetValue(type, out var lazyFunc))
            {
                var result = lazyFunc.DynamicInvoke();
                _services.Add(type, result);
                _lazyServices.Remove(type);
                service = result;
                return true;
            }

            if (_transientServices.TryGetValue(type, out var transientFunc))
            {
                service = transientFunc.DynamicInvoke();
                return true;
            }

            service = default(object);
            return false;
        }

        #endregion


        #region Public API: Add Singleton

        [PublicAPI]
        public ServiceContainer Add<T>(T service)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif
            if (!Application.isPlaying)
            {
                return null;
            }

            var type = typeof(T);

            if (!_registeredServiceTypes.Add(type))
            {
                Debug.LogWarning(_category, $"Service of type {type.FullName} is already registered!");
                return this;
            }

            _services.Add(typeof(T), service);

            return this;
        }

        [PublicAPI]
        public ServiceContainer Add(Type type, object service)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif

            if (!type.IsInstanceOfType(service))
            {
                Debug.LogWarning(_category, "Type of service does not match type of service interface!");
                return this;
            }

            if (!_registeredServiceTypes.Add(type))
            {
                Debug.LogWarning(_category, $"Service of type {type.FullName} is already registered!");
                return this;
            }

            _services.Add(type, service);

            return this;
        }

        [PublicAPI]
        public ServiceContainer AddTransient<T>(Func<T> func)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif

            var type = typeof(T);

            if (!_registeredServiceTypes.Add(type))
            {
                Debug.LogWarning(_category, $"Service of type {type.FullName} is already registered!");
                return this;
            }

            _transientServices.Add(type, func);

            return this;
        }

        [PublicAPI]
        public ServiceContainer AddTransient(Type type, Delegate func)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif

            if (!_registeredServiceTypes.Add(type))
            {
                Debug.LogWarning(_category, $"Service of type {type.FullName} is already registered!");
                return this;
            }

            if (func.Method.ReturnType != type)
            {
                Debug.LogWarning(_category, "Delegate of transient service is invalid!");
                return this;
            }

            _transientServices.Add(type, func);

            return this;
        }

        [PublicAPI]
        public ServiceContainer AddLazy<T>(Func<T> func)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif

            var type = typeof(T);

            if (!_registeredServiceTypes.Add(type))
            {
                Debug.LogWarning(_category, $"Service of type {type.FullName} is already registered!");
                return this;
            }

            _lazyServices.Add(type, func);

            return this;
        }

        [PublicAPI]
        public ServiceContainer AddLazy(Type type, Delegate func)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif

            if (!_registeredServiceTypes.Add(type))
            {
                Debug.LogWarning(_category, $"Service of type {type.FullName} is already registered!");
                return this;
            }

            if (func.Method.ReturnType != type)
            {
                Debug.LogWarning(_category, "Delegate of transient service is invalid!");
                return this;
            }

            _lazyServices.Add(type, func);

            return this;
        }

        #endregion


        #region Public API: Remove

        [PublicAPI]
        public ServiceContainer Remove<T>(T service)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif
            var type = typeof(T);

            if (!_registeredServiceTypes.Remove(type))
            {
                return this;
            }

            _services.TryRemove(type);
            _transientServices.TryRemove(type);
            _lazyServices.TryRemove(type);
            return this;
        }

        [PublicAPI]
        public ServiceContainer Remove<T>()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif
            var type = typeof(T);

            if (!_registeredServiceTypes.Remove(type))
            {
                return this;
            }

            _services.TryRemove(type);
            _transientServices.TryRemove(type);
            _lazyServices.TryRemove(type);
            return this;
        }

        #endregion
    }
}