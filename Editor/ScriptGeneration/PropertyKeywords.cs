using System;

namespace Baracuda.Bedrock.Editor.ScriptGeneration
{
    [Flags]
    public enum PropertyKeywords
    {
        Static = 1 << 0,
        Virtual = 1 << 1,
        Override = 1 << 2,
        Abstract = 1 << 3,
        New = 1 << 4,
        Sealed = 1 << 5
    }
}