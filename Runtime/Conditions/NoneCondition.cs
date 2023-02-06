using UnityEngine;

namespace Baracuda.Mediator
{
    public sealed class NoneCondition : Condition
    {
        [SerializeField] private Condition[] conditions;
        public override bool Check()
        {
            return conditions.None();
        }
    }
}