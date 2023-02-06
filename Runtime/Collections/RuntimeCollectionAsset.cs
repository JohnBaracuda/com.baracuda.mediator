using Baracuda.Utilities.Callbacks;
using Baracuda.Utilities.Inspector;
using System.Collections.Generic;
using UnityEngine;

namespace Baracuda.Mediator
{
    public abstract class RuntimeCollectionAsset<T> : CollectionAsset, IOnEnterEdit
    {
        [Foldout("Options")]
        [SerializeField] private bool logLeaks = true;
        [SerializeField] private bool clearLeaks = true;

        [Button("Clear")]
        [Foldout("Options")]
        private protected abstract void ClearInternal();

        private protected abstract int CountInternal { get; }
        private protected abstract IEnumerable<T> CollectionInternal { get; }

        protected virtual void OnEnable()
        {
            EngineCallbacks.AddEnterEditModeListener(this);
        }

        public void OnEnterEditMode()
        {
            if (logLeaks && CountInternal > 0)
            {
                Debug.LogWarning("Collection", $"Leak detected in runtime collection", this);
                Debug.LogWarning("Collection", CollectionInternal);
            }
            if (clearLeaks && CountInternal > 0)
            {
                ClearInternal();
            }
        }
    }
}