namespace Baracuda.Bedrock.Countdown
{
    public interface ICountdownDurationModifier
    {
        public void ModifyCooldownDuration(ref float totalDurationInSeconds, float unmodifiedTotalDurationInSeconds);
    }
}