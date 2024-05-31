using Baracuda.Utilities;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Baracuda.Bedrock.States
{
    /// <summary>
    ///     Generic base class for scriptable object based state machines.
    /// </summary>
    /// <typeparam name="T">The type of the state instances</typeparam>
    public abstract class StateMachineSO<T> : MonoBehaviour where T : State<T>
    {
        #region Fields

        private bool _enabled;
        private readonly Dictionary<Type, State<T>> _uniqueStates = new();
        private static readonly Color loggingColor = RandomUtility.LoggingColor();

        #endregion


        #region Properties & Events

        /// <summary>
        ///     Invoked when the state is changed form one to another
        /// </summary>
        public event Action<T, T> StateChanged;

        /// <summary>
        ///     The active state.
        /// </summary>
        [PublicAPI]
        [ReadOnly]
        [ShowInInspector]
        public T State { get; private set; }

        /// <summary>
        ///     The previously active state.
        /// </summary>
        [PublicAPI]
        [ReadOnly]
        [ShowInInspector]
        public T PreviousState { get; private set; }

        #endregion


        #region State Adding

        public void AddState(T state)
        {
            state.StateMachineSo = this;
        }

        public void AddUniqueState(T state)
        {
            var type = state.GetType();
            var wasAdded = _uniqueStates.TryAdd(type, state);
            if (wasAdded is false)
            {
                Debug.LogError(
                    $"Could not add state of type {type.FullName}! State of this type has already been registered!");
                return;
            }

            state.StateMachineSo = this;
        }

        public void AddUniqueState(Type type, T state)
        {
            var wasAdded = _uniqueStates.TryAdd(type, state);
            if (wasAdded is false)
            {
                Debug.LogError(
                    $"Could not add state of type {type.FullName}! State of this type has already been registered!");
                return;
            }

            state.StateMachineSo = this;
        }

        /// <summary>
        ///     Set the active state.
        /// </summary>
        public void SetActiveState<TState>() where TState : State<T>
        {
            var type = typeof(TState);
            if (_uniqueStates.TryGetValue(type, out var state))
            {
                SetActiveState((T) state);
            }
        }

        /// <summary>
        ///     Set the active state.
        /// </summary>
        public void SetActiveState(T nextState)
        {
            if (nextState == State)
            {
                return;
            }

            PreviousState = State;

            Debug.Log(GetType().Name, $"Transition from state: {State.ToNullString()} to {nextState.ToNullString()}",
                loggingColor);

            if (PreviousState != null)
            {
                PreviousState.OnStateExit(nextState);
            }
            State = nextState;
            if (nextState != null)
            {
                nextState.OnStateEnter(PreviousState);
            }

            StateChanged?.Invoke(PreviousState, nextState);
        }

        #endregion
    }
}