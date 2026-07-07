using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ERP.Application.Abstractions.Repositories
{
    public class QueryOptions<T> where T : class
    {
        public Expression<Func<T, bool>>? Filter { get; set; }

        public List<Expression<Func<T, object>>> Includes { get; set; } = new();

        public int? Skip { get; set; }

        public int? Take { get; set; }
    }
}
