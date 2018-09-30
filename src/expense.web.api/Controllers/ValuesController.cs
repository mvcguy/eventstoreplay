using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate.Model;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.api.Values.Commands;
using expense.web.api.Values.Dtos;
using expense.web.api.Values.ReadModel;
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
        private readonly IReadModelRepository<ValueRecord> _readModelRepository;

        public ValuesController(IMediator mediator, IValuesRepository repository, IReadModelRepository<ValueRecord> readModelRepository)
        {
            _mediator = mediator;
            _repository = repository;
            _readModelRepository = readModelRepository;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var records = await Task.Run(() => _readModelRepository.GetAll().ToList());
            
            return Ok(records.Select(ToViewModel));
        }

        // GET api/values/17aeed42-3aa7-42a6-a01e-00de257dbb91/2
        [HttpGet("{id}/{version?}")]
        public IActionResult Get(Guid id, long? version = null)
        {
            var aggregate = _repository.GetById(id, version.GetValueOrDefault());
            if (aggregate == null) return NotFound("Item not found");

            return Ok(ToViewModel(aggregate));
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ValueViewModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
                var vm = ToViewModel(result.ValueAggregateModel);
                return Created($"~/api/values/{vm.Id}", vm);
            }
            else
            {
                return BadRequest(result);
            }


        }

        // PUT api/values/17aeed42-3aa7-42a6-a01e-00de257dbb91
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid? id, [FromBody] ValueViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!id.HasValue || id == Guid.Empty || vm == null || vm.Version == null || vm.Version.Value.GetValueOrDefault() == default(long))
                return BadRequest(vm);

            var updateCommand = new UpdateValueCommand
            {
                Id = id.GetValueOrDefault(),
                Version = vm.Version.Value.GetValueOrDefault(),
                UpdateNameCmd = vm.Name != null ? new UpdateNameChildCmd(vm.Name.Value) : null,
                UpdateValueCmd = vm.Value != null ? new UpdateValueChildCmd(vm.Value.Value) : null,
                // NOTE: Let the user not modify the code directly, 
                // TODO: We should provide an alternative way to modify the value code
                //UpdateCodeCmd = vm.Code != null ? new UpdateCodeChildCmd(vm.Code.Value) : null,
            };

            var result = await _mediator.Send(updateCommand);

            if (result.Success)
            {
                var updatedVm = ToViewModel(result.ValueAggregateModel);
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

        public ValueViewModel ToViewModel(ValueRecord record)
        {
            return new ValueViewModel()
            {
                Id = record.PublicId,
                TenantId = new DtoProp<int?>(record.TenantId),
                Name = new DtoProp<string>(record.Name),
                Code = new DtoProp<string>(record.Code),
                Value = new DtoProp<string>(record.Value),
                Version = new DtoProp<long?>(record.Version)
            };
        }

        public ValueViewModel ToViewModel(IValueAggregateModel aggregate)
        {
            return new ValueViewModel()
            {
                Id = aggregate.Id,
                TenantId = new DtoProp<int?>(aggregate.TenantId),
                Name = new DtoProp<string>(aggregate.Name),
                Code = new DtoProp<string>(aggregate.Code),
                Value = new DtoProp<string>(aggregate.Value),
                Version = new DtoProp<long?>(aggregate.Version)
            };
        }
    }


}
