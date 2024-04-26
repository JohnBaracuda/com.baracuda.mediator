using JetBrains.Annotations;
using System;

namespace Baracuda.Mediator.Injection
{
    public enum InjectProvider
    {
        Global = 0,
        ActiveScene = 1,
        TargetScene = 2
    }

    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
        public string Callback { get; set; }
    }
}