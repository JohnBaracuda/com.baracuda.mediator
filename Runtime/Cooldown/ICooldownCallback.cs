namespace Baracuda.Mediator.Cooldown
{
    public interface ICooldownCallback
    {
        /// <summary>
        ///     Called on the callback receiver when the cooldown is started.
        /// </summary>
        public void OnBeginCooldown();

        /// <summary>
        ///     Called on the callback receiver when the cooldown has completed.
        /// </summary>
        public void OnEndCooldown();

        /// <summary>
        ///     Called on the callback receiver every update while the cooldown is running.
        /// </summary>
        /// <param name="remainingTime">seconds remaining</param>
        /// <param name="delta">delta value between the cooldowns start and end time</param>
        public void OnCooldownUpdate(float remainingTime, float delta)
        {
        }
    }

    /// <summary>
    ///     Optional cooldown interface to receive a callback when the cooldown is cancelled.
    /// </summary>
    public interface IOnCooldownCancelled
    {
        /// <summary>
        ///     Called on the receiver object when the cooldown is cancelled.
        /// </summary>
        public void OnCooldownCancelled();
    }
}