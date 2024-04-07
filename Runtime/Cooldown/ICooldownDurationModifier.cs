namespace Baracuda.Mediator.Cooldown
{
    public interface ICooldownDurationModifier
    {
        public void ModifyCooldownDuration(ref float totalDurationInSeconds, float unmodifiedTotalDurationInSeconds);
    }
}