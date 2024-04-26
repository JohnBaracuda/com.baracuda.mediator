using Baracuda.Mediator.Services;
using Baracuda.Utilities.Reflection;
using JetBrains.Annotations;
using System;
using System.Reflection;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

namespace Baracuda.Mediator.Injection
{
    public static class Inject
    {
        private const BindingFlags Flags = BindingFlags.Instance |
                                           BindingFlags.Static |
                                           BindingFlags.Public |
                                           BindingFlags.NonPublic |
                                           BindingFlags.FlattenHierarchy;

        public static void Dependencies([NotNull] object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Profiler.BeginSample("Dependency Injection");

            var type = target.GetType();

            var fields = type.GetFields(Flags);

            foreach (var fieldInfo in fields)
            {
                Assert.IsNotNull(fieldInfo, "fieldInfo != null");

                if (!fieldInfo.HasAttribute<InjectAttribute>())
                {
                    continue;
                }

                var dependencyType = fieldInfo.FieldType;
                var dependency = ServiceLocator.Global.Get(dependencyType);
                fieldInfo.SetValue(target, dependency);
            }

            var properties = type.GetProperties(Flags);

            foreach (var propertyInfo in properties)
            {
                Assert.IsNotNull(propertyInfo, "propertyInfo != null");

                if (!propertyInfo.HasAttribute<InjectAttribute>())
                {
                    continue;
                }

                if (propertyInfo.CanWrite)
                {
                    var dependencyType = propertyInfo.PropertyType;
                    var dependency = ServiceLocator.Global.Get(dependencyType);
                    propertyInfo.SetValue(target, dependency);
                }
                else if (propertyInfo.TryGetBackingField(out var backingField))
                {
                    var dependencyType = propertyInfo.PropertyType;
                    var dependency = ServiceLocator.Global.Get(dependencyType);
                    backingField.SetValue(target, dependency);
                }
            }

            Profiler.EndSample();
        }
    }
}