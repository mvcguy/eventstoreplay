using System.Collections.Generic;
using System.Linq;
using expense.web.api.Values.Aggregate.Model;
using expense.web.api.Values.Commands.Value;
using expense.web.api.Values.Dtos;
using expense.web.api.Values.ReadModel.Schema;
using Microsoft.AspNetCore.Mvc;

namespace expense.web.api.Controllers
{
    public class ValuesBaseController : ControllerBase
    {

        public ValueViewModel ToViewModel(ValueRecord record)
        {
            return new ValueViewModel()
            {
                Id = record.PublicId,
                TenantId = new DtoProp<int?>(record.TenantId),
                Name = new DtoProp<string>(record.Name),
                Code = new DtoProp<string>(record.Code),
                Value = new DtoProp<string>(record.Value),
                Version = new DtoProp<long?>(record.Version),
                Comments = record.Comments.Select(x=>ToViewModel(x, record.Version, record.TenantId))
            };
        }

        public CommentViewModel ToViewModel(ValueCommentRecord commentRecord, long? parentVersion, int? parentTenantId)
        {
            return new CommentViewModel
            {
                Id = commentRecord.PublicId,
                ParentId = commentRecord.ParentId,
                ParentVersion = new DtoProp<long?>(parentVersion),
                UserName = new DtoProp<string>(commentRecord.UserName),
                TenantId = new DtoProp<int?>(parentTenantId),
                CommentText = new DtoProp<string>(commentRecord.CommentText),
                Likes = new DtoProp<int?>(commentRecord.Likes),
                Dislikes = new DtoProp<int?>(commentRecord.Dislikes)
            };
        }

        public ValueViewModel ToViewModel(IValuesRootAggregateDataModel aggregate)
        {
            return new ValueViewModel()
            {
                Id = aggregate.Id,
                TenantId = new DtoProp<int?>(aggregate.TenantId),
                Name = new DtoProp<string>(aggregate.Name),
                Code = new DtoProp<string>(aggregate.Code),
                Value = new DtoProp<string>(aggregate.Value),
                Version = new DtoProp<long?>(aggregate.Version),
                Comments = aggregate.Comments.Select(ToViewModel)
            };
        }

        public CommentViewModel ToViewModel(IValueCommentAggregateChildDataModel comment)
        {
            return new CommentViewModel
            {
                Id = comment.Id,
                UserName = new DtoProp<string>(comment.UserName),
                ParentVersion = new DtoProp<long?>(comment.ParentVersion),
                ParentId = comment.ParentId,
                TenantId = new DtoProp<int?>(comment.TenantId),
                CommentText = new DtoProp<string>(comment.CommentText),
                Dislikes = new DtoProp<int?>(comment.Dislikes),
                Likes = new DtoProp<int?>(comment.Likes)
            };
        }


        protected IActionResult CreateBadRequestResult(CommandResponseBase result)
        {
            return BadRequest(new Dictionary<string, List<string>>() { { "Error", new List<string>() { result.Message } } });
        }

        protected IActionResult CreateInvalidRequestResult()
        {
            return BadRequest(new Dictionary<string, List<string>>()
                {{"Error", new List<string>() {"The request is invalid"}}});
        }

    }
}