using Sirenix.OdinInspector;
using UnityEngine;

namespace Baracuda.Bedrock.States
{
    public abstract class State<T> : ScriptableObject where T : State<T>
    {
        #region Protected API

        protected internal virtual void OnStateEnter(T previousState)
        {
        }

        protected internal virtual void OnStateExit(T nextState)
        {
        }

        protected internal StateMachineSO<T> StateMachineSo { get; set; }

        #endregion


        #region Editor

        public override string ToString()
        {
            return name;
        }

        [Button]
        public void Activate()
        {
            StateMachineSo.SetActiveState((T) this);
        }

        #endregion
    }
}