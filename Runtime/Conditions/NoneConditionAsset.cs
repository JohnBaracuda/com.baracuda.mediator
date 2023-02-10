using UnityEngine;

namespace Baracuda.Mediator
{
    public sealed class NoneConditionAsset : ConditionAsset
    {
        [SerializeField] private ConditionAsset[] conditions;
        public override bool Check()
        {
            return conditions.None();
        }
    }
}