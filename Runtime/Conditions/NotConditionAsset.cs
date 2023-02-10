using UnityEngine;
using UnityEngine.Serialization;

namespace Baracuda.Mediator
{
    public sealed class NotConditionAsset : ConditionAsset
    {
        [FormerlySerializedAs("condition")] [SerializeField] private ConditionAsset conditionAsset;

        public override bool Check()
        {
            return !conditionAsset.Check();
        }
    }
}