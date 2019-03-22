
namespace AVS.CoreLib.Data.Events
{
    public interface IConsumer<T>
    {
        void HandleEvent(T eventMessage);
    }
}
