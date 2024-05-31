using Baracuda.Bedrock.Callbacks;
using Baracuda.Bedrock.Injection;
using Baracuda.Bedrock.Locks;
using Baracuda.Bedrock.Odin;
using Baracuda.Utilities;
using Baracuda.Utilities.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Bedrock.Cursor
{
    /// <summary>
    ///     Base class for managing cursor state / textures / animations.
    /// </summary>
    public class CursorManager : MonoBehaviour
    {
        #region Fields

        [Inject] [Debug] private readonly CursorSystemSettings _settings;
        [Debug] private List<object> VisibilityBlocker => _cursorVisibilityBlocker.Locks.ToList();
        [Debug] private List<object> MovementLimiter => _cursorMovementLimiter.Locks.ToList();
        [Debug] private List<object> MovementBlocker => _cursorMovementBlocker.Locks.ToList();

        private readonly Lock _cursorVisibilityBlocker = new();
        private readonly Lock _cursorMovementLimiter = new();
        private readonly Lock _cursorMovementBlocker = new();

        private readonly StackList<CursorFile> _cursorStack = new();

        #endregion


        #region Properties

        public CursorSet ActiveCursorSet { get; private set; }
        public CursorFile ActiveCursor => _cursorStack.Peek();

        public static CursorLockMode LockState
        {
            get => UnityEngine.Cursor.lockState;
            private set
            {
                var changed = LockState != value;
                UnityEngine.Cursor.lockState = value;
                if (changed)
                {
                    CursorLockModeChanged?.Invoke(value);
                }
            }
        }

        public static bool Visible
        {
            get => UnityEngine.Cursor.visible;
            private set
            {
                var changed = Visible != value;
                UnityEngine.Cursor.visible = value;
                if (changed)
                {
                    CursorVisibilityChanged?.Invoke(value);
                }
            }
        }

        #endregion


        #region Events

        /// <summary>
        ///     Event is invoked every time the <see cref="UnityEngine.Cursor" /> is updated.<br />
        ///     Event will pass a reference to the new <see cref="CursorFile" />.
        /// </summary>
        public static event CursorFileDelegate CursorChanged;

        /// <summary>
        ///     Event is invoked every time the property <see cref="UnityEngine.Cursor" />.
        ///     <see cref="UnityEngine.Cursor.lockState" /> is updated.<br />
        ///     Event will pass the value of the new <see cref="CursorLockMode" />.
        /// </summary>
        public static event LockStateDelegate CursorLockModeChanged;

        /// <summary>
        ///     Event is invoked every time the property <see cref="UnityEngine.Cursor" />.
        ///     <see cref="UnityEngine.Cursor.visible" /> is updated.<br />
        ///     Event will pass the value of the new <see cref="CursorLockMode" />.
        /// </summary>
        public static event VisibilityDelegate CursorVisibilityChanged;

        #endregion


        #region Cursor State

        public void AddCursorVisibilityBlocker(object blocker)
        {
            _cursorVisibilityBlocker.Add(blocker);
        }

        public void AddCursorConfiner(object confiner)
        {
            _cursorMovementLimiter.Add(confiner);
        }

        public void AddCursorLocker(object blocker)
        {
            _cursorMovementBlocker.Add(blocker);
        }

        public void RemoveCursorVisibilityBlocker(object blocker)
        {
            _cursorVisibilityBlocker.Remove(blocker);
        }

        public void RemoveCursorConfiner(object confiner)
        {
            _cursorMovementLimiter.Remove(confiner);
        }

        public void RemoveCursorLocker(object blocker)
        {
            _cursorMovementBlocker.Remove(blocker);
        }

        #endregion


        #region Setup

        private void Start()
        {
            ActiveCursorSet = _settings.StartCursorSet;
            AddCursorOverride(_settings.StartCursor);

            _cursorVisibilityBlocker.FirstAdded += UpdateCursorState;
            _cursorVisibilityBlocker.LastRemoved += UpdateCursorState;
            _cursorMovementLimiter.FirstAdded += UpdateCursorState;
            _cursorMovementLimiter.LastRemoved += UpdateCursorState;
            _cursorMovementBlocker.FirstAdded += UpdateCursorState;
            _cursorMovementBlocker.LastRemoved += UpdateCursorState;
        }

        private void OnDestroy()
        {
            _cursorVisibilityBlocker.FirstAdded -= UpdateCursorState;
            _cursorVisibilityBlocker.LastRemoved -= UpdateCursorState;
            _cursorMovementLimiter.FirstAdded -= UpdateCursorState;
            _cursorMovementLimiter.LastRemoved -= UpdateCursorState;
            _cursorMovementBlocker.FirstAdded -= UpdateCursorState;
            _cursorMovementBlocker.LastRemoved -= UpdateCursorState;
            _cursorStack.Clear();
        }

        #endregion


        #region Cursor State

        private void UpdateCursorState()
        {
            Visible = _cursorVisibilityBlocker.HasAny() is false;

            if (_cursorMovementBlocker.HasAny())
            {
                LockState = CursorLockMode.Locked;
                return;
            }

            if (_cursorMovementLimiter.HasAny())
            {
                LockState = CursorLockMode.Confined;
                return;
            }

            LockState = CursorLockMode.None;
        }

        #endregion


        #region Cursor Set

        public void SwitchActiveCursorSet(CursorSet cursorSet)
        {
            for (var i = 0; i < _cursorStack.Count; i++)
            {
                var cursorFile = _cursorStack[i];
                var cursorType = ActiveCursorSet.GetType(cursorFile);
                _cursorStack[i] = cursorSet.GetCursor(cursorType);
            }

            ActiveCursorSet = cursorSet;
            UpdateCursorFileInternal();
        }

        #endregion


        #region Cursor Style

        public void AddCursorOverride(CursorType cursorType)
        {
            var cursorById = ActiveCursorSet.GetCursor(cursorType);
            AddCursorOverride(cursorById);
        }

        public void RemoveCursorOverride(CursorType cursorType)
        {
            var cursorById = ActiveCursorSet.GetCursor(cursorType);
            RemoveCursorOverride(cursorById);
        }

        internal void AddCursorOverride(CursorFile file)
        {
            if (file == null)
            {
                return;
            }
            if (file == ActiveCursor)
            {
                return;
            }

            _cursorStack.PushUnique(file);
            UpdateCursorFileInternal();
        }

        internal void RemoveCursorOverride(CursorFile file)
        {
            var isActiveCursor = file == ActiveCursor;
            _cursorStack.Remove(file);
            if (isActiveCursor)
            {
                UpdateCursorFileInternal();
            }
        }

        private void UpdateCursorFileInternal()
        {
            switch (ActiveCursor)
            {
                case CursorTexture cursorTexture:
                    SetCursorTextureInternal(cursorTexture);
                    break;

                case CursorAnimation cursorAnimation:
                    SetCursorAnimationInternal(cursorAnimation);
                    break;

                default:
                    SetNullCursorInternal();
                    break;
            }
        }

        #endregion


        #region Cursor Style Internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetCursorTextureInternal(CursorTexture file)
        {
            StopCursorAnimation();
            UnityEngine.Cursor.SetCursor(file.Texture, file.HotSpot, file.CursorMode);
            CursorChanged?.Invoke(file);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetCursorAnimationInternal(CursorAnimation file)
        {
            RunCursorAnimation(file);
            CursorChanged?.Invoke(file);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetNullCursorInternal()
        {
            StopCursorAnimation();
            UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            CursorChanged?.Invoke(null);
        }

        #endregion


        #region Cursor Animation

        private Coroutine _cursorAnimation;
        private bool _hasFocus = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StopCursorAnimation()
        {
            if (_cursorAnimation.IsNotNull())
            {
                Gameloop.StopCoroutine(_cursorAnimation);
            }
        }

        private void RunCursorAnimation(CursorAnimation animationFile)
        {
            StopCursorAnimation();

            switch (animationFile.cursorAnimationType)
            {
                case CursorAnimationType.PlayOnce:
                    _cursorAnimation = Gameloop.StartCoroutine(PlayOnce(animationFile));
                    break;
                case CursorAnimationType.Loop:
                    _cursorAnimation = Gameloop.StartCoroutine(Loop(animationFile));
                    break;
                case CursorAnimationType.PingPong:
                    _cursorAnimation = Gameloop.StartCoroutine(PingPong(animationFile));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion


        #region Cursor Animations Coroutines

        private IEnumerator PlayOnce(CursorAnimation file)
        {
            var frames = file.frames;
            var timer = file.Delay;
            var hotSpot = file.HotSpot;
            var cursorMode = file.CursorMode;

            for (var i = 0; i < frames.Length; i++)
            {
                UnityEngine.Cursor.SetCursor(frames[i], hotSpot, cursorMode);
                yield return timer;
            }

            _cursorAnimation = null;
        }

        private IEnumerator Loop(CursorAnimation file)
        {
            var frames = file.frames;
            var timer = file.Delay;
            var hotSpot = file.HotSpot;
            var cursorMode = file.CursorMode;
            var frameCount = frames.Length;
            var currentFrame = 0;

            while (_hasFocus)
            {
                UnityEngine.Cursor.SetCursor(frames[currentFrame++], hotSpot, cursorMode);
                currentFrame = currentFrame == frameCount ? 0 : currentFrame;
                yield return timer;
            }
        }

        private IEnumerator PingPong(CursorAnimation file)
        {
            var frames = file.frames;
            var timer = file.Delay;
            var hotSpot = file.HotSpot;
            var cursorMode = file.CursorMode;
            var frameCount = frames.Length - 1;
            var increment = 1;
            var frame = 0;

            while (_hasFocus)
            {
                if (frame == frameCount)
                {
                    increment = -1;
                }
                else if (frame == 0)
                {
                    increment = 1;
                }

                UnityEngine.Cursor.SetCursor(frames[frame += increment], hotSpot, cursorMode);
                yield return timer;
            }
        }

        #endregion


        #region Application Focus

        [CallbackMethod(Segment.ApplicationFocus)]
        private void OnApplicationFocus(bool focus)
        {
            _hasFocus = focus;
            if (_hasFocus)
            {
                AddCursorOverride(ActiveCursor);
            }
        }

        #endregion
    }
}