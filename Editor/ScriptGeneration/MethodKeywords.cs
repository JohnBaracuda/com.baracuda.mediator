using System;

namespace Baracuda.Bedrock.Editor.ScriptGeneration
{
    [Flags]
    public enum MethodKeywords
    {
        Static = 1 << 0,
        Virtual = 1 << 1,
        Override = 1 << 2,
        Abstract = 1 << 3,
        Async = 1 << 4,
        Extern = 1 << 5,
        New = 1 << 6,
        Partial = 1 << 7,
        Sealed = 1 << 8
    }
}