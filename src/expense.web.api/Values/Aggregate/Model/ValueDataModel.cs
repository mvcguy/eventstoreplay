﻿using System;
using System.Collections.Generic;

namespace expense.web.api.Values.Aggregate.Model
{
    public class ValueDataModel : IValueAggregateModel
    {

        // Notes:
        // The TenantId plus Code together must be unique!!!

        public int TenantId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public Guid Id { get; set; }

        public long Version { get; set; }

        public Guid CommitId { get; set; }
        public IList<ValueComment> Comments { get; set; }
    }
}