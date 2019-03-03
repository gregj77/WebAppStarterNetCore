using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quarks.CQRS;
using ReferenceModule.Data;
using ReferenceModule.Models;
using Utils.CQRS;

namespace ReferenceModule.Handlers
{
    class ManageModelQueryHandler : GenericCUDQueryHandler<CUDQuery<Model, Model>, Model>
    {
        private readonly ILogger<ManageModelQueryHandler> _logger;
        private readonly IModuleContext _ctx;

        public ManageModelQueryHandler(ILogger<ManageModelQueryHandler> logger, IModuleContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        protected override async Task<Model> OnCreate(Model payload)
        {
            var id = await _ctx.CreateModel(payload);
            payload.Id = id;
            return payload;
        }

        protected override Task<Model> OnUpdate(Model payload)
        {
            throw new NotImplementedException();
        }

        protected override Task<Model> OnDelete(Model payload)
        {
            throw new NotImplementedException();
        }
    }
}
