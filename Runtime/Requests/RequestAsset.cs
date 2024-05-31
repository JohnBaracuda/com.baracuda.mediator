using Baracuda.Bedrock.Mediator;
using Baracuda.Bedrock.PlayerLoop;
using Sirenix.OdinInspector;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Bedrock.Requests
{
    public class RequestAsset : MediatorAsset
    {
        [Tooltip("When enabled, the responder is automatically cleared when entering edit mode")]
        [SerializeField] private bool clearResponder;
        [SerializeField] private bool logUnansweredResponses;

        [ReadOnly] private Action _responder;


        #region API

        public bool HasResponder => _responder != null;

        [Button]
        private void DebugRequest()
        {
            Request();
        }

        public Response Request()
        {
            return RequestInternal();
        }

        public void AddResponder(Action responder)
        {
            _responder = responder;
        }

        public void RemoveResponder(Action responder)
        {
            if (_responder == responder)
            {
                _responder = null;
            }
        }

        public void ClearResponder()
        {
            _responder = null;
        }

        #endregion


        #region Internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Response RequestInternal()
        {
            if (_responder == null)
            {
                if (logUnansweredResponses)
                {
                    Debug.Log("Request", $"Request for {this} was left unanswered!", this);
                }
                return new Response(ResponseType.Unanswered);
            }
            try
            {
                _responder();
                return new Response(ResponseType.Answered);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return new Response(ResponseType.Faulted);
            }
        }

        #endregion


#if UNITY_EDITOR
        [CallbackOnEnterEditMode]
        private void OnEnterEditMode()
        {
            if (clearResponder)
            {
                ClearResponder();
            }
        }
#endif
    }

    public abstract class RequestAsset<T> : MediatorAsset
    {
        [Tooltip("When enabled, the responder is automatically cleared when entering edit mode")]
        [SerializeField] private bool clearResponder;
        [SerializeField] private bool logUnansweredResponses;

        [ReadOnly] private Func<T> _responder;


        #region API

        public bool HasResponder => _responder != null;

        public Response<T> Request()
        {
            return RequestInternal();
        }

        public void AddResponder(Func<T> responder)
        {
            _responder = responder;
        }

        public void RemoveResponder(Func<T> responder)
        {
            if (_responder == responder)
            {
                _responder = null;
            }
        }

        public void ClearResponder()
        {
            _responder = null;
        }

        #endregion


        #region Internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Response<T> RequestInternal()
        {
            if (_responder == null)
            {
                if (logUnansweredResponses)
                {
                    Debug.Log("Request", $"Request for {this} was left unanswered!", this);
                }
                return new Response<T>(ResponseType.Unanswered, default(T));
            }
            try
            {
                var result = _responder();
                return new Response<T>(ResponseType.Answered, result);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return new Response<T>(ResponseType.Faulted, default(T));
            }
        }

        #endregion


#if UNITY_EDITOR
        [CallbackOnEnterEditMode]
        private void OnEnterEditMode()
        {
            if (clearResponder)
            {
                ClearResponder();
            }
        }
#endif
    }

    public abstract class RequestAsset<T1, T2> : MediatorAsset
    {
        [Tooltip("When enabled, the responder is automatically cleared when entering edit mode")]
        [SerializeField] private bool clearResponder;
        [SerializeField] private bool logUnansweredResponses;

        [ReadOnly] private Func<(T1, T2)> _responder;


        #region API

        public bool HasResponder => _responder != null;

        public Response<T1, T2> Request()
        {
            return RequestInternal();
        }

        public void AddResponder(Func<(T1, T2)> responder)
        {
            _responder = responder;
        }

        public void RemoveResponder(Func<(T1, T2)> responder)
        {
            if (_responder == responder)
            {
                _responder = null;
            }
        }

        public void ClearResponder()
        {
            _responder = null;
        }

        #endregion


        #region Internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Response<T1, T2> RequestInternal()
        {
            if (_responder == null)
            {
                if (logUnansweredResponses)
                {
                    Debug.Log("Request", $"Request for {this} was left unanswered!", this);
                }
                return new Response<T1, T2>(ResponseType.Unanswered, default(T1), default(T2));
            }
            try
            {
                var result = _responder();
                return new Response<T1, T2>(ResponseType.Answered, result.Item1, result.Item2);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return new Response<T1, T2>(ResponseType.Faulted, default(T1), default(T2));
            }
        }

        #endregion


#if UNITY_EDITOR
        [CallbackOnEnterEditMode]
        private void OnEnterEditMode()
        {
            if (clearResponder)
            {
                ClearResponder();
            }
        }
#endif
    }

    public abstract class RequestAsset<T1, T2, T3> : MediatorAsset
    {
        [Tooltip("When enabled, the responder is automatically cleared when entering edit mode")]
        [SerializeField] private bool clearResponder;
        [SerializeField] private bool logUnansweredResponses;

        [ReadOnly] private Func<(T1, T2, T3)> _responder;


        #region API

        public bool HasResponder => _responder != null;

        public Response<T1, T2, T3> Request()
        {
            return RequestInternal();
        }

        public void AddResponder(Func<(T1, T2, T3)> responder)
        {
            _responder = responder;
        }

        public void RemoveResponder(Func<(T1, T2, T3)> responder)
        {
            if (_responder == responder)
            {
                _responder = null;
            }
        }

        public void ClearResponder()
        {
            _responder = null;
        }

        #endregion


        #region Internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Response<T1, T2, T3> RequestInternal()
        {
            if (_responder == null)
            {
                if (logUnansweredResponses)
                {
                    Debug.Log("Request", $"Request for {this} was left unanswered!", this);
                }
                return new Response<T1, T2, T3>(ResponseType.Unanswered, default(T1), default(T2), default(T3));
            }
            try
            {
                var result = _responder();
                return new Response<T1, T2, T3>(ResponseType.Answered, result.Item1, result.Item2, result.Item3);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return new Response<T1, T2, T3>(ResponseType.Faulted, default(T1), default(T2), default(T3));
            }
        }

        #endregion


#if UNITY_EDITOR
        [CallbackOnEnterEditMode]
        private void OnEnterEditMode()
        {
            if (clearResponder)
            {
                ClearResponder();
            }
        }
#endif
    }

    public abstract class RequestAsset<T1, T2, T3, T4> : MediatorAsset
    {
        [Tooltip("When enabled, the responder is automatically cleared when entering edit mode")]
        [SerializeField] private bool clearResponder;
        [SerializeField] private bool logUnansweredResponses;

        [ReadOnly] private Func<(T1, T2, T3, T4)> _responder;


        #region API

        public bool HasResponder => _responder != null;

        public Response<T1, T2, T3, T4> Request()
        {
            return RequestInternal();
        }

        public void AddResponder(Func<(T1, T2, T3, T4)> responder)
        {
            _responder = responder;
        }

        public void RemoveResponder(Func<(T1, T2, T3, T4)> responder)
        {
            if (_responder == responder)
            {
                _responder = null;
            }
        }

        public void ClearResponder()
        {
            _responder = null;
        }

        #endregion


        #region Internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Response<T1, T2, T3, T4> RequestInternal()
        {
            if (_responder == null)
            {
                if (logUnansweredResponses)
                {
                    Debug.Log("Request", $"Request for {this} was left unanswered!", this);
                }
                return new Response<T1, T2, T3, T4>(ResponseType.Unanswered, default(T1), default(T2), default(T3),
                    default(T4));
            }
            try
            {
                var result = _responder();
                return new Response<T1, T2, T3, T4>(ResponseType.Answered, result.Item1, result.Item2, result.Item3,
                    result.Item4);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return new Response<T1, T2, T3, T4>(ResponseType.Faulted, default(T1), default(T2), default(T3),
                    default(T4));
            }
        }

        #endregion


#if UNITY_EDITOR
        [CallbackOnEnterEditMode]
        private void OnEnterEditMode()
        {
            if (clearResponder)
            {
                ClearResponder();
            }
        }
#endif
    }
}