using Baracuda.Tools;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Baracuda.Mediator.States
{
    public abstract class State : MediatorAsset
    {
        #region Inspector

        [Foldout("Debug")]
        [ShowInInspector]
        [Tooltip("The name of the state sub asset.")]
        private string Name
        {
            get => name;
            set => name = value;
        }

        #endregion


        /// <summary>
        ///     Is this state the active state.
        /// </summary>
        [PublicAPI]
        public abstract bool IsActive { get; }
    }

    public abstract class State<T> : State where T : State<T>
    {
        #region Public API

        /// <summary>
        ///     Invoked when this state is enabled.
        /// </summary>
        public event Action Entered
        {
            add => entered += value;
            remove => entered -= value;
        }

        /// <summary>
        ///     Invoked when this state is disabled.
        /// </summary>
        public event Action Exited
        {
            add => exited += value;
            remove => exited -= value;
        }

        /// <summary>
        ///     Is this state the active state.
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        [Foldout("Debug")]
        public sealed override bool IsActive => stateMachine.State == this;

        /// <summary>
        ///     Was this state active previously.
        /// </summary>
        public bool WasActive => stateMachine.PreviousState == this;

        #endregion


        #region Protected API

        [ReadOnly]
        [ShowInInspector]
        [Foldout("Debug")]
        protected internal StateMachine<T> StateMachine
        {
            get => stateMachine;
            set => stateMachine = value;
        }

        protected internal virtual void OnStateEnter(T previousState)
        {
        }

        protected internal virtual void OnStateExit(T nextState)
        {
        }

        protected internal virtual void OnStateDisabled()
        {
        }

        protected internal virtual void OnStateEnabled()
        {
        }

        protected internal virtual void UpdateState()
        {
        }

        #endregion


        #region Fields

        [SerializeField] [HideInInspector] private StateMachine<T> stateMachine;

        internal Action entered;
        internal Action exited;

        #endregion


        #region Editor

        [Button]
        public void Activate()
        {
            StateMachine.SetState((T) this);
        }

        public override string ToString()
        {
            return name;
        }

        #endregion
    }
}