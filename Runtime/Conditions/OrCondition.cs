using UnityEngine;

namespace Baracuda.Mediator
{
    public sealed class OrCondition : Condition
    {
        [SerializeField] private Condition[] conditions;
        public override bool Check()
        {
            return conditions.Any();
        }
    }
}