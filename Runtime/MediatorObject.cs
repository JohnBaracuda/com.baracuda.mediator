using Baracuda.Utilities.Inspector;
using UnityEngine;

namespace Baracuda.Mediator
{
    public abstract class MediatorObject : ScriptableObject
    {
        [Foldout("Description")] [TextArea(0, 3)]
        [SerializeField] [RuntimeReadonly] private string description;
    }
}
