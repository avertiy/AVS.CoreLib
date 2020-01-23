using Newtonsoft.Json;

namespace AVS.CoreLib._System.Net
{
    public interface IResponse
    {
        string Error { get; }
        bool Success { get; }
    }

    public interface IResponse<T>: IResponse
    {
        T Data { get; set; }
    }
}