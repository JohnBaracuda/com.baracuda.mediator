using UnityEngine;

namespace Baracuda.Mediator
{
    public abstract class Condition : ScriptableObject
    {
        public abstract bool Check();
    }
}
