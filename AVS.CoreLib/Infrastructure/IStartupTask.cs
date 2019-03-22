namespace AVS.CoreLib.Infrastructure
{
    public interface IStartupTask
    {
        int Order { get; }
        void Execute();
    }
}