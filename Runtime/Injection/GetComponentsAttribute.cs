﻿using System;

namespace Baracuda.Bedrock.Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetComponentsAttribute : DependencyInjectionAttribute
    {
    }
}