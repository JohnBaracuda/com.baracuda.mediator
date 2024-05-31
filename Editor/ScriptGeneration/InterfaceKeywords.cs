using System;

namespace Baracuda.Bedrock.Editor.ScriptGeneration
{
    [Flags]
    public enum InterfaceKeywords
    {
        Public = 1 << 0,
        Partial = 1 << 1,
        New = 1 << 2
    }
}