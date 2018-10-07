namespace expense.web.api.Values.Commands.Comments
{
    public class UpdateCommentTextChildCmd
    {
        public string CommentText { get; set; }

        public UpdateCommentTextChildCmd()
        {

        }

        public UpdateCommentTextChildCmd(string commentText)
        {
            this.CommentText = commentText;
        }
    }
}