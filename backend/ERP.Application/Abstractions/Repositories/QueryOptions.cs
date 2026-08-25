
namespace ERP.Application.Abstractions.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class QueryOptions<T> where T : class
    {
        public List<Expression<Func<T, bool>>> Filters { get; set; } = new();

        public List<Expression<Func<T, object?>>> Includes { get; set; } = new();

        public int? Skip { get; set; }

        public int? Take { get; set; }

        public bool IncludeDeleted { get; set; } = false;

        public bool AsNoTracking { get; set; } = false;

        public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }
    }
}
