using System;

namespace Baracuda.Bedrock.Editor.ScriptGeneration
{
    [Flags]
    public enum ClassKeywords
    {
        Static = 1 << 0,
        Sealed = 1 << 1,
        Abstract = 1 << 2,
        Partial = 1 << 3,
        New = 1 << 4
    }
}