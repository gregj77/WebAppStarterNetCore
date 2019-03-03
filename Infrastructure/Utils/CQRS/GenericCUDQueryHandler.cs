using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quarks.CQRS;

namespace Utils.CQRS
{
    public abstract class GenericCUDQueryHandler<TPayload, TData> : IQueryHandler<TPayload, TData> where TPayload : CUDQuery<TData, TData>
    {
        public async Task<TData> HandleAsync(TPayload query, CancellationToken cancellationToken)
        {
            switch (query.QueryType)
            {
                case CreateUpdateDeleteQuery.Create:
                    return await OnCreate(query.Payload);

                case CreateUpdateDeleteQuery.Update:
                    return await OnUpdate(query.Payload);

                case CreateUpdateDeleteQuery.Delete:
                    return await OnDelete(query.Payload);
            }

            throw new NotSupportedException();
        }

        protected abstract Task<TData> OnCreate(TData payload);

        protected abstract Task<TData> OnUpdate(TData payload);

        protected abstract Task<TData> OnDelete(TData payload);
    }
}
