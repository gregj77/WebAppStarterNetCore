using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReferenceModule.Models;

namespace ReferenceModule.Data
{
    internal class ModuleContext : DbContext, IModuleContext
    {
        private readonly ILogger<ModuleContext> _logger;

        public ModuleContext(DbContextOptions options, ILogger<ModuleContext> logger) : base(options)
        {
            _logger = logger;
            _logger.LogInformation("ctor");
        }

        public DbSet<Model> Models { get; set; }
        public async Task<int> CreateModel(Model model)
        {
            var entityEntry = await Models.AddAsync(model);
            await SaveChangesAsync();
            return entityEntry.Entity.Id;
        }

        public Task<Model> UpdateModel(Model model)
        {
            throw new NotImplementedException();
        }

        public Task<Model> DeleteModel(Model model)
        {
            throw new NotImplementedException();
        }

        IQueryable<Model> IModuleContext.Models => Models;

        public override void Dispose()
        {
            _logger.LogInformation("dispose");
            base.Dispose();
        }
    }
}
