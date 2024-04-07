using Baracuda.Utilities;
using UnityEngine;

namespace Baracuda.Mediator.Pooling
{
    public class MeshRendererPool : PoolAsset<MeshRenderer>
    {
        protected override void OnReleaseInstance(MeshRenderer instance)
        {
            instance.SetActive(false);
            instance.transform.SetParent(Parent);
        }

        protected override void OnGetInstance(MeshRenderer instance)
        {
            instance.SetActive(true);
        }
    }
}