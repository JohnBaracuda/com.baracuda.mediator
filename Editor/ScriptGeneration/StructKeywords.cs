using System;

namespace Baracuda.Bedrock.Editor.ScriptGeneration
{
    [Flags]
    public enum StructKeywords
    {
        Readonly = 1 << 0,
        Partial = 1 << 1,
        New = 1 << 2,
        Ref = 1 << 3 // Added in C# 7.2 for ref structs
    }
}