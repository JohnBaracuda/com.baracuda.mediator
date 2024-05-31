using Baracuda.Bedrock.Services;
using Baracuda.Utilities.Reflection;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Profiling;

namespace Baracuda.Bedrock.Injection
{
    public static class Inject
    {
        #region Fields

        private static readonly Dictionary<Type, List<InjectionCache<object>>> fieldCache = new();
        private static readonly Dictionary<Type, List<InjectionCache<object>>> propertyCache = new();
        private static readonly Dictionary<Type, List<InjectionCache<MonoBehaviour>>> monoFieldCache = new();
        private static readonly Dictionary<Type, List<InjectionCache<MonoBehaviour>>> monoPropertyCache = new();

#if UNITY_EDITOR

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetInjectionCache()
        {
            fieldCache.Clear();
            propertyCache.Clear();
            monoFieldCache.Clear();
            monoPropertyCache.Clear();
        }

#endif

        private struct InjectionCache<T>
        {
            public void Inject(T target)
            {
                InjectDelegate(target);
            }

            public Action<T> InjectDelegate;
        }

        private const BindingFlags Flags = BindingFlags.Instance |
                                           BindingFlags.Static |
                                           BindingFlags.Public |
                                           BindingFlags.NonPublic |
                                           BindingFlags.FlattenHierarchy;

        #endregion


        #region Public API:

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dependencies([NotNull] object target, bool createTypeCache = true)
        {
            var type = target.GetType();
            if (target is MonoBehaviour monoBehaviour)
            {
                InjectFieldsInternal(monoBehaviour, type, createTypeCache);
                InjectPropertiesInternal(monoBehaviour, type, createTypeCache);
                PostInjection(monoBehaviour);
                return;
            }
            InjectFieldsInternal(target, type, createTypeCache);
            InjectPropertiesInternal(target, type, createTypeCache);
            PostInjection(target);
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fields([NotNull] object target, bool createTypeCache = true)
        {
            var type = target.GetType();
            if (target is MonoBehaviour monoBehaviour)
            {
                InjectFieldsInternal(monoBehaviour, type, createTypeCache);
                PostInjection(monoBehaviour);
                return;
            }
            InjectFieldsInternal(target, type, createTypeCache);
            PostInjection(target);
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Properties([NotNull] object target, bool createTypeCache = true)
        {
            var type = target.GetType();
            if (target is MonoBehaviour monoBehaviour)
            {
                InjectPropertiesInternal(monoBehaviour, type);
                PostInjection(monoBehaviour);
                return;
            }
            InjectPropertiesInternal(target, type);
            PostInjection(target);
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dependencies([NotNull] MonoBehaviour target, bool createTypeCache = true)
        {
            var type = target.GetType();
            InjectFieldsInternal(target, type, createTypeCache);
            InjectPropertiesInternal(target, type, createTypeCache);
            PostInjection(target);
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fields([NotNull] MonoBehaviour target, bool createTypeCache = true)
        {
            var type = target.GetType();
            InjectFieldsInternal(target, type, createTypeCache);
            PostInjection(target);
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Properties([NotNull] MonoBehaviour target, bool createTypeCache = true)
        {
            var type = target.GetType();
            InjectPropertiesInternal(target, type);
            PostInjection(target);
        }

        #endregion


        #region Post Injection

        private static void PostInjection(object target)
        {
            if (target is IInjectCallback injectCallback)
            {
                injectCallback.OnDependenciesInjected();
            }
        }

        #endregion


        #region Dependencies: Object

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InjectFieldsInternal([NotNull] object target, Type type, bool createTypeCache)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Profiler.BeginSample("Dependency Injection");

            if (createTypeCache)
            {
                if (!fieldCache.TryGetValue(type, out var profiles))
                {
                    profiles = new List<InjectionCache<object>>();
                    CreateFieldProfiles(type, ref profiles);
                    fieldCache.Add(type, profiles);
                }

                foreach (var injectionCache in profiles)
                {
                    injectionCache.Inject(target);
                }
            }
            else
            {
                var profiles = ListPool<InjectionCache<object>>.Get();
                CreateFieldProfiles(type, ref profiles);
                foreach (var injectionCache in profiles)
                {
                    injectionCache.Inject(target);
                }
                ListPool<InjectionCache<object>>.Release(profiles);
            }

            Profiler.EndSample();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InjectPropertiesInternal([NotNull] object target, Type type, bool createTypeCache = false)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Profiler.BeginSample("Dependency Injection");

            if (createTypeCache)
            {
                if (!propertyCache.TryGetValue(type, out var profiles))
                {
                    profiles = new List<InjectionCache<object>>();
                    CreatePropertyProfiles(type, ref profiles);
                    propertyCache.Add(type, profiles);
                }

                foreach (var injectionCache in profiles)
                {
                    injectionCache.Inject(target);
                }
            }
            else
            {
                var profiles = ListPool<InjectionCache<object>>.Get();
                CreatePropertyProfiles(type, ref profiles);
                foreach (var injectionCache in profiles)
                {
                    injectionCache.Inject(target);
                }
                ListPool<InjectionCache<object>>.Release(profiles);
            }

            Profiler.EndSample();
        }

        #endregion


        #region Dependencies: MonoBehaviour

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InjectFieldsInternal([NotNull] MonoBehaviour target, Type type, bool createTypeCache)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Profiler.BeginSample("Dependency Injection");

            if (createTypeCache)
            {
                if (!monoFieldCache.TryGetValue(type, out var profiles))
                {
                    profiles = new List<InjectionCache<MonoBehaviour>>();
                    CreateFieldProfiles(type, ref profiles);
                    monoFieldCache.Add(type, profiles);
                }

                foreach (var injectionCache in profiles)
                {
                    injectionCache.Inject(target);
                }
            }
            else
            {
                var profiles = ListPool<InjectionCache<MonoBehaviour>>.Get();
                CreateFieldProfiles(type, ref profiles);
                foreach (var injectionCache in profiles)
                {
                    injectionCache.Inject(target);
                }
                ListPool<InjectionCache<MonoBehaviour>>.Release(profiles);
            }

            Profiler.EndSample();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InjectPropertiesInternal([NotNull] MonoBehaviour target, Type type,
            bool createTypeCache = false)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Profiler.BeginSample("Dependency Injection");

            if (createTypeCache)
            {
                if (!monoPropertyCache.TryGetValue(type, out var profiles))
                {
                    profiles = new List<InjectionCache<MonoBehaviour>>();
                    CreatePropertyProfiles(type, ref profiles);
                    monoPropertyCache.Add(type, profiles);
                }

                foreach (var injectionCache in profiles)
                {
                    injectionCache.Inject(target);
                }
            }
            else
            {
                var profiles = ListPool<InjectionCache<MonoBehaviour>>.Get();
                CreatePropertyProfiles(type, ref profiles);
                foreach (var injectionCache in profiles)
                {
                    injectionCache.Inject(target);
                }
                ListPool<InjectionCache<MonoBehaviour>>.Release(profiles);
            }

            Profiler.EndSample();
        }

        #endregion


        #region Reflection Profiles: Object

        private static void CreatePropertyProfiles(Type type, ref List<InjectionCache<object>> profiles)
        {
            var properties = type.GetProperties(Flags);

            foreach (var propertyInfo in properties)
            {
                if (!propertyInfo.TryGetCustomAttribute<InjectAttribute>(out var injectAttribute))
                {
                    continue;
                }

                var serviceLocator = injectAttribute.Scope is null
                    ? ServiceLocator.Global
                    : ServiceLocator.ForScope(injectAttribute.Scope);

                var dependencyType = propertyInfo.PropertyType;

                if (propertyInfo.CanWrite)
                {
                    var profile = new InjectionCache<object>
                    {
                        InjectDelegate = target => propertyInfo.SetValue(target, serviceLocator.Resolve(dependencyType))
                    };
                    profiles.Add(profile);
                }
                else if (propertyInfo.TryGetBackingField(out var backingField))
                {
                    var profile = new InjectionCache<object>
                    {
                        InjectDelegate = target => backingField.SetValue(target, serviceLocator.Resolve(dependencyType))
                    };
                    profiles.Add(profile);
                }
            }
        }

        private static void CreateFieldProfiles(Type type, ref List<InjectionCache<object>> profiles)
        {
            var fields = type.GetFieldsIncludeBaseTypes(Flags);

            foreach (var fieldInfo in fields)
            {
                if (!fieldInfo.TryGetCustomAttribute<InjectAttribute>(out var injectAttribute))
                {
                    continue;
                }

                var serviceLocator = injectAttribute.Scope is null
                    ? ServiceLocator.Global
                    : ServiceLocator.ForScope(injectAttribute.Scope);

                var dependencyType = fieldInfo.FieldType;

                var profile = new InjectionCache<object>
                {
                    InjectDelegate = target => fieldInfo.SetValue(target, serviceLocator.Resolve(dependencyType))
                };

                profiles.Add(profile);
            }
        }

        #endregion


        #region Reflection Profiles: MonoBehaviour

        private static void CreatePropertyProfiles(Type type, ref List<InjectionCache<MonoBehaviour>> profiles)
        {
            var properties = type.GetProperties(Flags);

            foreach (var propertyInfo in properties)
            {
                if (!propertyInfo.TryGetCustomAttribute<DependencyInjectionAttribute>(out var dependencyAttribute,
                        true))
                {
                    continue;
                }

                switch (dependencyAttribute)
                {
                    case InjectAttribute injectAttribute:
                    {
                        var serviceLocator = injectAttribute.Scope is null
                            ? ServiceLocator.Global
                            : ServiceLocator.ForScope(injectAttribute.Scope);

                        var dependencyType = propertyInfo.PropertyType;

                        if (propertyInfo.CanWrite)
                        {
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target =>
                                    propertyInfo.SetValue(target, serviceLocator.Resolve(dependencyType))
                            };
                            profiles.Add(profile);
                        }
                        else if (propertyInfo.TryGetBackingField(out var backingField))
                        {
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target =>
                                    backingField.SetValue(target, serviceLocator.Resolve(dependencyType))
                            };
                            profiles.Add(profile);
                        }
                        continue;
                    }
                    case GetComponentAttribute:
                    {
                        if (propertyInfo.CanWrite)
                        {
                            var componentType = propertyInfo.PropertyType;
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target => propertyInfo.SetValue(target,
                                    target.GetComponent(componentType))
                            };
                            profiles.Add(profile);
                        }
                        else if (propertyInfo.TryGetBackingField(out var backingField))
                        {
                            var componentType = propertyInfo.PropertyType;
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target => backingField.SetValue(target,
                                    target.GetComponent(componentType))
                            };
                            profiles.Add(profile);
                        }
                        continue;
                    }
                    case GetComponentInChildrenAttribute attribute:
                    {
                        if (propertyInfo.CanWrite)
                        {
                            var componentType = propertyInfo.PropertyType;
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target => propertyInfo.SetValue(target,
                                    target.GetComponentInChildren(componentType,
                                        attribute.IncludeInactive))
                            };
                            profiles.Add(profile);
                        }
                        else if (propertyInfo.TryGetBackingField(out var backingField))
                        {
                            var componentType = propertyInfo.PropertyType;
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target => backingField.SetValue(target,
                                    target.GetComponentInChildren(componentType,
                                        attribute.IncludeInactive))
                            };
                            profiles.Add(profile);
                        }
                        continue;
                    }
                    case GetComponentInParentAttribute attribute:
                    {
                        if (propertyInfo.CanWrite)
                        {
                            var componentType = propertyInfo.PropertyType;
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target => propertyInfo.SetValue(target,
                                    target.GetComponentInParent(componentType,
                                        attribute.IncludeInactive))
                            };
                            profiles.Add(profile);
                        }
                        else if (propertyInfo.TryGetBackingField(out var backingField))
                        {
                            var componentType = propertyInfo.PropertyType;
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target => backingField.SetValue(target,
                                    target.GetComponentInParent(componentType,
                                        attribute.IncludeInactive))
                            };
                            profiles.Add(profile);
                        }
                        continue;
                    }
                    case GetComponentsAttribute:
                    {
                        if (propertyInfo.CanWrite)
                        {
                            var componentType = propertyInfo.PropertyType.GetElementType();
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target => propertyInfo.SetValue(target,
                                    target.GetComponent(componentType))
                            };
                            profiles.Add(profile);
                        }
                        else if (propertyInfo.TryGetBackingField(out var backingField))
                        {
                            var componentType = propertyInfo.PropertyType.GetElementType();
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target => backingField.SetValue(target,
                                    target.GetComponents(componentType))
                            };
                            profiles.Add(profile);
                        }
                        continue;
                    }
                    case GetComponentsInChildrenAttribute attribute:
                    {
                        if (propertyInfo.CanWrite)
                        {
                            var componentType = propertyInfo.PropertyType.GetElementType();
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target => propertyInfo.SetValue(target,
                                    target.GetComponentsInChildren(componentType,
                                        attribute.IncludeInactive))
                            };
                            profiles.Add(profile);
                        }
                        else if (propertyInfo.TryGetBackingField(out var backingField))
                        {
                            var componentType = propertyInfo.PropertyType.GetElementType();
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target => backingField.SetValue(target,
                                    target.GetComponentInChildren(componentType,
                                        attribute.IncludeInactive))
                            };
                            profiles.Add(profile);
                        }
                        continue;
                    }
                    case GetComponentsInParentAttribute attribute:
                    {
                        if (propertyInfo.CanWrite)
                        {
                            var componentType = propertyInfo.PropertyType.GetElementType();
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target => propertyInfo.SetValue(target,
                                    target.GetComponentInParent(componentType,
                                        attribute.IncludeInactive))
                            };
                            profiles.Add(profile);
                        }
                        else if (propertyInfo.TryGetBackingField(out var backingField))
                        {
                            var componentType = propertyInfo.PropertyType.GetElementType();
                            var profile = new InjectionCache<MonoBehaviour>
                            {
                                InjectDelegate = target => backingField.SetValue(target,
                                    target.GetComponentsInParent(componentType,
                                        attribute.IncludeInactive))
                            };
                            profiles.Add(profile);
                        }
                        continue;
                    }
                }
            }
        }

        private static void CreateFieldProfiles(Type type, ref List<InjectionCache<MonoBehaviour>> profiles)
        {
            var fields = type.GetFieldsIncludeBaseTypes(Flags);

            foreach (var fieldInfo in fields)
            {
                if (!fieldInfo.TryGetCustomAttribute<DependencyInjectionAttribute>(out var dependencyAttribute, true))
                {
                    continue;
                }

                switch (dependencyAttribute)
                {
                    case InjectAttribute injectAttribute:
                    {
                        var serviceLocator = injectAttribute.Scope is null
                            ? ServiceLocator.Global
                            : ServiceLocator.ForScope(injectAttribute.Scope);

                        var dependencyType = fieldInfo.FieldType;

                        var profile = new InjectionCache<MonoBehaviour>
                        {
                            InjectDelegate = target =>
                                fieldInfo.SetValue(target, serviceLocator.Resolve(dependencyType))
                        };
                        profiles.Add(profile);

                        continue;
                    }
                    case GetComponentAttribute:
                    {
                        var componentType = fieldInfo.FieldType;
                        var profile = new InjectionCache<MonoBehaviour>
                        {
                            InjectDelegate = target => fieldInfo.SetValue(target,
                                target.GetComponent(componentType))
                        };
                        profiles.Add(profile);

                        continue;
                    }
                    case GetComponentInChildrenAttribute attribute:
                    {
                        var componentType = fieldInfo.FieldType;
                        var profile = new InjectionCache<MonoBehaviour>
                        {
                            InjectDelegate = target => fieldInfo.SetValue(target,
                                target.GetComponentInChildren(componentType,
                                    attribute.IncludeInactive))
                        };
                        profiles.Add(profile);

                        continue;
                    }
                    case GetComponentInParentAttribute attribute:
                    {
                        var componentType = fieldInfo.FieldType;
                        var profile = new InjectionCache<MonoBehaviour>
                        {
                            InjectDelegate = target => fieldInfo.SetValue(target,
                                target.GetComponentInParent(componentType,
                                    attribute.IncludeInactive))
                        };
                        profiles.Add(profile);

                        continue;
                    }
                    case GetComponentsAttribute:
                    {
                        var componentType = fieldInfo.FieldType.GetElementType();
                        var profile = new InjectionCache<MonoBehaviour>
                        {
                            InjectDelegate = target => fieldInfo.SetValue(target,
                                target.GetComponent(componentType))
                        };
                        profiles.Add(profile);

                        continue;
                    }
                    case GetComponentsInChildrenAttribute attribute:
                    {
                        var componentType = fieldInfo.FieldType.GetElementType();
                        var profile = new InjectionCache<MonoBehaviour>
                        {
                            InjectDelegate = target => fieldInfo.SetValue(target,
                                target.GetComponentsInChildren(componentType, attribute.IncludeInactive))
                        };
                        profiles.Add(profile);

                        continue;
                    }
                    case GetComponentsInParentAttribute attribute:
                    {
                        var componentType = fieldInfo.FieldType.GetElementType();
                        var profile = new InjectionCache<MonoBehaviour>
                        {
                            InjectDelegate = target => fieldInfo.SetValue(target,
                                target.GetComponentInParent(componentType,
                                    attribute.IncludeInactive))
                        };
                        profiles.Add(profile);

                        continue;
                    }
                }
            }
        }

        #endregion
    }
}