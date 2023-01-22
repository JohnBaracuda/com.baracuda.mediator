using UnityEngine;

namespace Baracuda.Mediator
{
    public class ColorEvent : EventBroadcast<Color>
    {
        // Disable warning to hide base.Raise with custom parameter name.
#pragma warning disable 109

        public new void Raise(Color color)
        {
            base.Raise(color);
        }
    }
}