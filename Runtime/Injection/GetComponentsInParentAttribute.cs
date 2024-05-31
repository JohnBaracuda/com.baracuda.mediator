using System;

namespace Baracuda.Bedrock.Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetComponentsInParentAttribute : DependencyInjectionAttribute
    {
        public GetComponentsInParentAttribute()
        {
        }

        public GetComponentsInParentAttribute(bool includeInactive)
        {
            IncludeInactive = includeInactive;
        }

        public bool IncludeInactive { get; }
    }
}