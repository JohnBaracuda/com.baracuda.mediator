using Baracuda.Mediator.Callbacks;
using Baracuda.Mediator.Events;
using Baracuda.Mediator.Registry;
using Baracuda.Serialization;
using Baracuda.Utilities;
using Baracuda.Utilities.Types;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

namespace Baracuda.Mediator.States
{
    /// <summary>
    ///     Base class for scriptable object based state machines.
    /// </summary>
    public abstract class StateMachine : RegisteredAsset
    {
    }

    /// <summary>
    ///     Generic base class for scriptable object based state machines.
    /// </summary>
    /// <typeparam name="T">The type of the state instances</typeparam>
    public abstract class StateMachine<T> : StateMachine where T : State<T>
    {
        #region Fields

        [Title("State Machine")]
        [Tooltip("When enabled, the state machine automatically enabled on load.")]
        [SerializeField] private bool autoEnable = true;
        [Tooltip("When enabled, the active state of the state machine is saved persistently.")]
        [SerializeField] private bool saveStatePersistent;
        [ShowIf(nameof(saveStatePersistent))]
        [SerializeField] private bool saveNullState;
        [Tooltip("Optional start state that is only applied if no state is set manually during startup.")]
        [SerializeField] private Optional<T> startState;
        [Space]
        [Tooltip("Registry containing every available state.")]
        [SerializeField] private List<T> states;

        [Title("Debug")]
        [Tooltip("Color in which the log category is displayed.")]
        [SerializeField] private Color messageColor = Color.cyan;

        [NonSerialized] private bool _enabled;
        [NonSerialized] private T _bufferedState;
        private readonly Broadcast<T, T> _stateChanged = new();
        private const int NullIndex = -1;

        public event Action<T, T> StateChanged
        {
            add => _stateChanged.Add(value);
            remove => _stateChanged.Remove(value);
        }

        #endregion


        #region Properties

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

        /// <summary>
        ///     The active state of the system.
        /// </summary>
        [PublicAPI]
        [ReadOnly]
        [ShowInInspector]
        public bool Enabled
        {
            get => _enabled;
            set => SetEnabled(value);
        }

        #endregion


        #region Initialization & Shutdown

        [CallbackOnInitialization]
        private void InitializeStateMachine()
        {
            Log("Initializing State Machine");
            PreviousState = null;

            if (autoEnable)
            {
                Enabled = true;
            }

            if (_bufferedState != null)
            {
                var stateName = _bufferedState.name;
                SetState(_bufferedState);
                Log($"Set buffered state ({stateName}) as start state");
                return;
            }

            if (saveStatePersistent && FileSystem.Profile.TryLoadFile<int>(GUID, out var stateIndex))
            {
                var state = stateIndex == NullIndex ? null : states[stateIndex];
                var stateName = state != null ? state.name : "none";
                SetState(state);
                Log($"Set loaded state ({stateName}) as start state with index ({stateIndex})");
                return;
            }

            if (State == null && startState.TryGetValue(out var start))
            {
                var stateName = start != null ? start.name : "none";
                SetState(start);
                Log($"Set default state ({stateName}) as start state");
            }
        }

        [CallbackOnApplicationQuit]
        private void ShutdownStateMachine()
        {
            Enabled = false;
            State = null;
            State = null;
            PreviousState = null;
            _stateChanged.Clear();
            _bufferedState = null;

            Log("Shutdown State Machine");
        }

        #endregion


        #region Set State

        /// <summary>
        ///     Set the active state.
        /// </summary>
        public void SetState(T nextState)
        {
            if (nextState == State)
            {
                return;
            }

            if (Enabled is false)
            {
                _bufferedState = nextState;
                return;
            }

            _bufferedState = null;
            PreviousState = State;

            if (PreviousState != null)
            {
                PreviousState.OnStateExit(nextState);
                PreviousState.exited?.Invoke();
            }
            State = nextState;
            if (nextState != null)
            {
                nextState.OnStateEnter(PreviousState);
                nextState.entered?.Invoke();
            }

            var fromStateString = PreviousState != null ? PreviousState.name : "none";
            var toStateString = nextState != null ? nextState.name : "none";
            Log("Transition " +
                $"from ({fromStateString}) " +
                $"to ({toStateString})");

            _stateChanged.Raise(PreviousState, nextState);
            if (saveStatePersistent)
            {
                var isNullState = State == null;
                if (isNullState && saveNullState is false)
                {
                    return;
                }
                var stateIndex = isNullState ? NullIndex : states.IndexOf(State);
                FileSystem.Profile.SaveFile(GUID, stateIndex);
            }
        }

        public void ResetStateMachine()
        {
            if (State == null)
            {
                return;
            }
            _bufferedState = null;
            PreviousState = State;

            if (PreviousState != null)
            {
                PreviousState.OnStateExit(null);
                PreviousState.exited?.Invoke();
            }
            State = null;

            Log("Transition " +
                $"from ({PreviousState.ToNullString()}) " +
                $"to ({null})");

            _stateChanged.Raise(PreviousState, null);
            if (saveStatePersistent && saveNullState)
            {
                FileSystem.Profile.SaveFile(GUID, NullIndex);
            }
        }

        #endregion


        #region Gameloop

        private void UpdateGameState()
        {
            if (Enabled is false)
            {
                return;
            }
            if (State == null)
            {
                return;
            }

            State.UpdateState();
        }

        #endregion


        #region Active State

        private void SetEnabled(bool enabled)
        {
            if (enabled == _enabled)
            {
                return;
            }

            _enabled = enabled;

            if (_enabled)
            {
                Assert.IsFalse(Gameloop.IsDelegateSubscribedToUpdate(UpdateGameState));
                Gameloop.Update += UpdateGameState;
            }
            else
            {
                Assert.IsTrue(Gameloop.IsDelegateSubscribedToUpdate(UpdateGameState));
                Gameloop.Update -= UpdateGameState;
            }

            if (State == null)
            {
                return;
            }

            if (enabled)
            {
                Log($"Enable ({State.name})");
                State.OnStateEnabled();
            }
            else
            {
                Log($"Disable ({State.name})");
                State.OnStateDisabled();
            }
        }

        #endregion


        #region Editor

        [Conditional("DEBUG")]
        private void Log(string message)
        {
            Debug.Log(typeof(T).Name, message, messageColor);
        }

#if UNITY_EDITOR

        protected override void OnEnable()
        {
            base.OnEnable();
            for (var i = states.Count - 1; i >= 0; i--)
            {
                if (states[i] == null)
                {
                    states.RemoveAt(i);
                }
            }
            foreach (var state in states)
            {
                state.StateMachine = this;
            }
        }

        internal void RemoveState(T state)
        {
            if (UnityEditor.Selection.activeObject == state)
            {
                UnityEditor.Selection.activeObject = this;
            }
            states.Remove(state);
            UnityEditor.AssetDatabase.RemoveObjectFromAsset(state);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorApplication.delayCall += () => { DestroyImmediate(state); };
        }

#endif

        #endregion


        #region State Subsystem

        public void Activate()
        {
            if (Enabled)
            {
                return;
            }

            Enabled = true;

            if (_bufferedState != null)
            {
                var state = _bufferedState;
                SetState(_bufferedState);
                Log($"Set buffered state ({state}) as start state");
                return;
            }

            if (saveStatePersistent && FileSystem.Profile.TryLoadFile<int>(GUID, out var stateIndex))
            {
                var state = stateIndex == -1 ? null : states[stateIndex];
                SetState(state);
                Log($"Set loaded state ({state}) as start state with index ({stateIndex})");
                return;
            }

            if (State == null && startState.TryGetValue(out var start))
            {
                SetState(start);
                Log($"Set default state ({start}) as start state");
            }
        }

        public void Deactivate()
        {
            Enabled = false;
        }

        #endregion
    }
}