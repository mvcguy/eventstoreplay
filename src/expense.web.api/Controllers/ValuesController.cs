﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate;
using expense.web.api.Values.Aggregate.Model;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.api.Values.Commands;
using expense.web.api.Values.Commands.Value;
using expense.web.api.Values.Dtos;
using expense.web.api.Values.ReadModel;
using expense.web.api.Values.ReadModel.Schema;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace expense.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ValuesBaseController
    {
        private readonly IMediator _mediator;
        private readonly IRepository<ValuesRootAggregate> _rootAggregateRepository;
        private readonly IReadModelRepository<ValueRecord> _readModelRepository;

        public ValuesController(IMediator mediator, IRepository<ValuesRootAggregate> rootAggregateRepository, IReadModelRepository<ValueRecord> readModelRepository)
        {
            _mediator = mediator;
            _rootAggregateRepository = rootAggregateRepository;
            _readModelRepository = readModelRepository;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber = 1)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            var totalPerPage = 5;

            var take = totalPerPage;
            var skip = (pageNumber - 1) * totalPerPage;

            var count = await Task.Run(() => _readModelRepository.GetAll().Count());
            var totalPages = Math.Ceiling(count / (double)totalPerPage);

            var records = await Task.Run(() => _readModelRepository.GetAll().Skip(skip).Take(take).ToList().OrderByDescending(x => x.LastModifiedOn));

            return Ok(new { valuesList = records.Select(ToViewModel), totalPages });
        }

        // GET api/values/17aeed42-3aa7-42a6-a01e-00de257dbb91/2
        [HttpGet("{id}/{version?}")]
        public IActionResult Get(Guid id, long? version = null)
        {
            // TODO: Use mongo db instead of reading from eventstore.
            var aggregate = _rootAggregateRepository.GetById(id, version.GetValueOrDefault());
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
                TenantId = request.TenantId?.Value,
                Name = request.Name.Value,
                Code = request.Code.Value,
                Value = request.Value.Value
            };

            var result = await _mediator.Send(createCommand);

            if (result.Success)
            {
                var vm = ToViewModel(result.Model);
                return Created($"~/api/values/{vm.Id}", vm);
            }
            else
            {
                return CreateBadRequestResult(result);
            }
        }


        // PUT api/values/17aeed42-3aa7-42a6-a01e-00de257dbb91
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid? id, [FromBody] ValueViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!id.HasValue || id == Guid.Empty || vm == null || vm.Version == null)
                return CreateInvalidRequestResult();

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
                var updatedVm = ToViewModel(result.Model);
                return Ok(updatedVm);
            }
            else
            {
                return CreateBadRequestResult(result);
            }
        }



        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

    }


}
