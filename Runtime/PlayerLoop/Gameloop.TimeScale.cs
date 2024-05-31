using System.Collections.Generic;
using UnityEngine;

namespace Baracuda.Bedrock.PlayerLoop
{
    public delegate void TimeScaleDelegate(ref float timeScale);

    public partial class Gameloop
    {
        private static readonly List<TimeScaleDelegate> timeScaleModifier = new();

        private static void UpdateTimeScale()
        {
            if (ControlTimeScale is false)
            {
                return;
            }

            var timeScale = 1f;
            foreach (var timeScaleDelegate in timeScaleModifier)
            {
                timeScaleDelegate(ref timeScale);
            }

            Time.timeScale = timeScale;
        }
    }
}