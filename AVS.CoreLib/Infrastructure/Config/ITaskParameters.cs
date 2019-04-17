using System.Collections.Generic;

namespace AVS.CoreLib.Infrastructure.Config
{
    public interface ITaskParameters
    {
        void Init(Dictionary<string, string> args);

        bool HasAny { get; }
    }

}