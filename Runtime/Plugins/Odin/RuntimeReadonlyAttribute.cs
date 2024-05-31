using System;
using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Bedrock.Odin
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class RuntimeReadonlyAttribute : PropertyAttribute
    {
    }
}