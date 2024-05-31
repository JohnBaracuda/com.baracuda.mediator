using System;

namespace Baracuda.Bedrock.Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetComponentInParentAttribute : DependencyInjectionAttribute
    {
        public GetComponentInParentAttribute()
        {
        }

        public GetComponentInParentAttribute(bool includeInactive)
        {
            IncludeInactive = includeInactive;
        }

        public bool IncludeInactive { get; }
    }
}