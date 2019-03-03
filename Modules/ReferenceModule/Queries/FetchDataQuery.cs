using System;
using System.Collections.Generic;
using System.Text;
using Quarks.CQRS;
using ReferenceModule.Models;

namespace ReferenceModule.Queries
{
    public class FetchDataQuery : IQuery<IEnumerable<Model>>
    {
    }
}
