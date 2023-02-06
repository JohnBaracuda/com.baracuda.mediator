using UnityEngine;

namespace Baracuda.Mediator
{
    public sealed class NotCondition : Condition
    {
        [SerializeField] private Condition condition;

        public override bool Check()
        {
            return !condition.Check();
        }
    }
}