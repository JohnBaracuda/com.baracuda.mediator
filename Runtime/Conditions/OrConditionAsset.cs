using UnityEngine;

namespace Baracuda.Mediator
{
    public sealed class OrConditionAsset : ConditionAsset
    {
        [SerializeField] private ConditionAsset[] conditions;
        public override bool Check()
        {
            return conditions.Any();
        }
    }
}