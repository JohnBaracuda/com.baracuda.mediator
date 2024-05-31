using UnityEngine;

namespace Baracuda.Bedrock.Pooling
{
    public class LineRendererPool : ComponentPool<LineRenderer>
    {
        [SerializeField] [Min(2)] private int positionCount = 2;

        protected override void OnGetInstance(LineRenderer instance)
        {
            base.OnGetInstance(instance);
            instance.positionCount = positionCount;
        }
    }
}