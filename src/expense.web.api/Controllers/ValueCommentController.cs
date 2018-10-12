using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.api.Values.Commands.Comments;
using expense.web.api.Values.Dtos;
using expense.web.api.Values.ReadModel;
using expense.web.api.Values.ReadModel.Schema;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace expense.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValueCommentController : ValuesBaseController
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

        // GET: api/ValueComment/{parentID: GUID}
        [HttpGet("{parentId}")]
        public async Task<IActionResult> Get(Guid? parentId)
        {
            // get all the comments for the parentId
            var aggregate = await _readModelRepository.GetByAggregateId(parentId.GetValueOrDefault());
            if (aggregate == null) return BadRequest();

            return Ok(aggregate.Comments.Select(x => ToViewModel(x, aggregate.Version, aggregate.TenantId)));
        }

        // GET: api/ValueComment/{parentId:GUID}/{commentId:GUID}
        [HttpGet("{parentId}/{id}", Name = "Get")]
        public async Task<IActionResult> Get(Guid? parentId, Guid? id)
        {
            // BUG: Bad idea to lead all the comments first and then search for the required one. find alternative!!!

            var aggregate = await _readModelRepository.GetByAggregateId(parentId.GetValueOrDefault());
            if (aggregate == null) return BadRequest();

            var comment = aggregate.Comments.FirstOrDefault(x => x.PublicId == id);
            if (comment == null) return BadRequest();
            return Ok(ToViewModel(comment, aggregate.Version, aggregate.TenantId));
        }

        // POST: api/ValueComment/{parentId:GUID}
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

            return result.Success
                ? Created($"~/api/valuecomment/{result.Model.ParentId}/{result.Model.Id}", ToViewModel(result.Model))
                : CreateBadRequestResult(result);
        }

        [HttpPut(template: "LikeCommentAction/{parentId}/{id}")]
        public async Task<IActionResult> CommentLiked(Guid? parentId, Guid? id, [FromBody] CommentViewModel comment)
        {

            var command = new UpdateCommentCommand
            {
                ParentId = parentId.GetValueOrDefault(),
                Id = id.GetValueOrDefault(),
                ParentVersion = (comment.ParentVersion?.Value).GetValueOrDefault(),
                CommentLikedChildCmd = new CommentLikedChildCmd()
            };

            var result = await _mediator.Send(command);

            return result.Success
                ? Ok(ToViewModel(result.Model))
                : CreateBadRequestResult(result);
        }

        [HttpPut("DislikeCommentAction/{parentId}/{id}")]
        public async Task<IActionResult> CommentDisliked(Guid? parentId, Guid? id, [FromBody] CommentViewModel comment)
        {

            var command = new UpdateCommentCommand
            {
                ParentId = parentId.GetValueOrDefault(),
                Id = id.GetValueOrDefault(),
                ParentVersion = (comment.ParentVersion?.Value).GetValueOrDefault(),
                CommentDislikedChildCmd = new CommentDislikedChildCmd()
            };

            var result = await _mediator.Send(command);

            return result.Success
                ? Ok(ToViewModel(result.Model))
                : CreateBadRequestResult(result);
        }

        // PUT: api/ValueComment/{parentID: GUID}/{commentID: GUID}
        [HttpPut("{parentId}/{id}", Order = 0)]
        public async Task<IActionResult> Put(Guid? parentId, Guid? id, [FromBody] CommentViewModel comment)
        {
            var command = new UpdateCommentCommand
            {
                ParentId = parentId.GetValueOrDefault(),
                Id = id.GetValueOrDefault(),
                ParentVersion = (comment.ParentVersion?.Value).GetValueOrDefault(),
                UpdateCommentTextCmd = comment.CommentText != null ? new UpdateCommentTextChildCmd(comment.CommentText.Value) : null
            };

            var result = await _mediator.Send(command);

            return result.Success
                ? Ok(ToViewModel(result.Model))
                : CreateBadRequestResult(result);
        }

        // DELETE: api/ValueComment/{parentID: GUID}/{commentID: GUID}
        [HttpDelete("{parentId}/{id}")]
        public void Delete(Guid? parentId, Guid? id)
        {
        }
    }
}
