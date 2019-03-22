using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Data;
using AVS.CoreLib.Data.Domain.Logging;
using AVS.CoreLib.Data.Events;
using AVS.CoreLib.Data.Services;

namespace AVS.CoreLib.Services.Logging
{
    public interface ILogEntityService : IEntityServiceBase<Log>
    {
        Log GetLastLog();
        List<Log> GetLastLogs(int count);
    }

    public class LogEntityService : EntityServiceBase<Log>, ILogEntityService
    {
        public LogEntityService(IRepository<Log> repository, IEventPublisher eventPublisher) : base(repository, eventPublisher)
        {
        }

        public List<Log> GetLastLogs(int count)
        {
            return Repository.Table.OrderByDescending(l => l.CreatedOnUtc).Take(count).ToList();
        }

        public Log GetLastLog()
        {
            return Repository.Table.OrderByDescending(l => l.CreatedOnUtc).FirstOrDefault();
        }
    }
}