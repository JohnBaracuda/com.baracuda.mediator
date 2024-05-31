namespace Baracuda.Bedrock.States
{
    public interface IStateObject
    {
        public void OnStateEnter();

        public void OnStateExit();

        public void OnStateUpdate()
        {
        }
    }
}