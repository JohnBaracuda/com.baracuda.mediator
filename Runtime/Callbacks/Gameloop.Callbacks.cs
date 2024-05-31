using Baracuda.Utilities.Reflection;
using Baracuda.Utilities.Types;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Baracuda.Bedrock.Callbacks
{
    public partial class Gameloop
    {
        #region Fields

        private const int Capacity4 = 4;
        private const int Capacity8 = 8;
        private const int Capacity16 = 16;
        private const int Capacity32 = 32;
        private const int Capacity64 = 64;
        private const int Capacity128 = 128;
        private const int Capacity256 = 256;
        private static readonly LogCategory logCategory = nameof(Gameloop);

        private static readonly HashSet<Object> registeredObjects = new(Capacity256);
        private static readonly Dictionary<Type, CallbackMethodInfo> callbackMethodInfoCache = new(Capacity128);

        private static readonly List<Action> initializationCompletedCallbacks = new(Capacity64);
        private static readonly List<Action> beforeFirstSceneLoadCallbacks = new(Capacity16);
        private static readonly List<Action> afterFirstSceneLoadCallbacks = new(Capacity16);
        private static readonly List<Action> preUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> postUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> preLateUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> postLateUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> updateCallbacks = new(Capacity64);
        private static readonly List<Action> lateUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> fixedUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> slowTickUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> tickUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> applicationQuitCallbacks = new(Capacity32);
        private static readonly List<Action> firstUpdateCallbacks = new(Capacity8);
        private static readonly List<Action<bool>> applicationFocusCallbacks = new(Capacity16);
        private static readonly List<Action<bool>> applicationPauseCallbacks = new(Capacity16);
        private static readonly List<Func<Task>> asyncShutdownCallbacks = new(Capacity16);
        private static readonly Dictionary<string, List<Action>> customCallbacks = new();
        private static MonoBehaviour monoBehaviour;

        private static Timer OneSecondTimer { get; set; } = Timer.None;
        private static Timer TickTimer { get; set; }

        public delegate void WillDeleteAssetCallback(string assetPath, Object asset);
#pragma warning disable
        public static event WillDeleteAssetCallback BeforeDeleteAsset;
#pragma warning restore
#if UNITY_EDITOR
        private static readonly List<Action> editorUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> exitPlayModeDelegate = new(Capacity32);
        private static readonly List<Action> enterPlayModeDelegate = new(Capacity32);
        private static readonly List<Action> exitEditModeDelegate = new(Capacity32);
        private static readonly List<Action> enterEditModeDelegate = new(Capacity32);
        private static readonly List<Action<BuildReportData>> buildPreprocessorCallbacks = new(Capacity4);
        private static readonly List<Action<BuildReportData>> buildPostprocessorCallbacks = new(Capacity4);
#endif

        #endregion


        #region Callbacks

        private static void UnregisterInternal(Object target)
        {
            Profiler.BeginSample("Gameloop.UnregisterInternal");

            var wasRemoved = registeredObjects.Remove(target);
            if (wasRemoved is false)
            {
                Profiler.EndSample();
                return;
            }

            RemoveCallbacksFromList(updateCallbacks, target);
            RemoveCallbacksFromList(preUpdateCallbacks, target);
            RemoveCallbacksFromList(postUpdateCallbacks, target);
            RemoveCallbacksFromList(preLateUpdateCallbacks, target);
            RemoveCallbacksFromList(postLateUpdateCallbacks, target);
            RemoveCallbacksFromList(lateUpdateCallbacks, target);
            RemoveCallbacksFromList(fixedUpdateCallbacks, target);
            RemoveCallbacksFromList(applicationQuitCallbacks, target);
            RemoveCallbacksFromList(applicationFocusCallbacks, target);
            RemoveCallbacksFromList(applicationPauseCallbacks, target);
            RemoveCallbacksFromList(asyncShutdownCallbacks, target);
            RemoveCallbacksFromList(afterFirstSceneLoadCallbacks, target);
            RemoveCallbacksFromList(beforeFirstSceneLoadCallbacks, target);
            RemoveCallbacksFromList(initializationCompletedCallbacks, target);

            foreach (var list in customCallbacks.Values)
            {
                RemoveCallbacksFromList(list, target);
            }

#if UNITY_EDITOR
            RemoveCallbacksFromList(enterEditModeDelegate, target);
            RemoveCallbacksFromList(exitEditModeDelegate, target);
            RemoveCallbacksFromList(enterPlayModeDelegate, target);
            RemoveCallbacksFromList(exitPlayModeDelegate, target);
            RemoveCallbacksFromList(buildPreprocessorCallbacks, target);
#endif

            Profiler.EndSample();
        }

        private static void RegisterInternal(Object target)
        {
            Profiler.BeginSample("Gameloop.RegisterInternal");

            var wasAdded = registeredObjects.Add(target);
            if (wasAdded is false)
            {
                Profiler.EndSample();
                return;
            }

            var type = target.GetType();
            var callbackMethodInfo = GenerateCallbackMethodInfo(type);

            if (callbackMethodInfo.HasMethods is false)
            {
                Profiler.EndSample();
                return;
            }

            foreach (var (segment, methodInfo) in callbackMethodInfo.SegmentMethods)
            {
                try
                {
                    CreateDelegateCallbacks(target, segment, methodInfo);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            foreach (var (segment, methodInfo) in callbackMethodInfo.CustomMethods)
            {
                try
                {
                    CreatDelegateCustomCallbacks(target, segment, methodInfo);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            Profiler.EndSample();
        }

        private static void CreatDelegateCustomCallbacks(Object target, string callbackName, MethodInfo methodInfo)
        {
            if (!customCallbacks.TryGetValue(callbackName, out var list))
            {
                list = new List<Action>(Capacity16);
                customCallbacks.Add(callbackName, list);
            }

            var callback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
            list.Add(callback);
        }

        private static void CreateDelegateCallbacks(Object target, Segment segment, MethodInfo methodInfo)
        {
            switch (segment)
            {
                case Segment.None:
                case Segment.Custom:
                    break;

                case Segment.Update:
                    var updateCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    updateCallbacks.Add(updateCallback);
                    break;

                case Segment.LateUpdate:
                    var lateUpdateCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    lateUpdateCallbacks.Add(lateUpdateCallback);
                    break;

                case Segment.FixedUpdate:
                    var fixedUpdateCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    fixedUpdateCallbacks.Add(fixedUpdateCallback);
                    break;

                case Segment.ApplicationQuit:
                    var applicationQuitCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    applicationQuitCallbacks.Add(applicationQuitCallback);
                    break;

                case Segment.ApplicationFocus:
                    var applicationFocusCallback =
                        (Action<bool>) methodInfo.CreateDelegate(typeof(Action<bool>), target);
                    applicationFocusCallbacks.Add(applicationFocusCallback);
                    break;

                case Segment.ApplicationPause:
                    var applicationPauseCallback =
                        (Action<bool>) methodInfo.CreateDelegate(typeof(Action<bool>), target);
                    applicationPauseCallbacks.Add(applicationPauseCallback);
                    break;

                case Segment.AsyncShutdown:
                    var asyncShutdownCallback = (Func<Task>) methodInfo.CreateDelegate(typeof(Func<Task>), target);
                    asyncShutdownCallbacks.Add(asyncShutdownCallback);
                    break;

                case Segment.FirstUpdate:
                    var firstUpdateCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    firstUpdateCallbacks.Add(firstUpdateCallback);
                    break;

                case Segment.InitializationCompleted:
                    var initializationCompletedCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    initializationCompletedCallbacks.Add(initializationCompletedCallback);
                    if (InitializationCompletedState)
                    {
                        initializationCompletedCallback();
                    }
                    break;

                case Segment.BeforeFirstSceneLoad:
                    var beforeFirstSceneLoadCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    beforeFirstSceneLoadCallbacks.Add(beforeFirstSceneLoadCallback);
                    if (BeforeSceneLoadCompleted)
                    {
                        beforeFirstSceneLoadCallback();
                    }
                    break;

                case Segment.AfterFirstSceneLoad:
                    var afterFirstSceneLoadCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    afterFirstSceneLoadCallbacks.Add(afterFirstSceneLoadCallback);
                    if (AfterSceneLoadCompleted)
                    {
                        afterFirstSceneLoadCallback();
                    }
                    break;

                case Segment.PreUpdate:
                    var preUpdateCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    preUpdateCallbacks.Add(preUpdateCallback);
                    break;

                case Segment.PostUpdate:
                    var postUpdateCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    postUpdateCallbacks.Add(postUpdateCallback);
                    break;

                case Segment.PreLateUpdate:
                    var preLateUpdateCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    preLateUpdateCallbacks.Add(preLateUpdateCallback);
                    break;

                case Segment.PostLateUpdate:
                    var postLateUpdateCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    postLateUpdateCallbacks.Add(postLateUpdateCallback);
                    break;

#if UNITY_EDITOR
                case Segment.EditorUpdate:
                    var editorUpdateCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    editorUpdateCallbacks.Add(editorUpdateCallback);
                    break;

                case Segment.EnteredEditMode:
                    var enteredEditModeCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    enterEditModeDelegate.Add(enteredEditModeCallback);
                    break;

                case Segment.ExitingEditMode:
                    var exitingEditModeCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    exitEditModeDelegate.Add(exitingEditModeCallback);
                    break;

                case Segment.EnteredPlayMode:
                    var enteredPlayModeCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    enterPlayModeDelegate.Add(enteredPlayModeCallback);
                    break;

                case Segment.ExitingPlayMode:
                    var exitingPlayModeCallback = (Action) methodInfo.CreateDelegate(typeof(Action), target);
                    exitPlayModeDelegate.Add(exitingPlayModeCallback);
                    break;

                case Segment.BuildPreprocessor:
                    var buildPreprocessor =
                        (Action<BuildReportData>) methodInfo.CreateDelegate(typeof(Action<BuildReportData>), target);
                    buildPreprocessorCallbacks.Add(buildPreprocessor);
                    break;

                case Segment.BuildPostprocessor:
                    var buildPostprocessor =
                        (Action<BuildReportData>) methodInfo.CreateDelegate(typeof(Action<BuildReportData>), target);
                    buildPostprocessorCallbacks.Add(buildPostprocessor);
                    break;
#endif
            }
        }

        private static void RemoveCallbacksFromList<T>(IList<T> list, Object target) where T : Delegate
        {
            for (var index = list.Count - 1; index >= 0; index--)
            {
                if (ReferenceEquals(list[index].Target, target))
                {
                    list.RemoveAt(index);
                }
            }
        }

        private static CallbackMethodInfo GenerateCallbackMethodInfo(Type type)
        {
            if (!callbackMethodInfoCache.TryGetValue(type, out var callbackMethodInfo))
            {
                callbackMethodInfo = CallbackMethodInfo.Create(type);
                callbackMethodInfoCache.Add(type, callbackMethodInfo);
            }

            return callbackMethodInfo;
        }

        private struct CallbackMethodInfo
        {
            public bool HasMethods { get; set; }

            public List<(Segment segment, MethodInfo methodInfo)> SegmentMethods;
            public List<(string callback, MethodInfo methodInfo)> CustomMethods;

            public static CallbackMethodInfo Create(Type type)
            {
                var methods = type.GetMethodsIncludeBaseTypes(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly |
                    BindingFlags.FlattenHierarchy);

                var data = new CallbackMethodInfo
                {
                    SegmentMethods = new List<(Segment segment, MethodInfo methodInfo)>(),
                    CustomMethods = new List<(string callback, MethodInfo methodInfo)>()
                };

                foreach (var methodInfo in methods)
                {
                    var attributes = methodInfo.GetCustomAttributes<CallbackMethodAttribute>(true);
                    foreach (var attribute in attributes)
                    {
                        if (attribute.Segment == Segment.Custom)
                        {
                            data.HasMethods = true;
                            data.CustomMethods.Add((attribute.Custom, methodInfo));
                        }
                        else
                        {
                            data.HasMethods = true;
                            data.SegmentMethods.Add((attribute.Segment, methodInfo));
                        }
                    }
                }

                return data;
            }
        }

        #endregion


        #region Shutdown

        private static async Task ShutdownAsyncInternal()
        {
            var buffer = new Task[asyncShutdownCallbacks.Count];
            for (var index = 0; index < asyncShutdownCallbacks.Count; index++)
            {
                buffer[index] = asyncShutdownCallbacks[index]();
            }

            await Task.WhenAll(buffer);

            Debug.Log("Gameloop", "Quitting Application");
            EnableApplicationQuit = true;
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
        }

        #endregion
    }
}