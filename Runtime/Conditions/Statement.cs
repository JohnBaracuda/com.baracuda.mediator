using Baracuda.Utilities.Inspector;
using System;
using UnityEngine;

namespace Baracuda.Mediator
{
    public enum StatementType
    {
        All,
        Any,
        None
    }

    [Serializable]
    public struct Statement
    {
        [SerializeField] private StatementType type;
        [SerializeField] private ConditionAsset[] conditions;

        public static implicit operator bool(Statement statement)
        {
            return statement.Check();
        }

        public bool Check() =>
            type switch
            {
                StatementType.All => conditions.All(),
                StatementType.Any => conditions.Any(),
                StatementType.None => conditions.None(),
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
