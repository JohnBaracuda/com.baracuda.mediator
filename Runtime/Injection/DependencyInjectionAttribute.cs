using JetBrains.Annotations;
using System;

namespace Baracuda.Bedrock.Injection
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class DependencyInjectionAttribute : Attribute
    {
    }
}