using System;

namespace Baracuda.Bedrock.Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetComponentInChildrenAttribute : DependencyInjectionAttribute
    {
        public GetComponentInChildrenAttribute()
        {
        }

        public GetComponentInChildrenAttribute(bool includeInactive)
        {
            IncludeInactive = includeInactive;
        }

        public bool IncludeInactive { get; }
    }
}