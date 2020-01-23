using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.Data.EF
{
    public class DummyDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public DummyDbAsyncEnumerator(List<T> items)
        {
            _enumerator = items.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_enumerator.MoveNext());
        }

        public T Current => _enumerator.Current;

        object IDbAsyncEnumerator.Current => Current;
    }
}