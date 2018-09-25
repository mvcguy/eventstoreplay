using System;
using MongoDB.Bson;

namespace expense.web.api.Values.ReadModel
{
    public interface IEntityBase
    {
        ObjectId Id { get; set; }
        
        // some auditing fields
        DateTime? CreatedOn { get; set; }

        DateTime? LastModifiedOn { get; set; }

        Guid? CreatedBy { get; set; }

        Guid? LastModifiedBy { get; set; }

        Guid? PublicId { get; set; }
    }

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