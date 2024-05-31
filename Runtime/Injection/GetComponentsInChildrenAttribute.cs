using System;

namespace Baracuda.Bedrock.Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetComponentsInChildrenAttribute : DependencyInjectionAttribute
    {
        public GetComponentsInChildrenAttribute()
        {
        }

        public GetComponentsInChildrenAttribute(bool includeInactive)
        {
            IncludeInactive = includeInactive;
        }

        public bool IncludeInactive { get; }
    }
}