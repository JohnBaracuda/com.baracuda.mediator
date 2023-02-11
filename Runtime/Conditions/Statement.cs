using System;
using UnityEngine;

namespace Baracuda.Mediator
{
    public enum CheckMethod
    {
        WhenAll,
        WhenAny,
        WhenNone
    }

    [Serializable]
    public struct Statement
    {
        [SerializeField] private CheckMethod checkMethod;
        [SerializeField] private ConditionAsset[] conditions;

        public static implicit operator bool(Statement statement)
        {
            return statement.Check();
        }

        public bool Check() =>
            checkMethod switch
            {
                CheckMethod.WhenAll => conditions.All(),
                CheckMethod.WhenAny => conditions.Any(),
                CheckMethod.WhenNone => conditions.None(),
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
