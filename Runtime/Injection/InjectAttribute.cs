using System;

namespace Baracuda.Bedrock.Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : DependencyInjectionAttribute
    {
        public string Scope { get; set; } = null;
    }
}