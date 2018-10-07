using expense.web.api.Values.Aggregate.Model;
using expense.web.api.Values.Commands.Value;

namespace expense.web.api.Values.Commands.Comments
{
    public class CommentCommandResponse : CommandResponseBase
    {
        public IValueCommentAggregateChildDataModel Model { get; set; }
    }
}