using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.api.Values.Commands.Comments;
using expense.web.api.Values.Dtos;
using expense.web.api.Values.ReadModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace expense.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValueCommentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<ValuesRootAggregate> _rootAggregateRepository;
        private readonly IReadModelRepository<ValueRecord> _readModelRepository;

        public ValueCommentController(IMediator mediator, 
            IRepository<ValuesRootAggregate> rootAggregateRepository, 
            IReadModelRepository<ValueRecord> readModelRepository)
        {
            _mediator = mediator;
            _rootAggregateRepository = rootAggregateRepository;
            _readModelRepository = readModelRepository;
        }

        // GET: api/ValueComment
        [HttpGet("{parentId}")]
        public IActionResult Get(Guid? parentId)
        {
            return Ok(new List<CommentViewModel>());
        }

        // GET: api/ValueComment/5
        [HttpGet("{parentId}/{id}", Name = "Get")]
        public IActionResult Get(Guid? parentId, Guid? id)
        {
            return Ok(new CommentViewModel());
        }

        // POST: api/ValueComment
        [HttpPost("{parentId}")]
        public async Task<IActionResult> Post(Guid? parentId, [FromBody] CommentViewModel comment)
        {

            var command = new AddCommentCommand
            {
                ParentId = parentId.GetValueOrDefault(),
                ParentVersion = comment.ParentVersion.Value.GetValueOrDefault(),
                CommentText = comment.CommentText.Value,
                UserName = comment.UserName.Value,
                TenantId = comment.TenantId.Value
            };

            var result = await _mediator.Send(command);

            return Created($"~/api/valuecomment/{result.Model.ParentId}/{result.Model.Id}", result);
        }

        // PUT: api/ValueComment/5
        [HttpPut("{parentId}/{id}")]
        public IActionResult Put(Guid? parentId, Guid? id, [FromBody] CommentViewModel comment)
        {
            return Ok(comment);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
