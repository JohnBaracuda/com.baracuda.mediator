namespace Baracuda.Bedrock.Statistics
{
    public enum Modification
    {
        /// <summary>
        ///     Set the current value for the stat.
        /// </summary>
        Update = 0,

        /// <summary>
        ///     Increments the current value by 1 or a given amount
        /// </summary>
        Increment = 1,

        /// <summary>
        ///     Updates the current value to the passed value if the passed value is greater. Used for high scores like
        ///     kill counts etc.
        /// </summary>
        Highscore = 2,

        /// <summary>
        ///     Updates the current value to the passed value if the passed value is lower. Used for minimal scores
        ///     like level completion time etc.
        /// </summary>
        Minimal = 3
    }
}