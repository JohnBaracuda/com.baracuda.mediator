using System;

namespace Baracuda.Bedrock.Editor.ScriptGeneration
{
    [Flags]
    public enum FieldKeywords
    {
        Static = 1 << 0,
        Readonly = 1 << 1,
        Volatile = 1 << 2,
        Const = 1 << 3,
        New = 1 << 4
    }
}