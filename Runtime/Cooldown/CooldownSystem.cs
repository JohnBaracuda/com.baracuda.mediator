using Baracuda.Mediator.Callbacks;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Baracuda.Mediator.Cooldown
{
    public static class CooldownSystem
    {
        #region Public API

        [PublicAPI]
        public static void PauseAllCooldowns()
        {
            for (var index = cooldowns.Count - 1; index >= 0; index--)
            {
                cooldowns[index].Pause();
            }
        }

        [PublicAPI]
        public static void ResumeAllCooldowns()
        {
            for (var index = cooldowns.Count - 1; index >= 0; index--)
            {
                cooldowns[index].Resume();
            }
        }

        [PublicAPI]
        public static void CancelAllCooldowns()
        {
            for (var index = cooldowns.Count - 1; index >= 0; index--)
            {
                cooldowns[index].Cancel();
            }
        }

        [PublicAPI]
        public static void CompleteAllCooldowns()
        {
            for (var index = cooldowns.Count - 1; index >= 0; index--)
            {
                cooldowns[index].Complete();
            }
        }

        [PublicAPI]
        public static IEnumerator<ICooldown> GetAllActiveCooldowns()
        {
            for (var index = cooldowns.Count - 1; index >= 0; index--)
            {
                yield return cooldowns[index];
            }
        }

        #endregion


        #region Cooldown System

        private static readonly List<ICooldown> cooldowns = new();

        static CooldownSystem()
        {
            Gameloop.Update -= OnUpdate;
            Gameloop.Update += OnUpdate;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= OnEditorUpdate;
            UnityEditor.EditorApplication.update += OnEditorUpdate;
#endif
        }

        internal static void AddCooldown(ICooldown cooldownUpdate)
        {
            cooldowns.Add(cooldownUpdate);
        }

        internal static void RemoveCooldown(ICooldown cooldownUpdate)
        {
            cooldowns.Remove(cooldownUpdate);
        }

        private static void OnUpdate()
        {
            var deltaTime = Time.deltaTime;
            for (var index = cooldowns.Count - 1; index >= 0; index--)
            {
                cooldowns[index].UpdateCooldown(deltaTime);
            }
        }

#if UNITY_EDITOR
        private static void OnEditorUpdate()
        {
            if (Application.isPlaying is false)
            {
                OnUpdate();
            }
        }
#endif

        #endregion
    }
}