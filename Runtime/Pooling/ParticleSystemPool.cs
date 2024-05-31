using UnityEngine;

namespace Baracuda.Bedrock.Pooling
{
    public class ParticleSystemPool : ComponentPool<ParticleSystem>
    {
        protected override void OnReleaseInstance(ParticleSystem instance)
        {
            instance.Stop(true);
            base.OnReleaseInstance(instance);
        }
    }
}