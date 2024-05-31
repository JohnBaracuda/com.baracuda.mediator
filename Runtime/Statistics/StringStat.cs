namespace Baracuda.Bedrock.Statistics
{
    public class StringStat : StatAsset<string>
    {
        protected override string DefaultValue()
        {
            return string.Empty;
        }

        public override Modification Type()
        {
            return Modification.Update;
        }

        public override string ValueString => Value;
    }
}