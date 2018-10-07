using System;
using MongoDB.Bson;

namespace expense.web.api.Values.ReadModel.Schema
{
    public class EntityBase : IEntityBase
    {
        public ObjectId Id { get; set; }

        public Guid? PublicId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? LastModifiedBy { get; set; }
    }
}