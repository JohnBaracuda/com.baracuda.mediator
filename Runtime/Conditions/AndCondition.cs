using UnityEngine;

namespace Baracuda.Mediator
{
    public sealed class AndCondition : Condition
    {
        [SerializeField] private Condition[] conditions;
        public override bool Check()
        {
            return conditions.All();
        }
    }
}