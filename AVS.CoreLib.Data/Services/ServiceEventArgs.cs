using System;


namespace AVS.CoreLib.Data.Services
{
    public class ServiceEventArgs<T> : EventArgs
        where T : BaseEntity
    {
        private readonly T _entity;

        public ServiceEventArgs(T entity)
        {
            _entity = entity;
        }

        public T Entity
        {
            get { return _entity; }
        }
    }
}