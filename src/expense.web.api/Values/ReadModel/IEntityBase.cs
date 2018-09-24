using MongoDB.Bson;

namespace expense.web.api.Values.ReadModel
{
    public interface IEntityBase
    {
        ObjectId Id { get; set; }
    }
}