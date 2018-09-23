using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.api.Values.Commands;
using expense.web.api.Values.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace expense.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValuesRepository _repository;

        public ValuesController(IMediator mediator, IValuesRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}/{version?}")]
        public IActionResult Get(Guid id, long? version = null)
        {
            var aggregate = _repository.GetById(id, version.GetValueOrDefault());
            if (aggregate == null) return NotFound("Item not found");

            var vm = new ValueViewModel()
            {
                Id = aggregate.Id,
                TenantId = new DtoProp<int>(aggregate.TenantId),
                Name = new DtoProp<string>(aggregate.Name),
                Code = new DtoProp<string>(aggregate.Code),
                Value = new DtoProp<string>(aggregate.Value),
                Version = new DtoProp<long>(aggregate.Version)
            };

            return Ok(vm);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ValueViewModel request)
        {
            var createCommand = new CreateValueCommand
            {
                Request = new ValueCommandRequest()
                {
                    TenantId = request.TenantId?.Value,
                    Name = request.Name.Value,
                    Code = request.Code.Value,
                    Value = request.Value.Value

                }
            };

            var result = await _mediator.Send(createCommand);

            if (result.Success)
            {
                var aggregate = result.ValueAggregateModel;
                var vm = new ValueViewModel()
                {
                    Id = aggregate.Id,
                    TenantId = new DtoProp<int>(aggregate.TenantId),
                    Name = new DtoProp<string>(aggregate.Name),
                    Code = new DtoProp<string>(aggregate.Code),
                    Value = new DtoProp<string>(aggregate.Value),
                    Version = new DtoProp<long>(aggregate.Version)
                };
                return Created($"~/api/values/{vm.Id}", vm);
            }
            else
            {
                return BadRequest(result);
            }


        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] ValueViewModel vm)
        {

            if (id == Guid.Empty || vm == null || vm.Version == null || vm.Version.Value == default(long))
                return BadRequest(vm);

            var updateCommand = new UpdateValueCommand
            {
                Id = id,
                Version = vm.Version.Value,
                UpdateNameCmd = vm.Name != null ? new UpdateNameChildCmd(vm.Name.Value) : null,
                UpdateValueCmd = vm.Value != null ? new UpdateValueChildCmd(vm.Value.Value) : null,
                UpdateCodeCmd = vm.Code != null ? new UpdateCodeChildCmd(vm.Code.Value) : null,
            };

            var result = await _mediator.Send(updateCommand);

            if (result.Success)
            {
                var aggregate = result.ValueAggregateModel;
                var updatedVm = new ValueViewModel()
                {
                    Id = aggregate.Id,
                    TenantId = new DtoProp<int>(aggregate.TenantId),
                    Name = new DtoProp<string>(aggregate.Name),
                    Code = new DtoProp<string>(aggregate.Code),
                    Value = new DtoProp<string>(aggregate.Value),
                    Version = new DtoProp<long>(aggregate.Version)
                };
                return Ok(updatedVm);
            }
            else
            {
                return BadRequest(result);
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class ValuesViewModel
    {

    }
}
