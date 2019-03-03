using System;
using System.Collections.Generic;
using System.Text;
using Quarks.CQRS;

namespace Utils.CQRS
{
    public class CUDQuery<TData, TResult> : IQuery<TResult>
    {
        private readonly TData _payload;
        private readonly CreateUpdateDeleteQuery _queryType;

        public CUDQuery(TData payload, CreateUpdateDeleteQuery queryType)
        {
            _payload = payload;
            _queryType = queryType;
        }

        public TData Payload => _payload;

        public CreateUpdateDeleteQuery QueryType => _queryType;
    }
}
