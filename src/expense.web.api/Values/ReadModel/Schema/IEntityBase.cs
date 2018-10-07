using System;
using MongoDB.Bson;

namespace expense.web.api.Values.ReadModel.Schema
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
}