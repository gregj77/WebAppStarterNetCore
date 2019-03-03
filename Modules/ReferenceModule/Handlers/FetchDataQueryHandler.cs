using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quarks.CQRS;
using ReferenceModule.Data;
using ReferenceModule.Models;
using ReferenceModule.Queries;

namespace ReferenceModule.Handlers
{
    class FetchDataQueryHandler : IQueryHandler<FetchDataQuery, IEnumerable<Model>>
    {
        private readonly ILogger<FetchDataQueryHandler> _logger;
        private readonly IModuleContext _context;

        public FetchDataQueryHandler(ILogger<FetchDataQueryHandler> logger, IModuleContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<Model>> HandleAsync(FetchDataQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("running....");

            await Task.Delay(1000, cancellationToken);
            return await _context.Models.ToListAsync(cancellationToken);
        }
    }
}
