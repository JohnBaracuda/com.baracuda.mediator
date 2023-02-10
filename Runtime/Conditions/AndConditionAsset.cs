using UnityEngine;

namespace Baracuda.Mediator
{
    public sealed class AndConditionAsset : ConditionAsset
    {
        [SerializeField] private ConditionAsset[] conditions;
        public override bool Check()
        {
            return conditions.All();
        }
    }
}