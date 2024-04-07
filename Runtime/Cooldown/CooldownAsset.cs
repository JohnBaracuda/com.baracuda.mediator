using Baracuda.Mediator.Callbacks;
using Baracuda.Mediator.Events;
using Baracuda.Tools;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Baracuda.Mediator.Cooldown
{
    public class CooldownAsset : MediatorAsset, ICooldown
    {
        #region Inspector

        [Foldout("Cooldown")]
        [SerializeField] private float cooldownInSeconds;

        #endregion


        #region Properties

        [ReadOnly]
        [Foldout("Debug")]
        public float TotalDurationInSeconds => GetTotalDurationInSeconds();

        [ReadOnly]
        [Foldout("Debug")]
        public float Value
        {
            get => cooldownInSeconds;
            set => cooldownInSeconds = value;
        }

        [ReadOnly]
        [Foldout("Debug")]
        public float RemainingDurationInSeconds { get; private set; }

        [ReadOnly]
        [Foldout("Debug")]
        public float PassedDurationInSeconds => TotalDurationInSeconds - RemainingDurationInSeconds;

        [ReadOnly]
        [Foldout("Debug")]
        public float PercentageCompleted => FactorCompleted * 100;

        [ReadOnly]
        [Foldout("Debug")]
        public float FactorCompleted => PassedDurationInSeconds * _reciprocalOfTotalDuration;

        [ReadOnly]
        [Foldout("Debug")]
        public bool IsRunning { get; private set; }

        [ReadOnly]
        [Foldout("Debug")]
        public bool IsPaused => IsActive && IsRunning is false;

        [ReadOnly]
        [Foldout("Debug")]
        public bool IsActive { get; private set; }

        [ReadOnly]
        [Foldout("Debug")]
        public bool IsInactive => IsActive is false;

        [Line]
        [ReadOnly]
        public IList<ICooldownDurationModifier> CooldownDurationModifiers { get; } =
            new List<ICooldownDurationModifier>();

        #endregion


        #region Events

        public event Action Completed
        {
            add => _completed.Add(value);
            remove => _completed.Remove(value);
        }

        public event Action Cancelled
        {
            add => _cancelled.Add(value);
            remove => _cancelled.Remove(value);
        }

        public event Action Started
        {
            add => _started.Add(value);
            remove => _started.Remove(value);
        }

        public event Action Restarted
        {
            add => _restarted.Add(value);
            remove => _restarted.Remove(value);
        }

        public event Action Paused
        {
            add => _paused.Add(value);
            remove => _paused.Remove(value);
        }

        public event Action Resumed
        {
            add => _resumed.Add(value);
            remove => _resumed.Remove(value);
        }

        public event Action<float> Reduced
        {
            add => _reduced.Add(value);
            remove => _reduced.Remove(value);
        }

        #endregion


        #region Fields

        private readonly Broadcast _completed = new();
        private readonly Broadcast _cancelled = new();
        private readonly Broadcast _started = new();
        private readonly Broadcast _restarted = new();
        private readonly Broadcast _paused = new();
        private readonly Broadcast _resumed = new();
        private readonly Broadcast<float> _reduced = new();
        private float _reciprocalOfTotalDuration;

        #endregion


        #region Public API

        [Button]
        [Foldout("Controls")]
        public bool Start()
        {
            if (IsActive)
            {
                return false;
            }

            CooldownSystem.AddCooldown(this);
            IsRunning = true;
            IsActive = true;
            var totalDurationInSeconds = TotalDurationInSeconds;
            RemainingDurationInSeconds = totalDurationInSeconds;
            _reciprocalOfTotalDuration = 1 / totalDurationInSeconds;
            _started.Raise();
            return true;
        }

        [Button]
        [Foldout("Controls")]
        public bool Cancel()
        {
            if (IsInactive)
            {
                return false;
            }

            IsActive = false;
            IsRunning = false;
            RemainingDurationInSeconds = 0;
            CooldownSystem.RemoveCooldown(this);
            _cancelled.Raise();
            return true;
        }

        [Button]
        [Foldout("Controls")]
        public bool Complete()
        {
            if (IsInactive)
            {
                return false;
            }

            IsActive = false;
            IsRunning = false;
            RemainingDurationInSeconds = 0;
            CooldownSystem.RemoveCooldown(this);

            _completed.Raise();
            return true;
        }

        [Button]
        [Foldout("Controls")]
        public bool Restart(bool startIfInactive = true)
        {
            if (IsInactive)
            {
                if (startIfInactive)
                {
                    Start();
                    return true;
                }
                return false;
            }

            var totalDurationInSeconds = TotalDurationInSeconds;
            RemainingDurationInSeconds = totalDurationInSeconds;
            _reciprocalOfTotalDuration = 1 / totalDurationInSeconds;
            IsRunning = true;
            _restarted.Raise();
            return true;
        }

        [Line]
        [Button]
        [Foldout("Controls")]
        public bool Reduce(float durationInSeconds)
        {
            if (IsInactive)
            {
                return false;
            }

            RemainingDurationInSeconds -= durationInSeconds;
            if (RemainingDurationInSeconds <= 0)
            {
                Complete();
            }
            _reduced.Raise(durationInSeconds);
            return true;
        }

        [Line]
        [Button]
        [Foldout("Controls")]
        public bool Pause()
        {
            if (IsInactive)
            {
                return false;
            }
            if (IsPaused)
            {
                return false;
            }

            IsRunning = false;
            _paused.Raise();
            return true;
        }

        [Button]
        [Foldout("Controls")]
        public bool Resume()
        {
            if (IsInactive)
            {
                return false;
            }
            if (IsRunning)
            {
                return false;
            }

            IsRunning = true;
            _resumed.Raise();
            return true;
        }

        #endregion


        #region Internal

        [CallbackOnEnterEditMode]
        [CallbackOnExitEditMode]
        private void OnEnterEditMode()
        {
            _started.Clear();
            _completed.Clear();
            _cancelled.Clear();
            _restarted.Clear();
            _reduced.Clear();
            Cancel();
        }

        private float GetTotalDurationInSeconds()
        {
            var duration = Value;
            foreach (var cooldownDurationModifier in CooldownDurationModifiers)
            {
                cooldownDurationModifier.ModifyCooldownDuration(ref duration, Value);
            }
            return duration;
        }

        void ICooldown.UpdateCooldown(float deltaTime)
        {
            Assert.IsTrue(IsActive);

            if (IsRunning)
            {
                RemainingDurationInSeconds -= deltaTime;
                if (RemainingDurationInSeconds <= 0)
                {
                    Complete();
                }
#if UNITY_EDITOR
                Repaint();
#endif
            }
        }

        #endregion
    }
}