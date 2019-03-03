using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quarks.CQRS;
using ReferenceModule.Models;
using ReferenceModule.Queries;
using Utils.CQRS;

namespace StarterApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IQueryDispatcher _queryDispatcher;

        public ValuesController(ILogger<ValuesController> logger, IQueryDispatcher queryDispatcher)
        {
            _logger = logger;
            _queryDispatcher = queryDispatcher;
            _logger.LogInformation("ctor");
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            _logger.LogInformation(nameof(Get));
            var result = await _queryDispatcher.DispatchAsync(new FetchDataQuery());
            return Ok(result);
        }

        // GET api/values/5
        [HttpGet("{id}", Name = nameof(GetById))]
        public ActionResult<Model> GetById(int id)
        {
            return null;
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<Model>> Post([FromBody] Model value)
        {
            _logger.LogInformation("creating...");
            var model = await _queryDispatcher.DispatchAsync(new CUDQuery<Model, Model>(value, CreateUpdateDeleteQuery.Create));
            return CreatedAtRoute(nameof(GetById), new {id = model.Id}, model);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
