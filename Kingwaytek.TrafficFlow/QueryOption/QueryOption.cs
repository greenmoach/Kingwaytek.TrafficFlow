using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

namespace Kingwaytek.TrafficFlow
{
    public abstract class QueryOption<TModel> : IQueryOption<TModel>
    {
        private int _pageSize;

        public QueryOption() : base()
        {
            this.Page = 1;
            this.DefaultPageSize = 10;
            this.Query = string.Empty;
        }

        public virtual string Query { get; set; }

        public virtual bool HasQuery { get; set; } = true;

        public virtual string SortColumn { get; set; }

        public virtual string LastActive { get; set; }

        public virtual SubmitType SubmitType { get; set; }

        public virtual Direction SortDirection { get; set; }

        public virtual IPagedList<TModel> Results { get; set; }

        public virtual int Page { get; set; }

        public int PageSize
        {
            get
            {
                if (this._pageSize == 0)
                {
                    return this.DefaultPageSize;
                }

                return this._pageSize;
            }

            set
            {
                this._pageSize = value;
            }
        }

        public int DefaultPageSize { get; set; }

        public virtual IPagedList<TModel> Apply(
            IQueryable<TModel> dataSource,
            Func<IOrderedQueryable<TModel>, IOrderedQueryable<TModel>> extendedSort = null)
        {
            var results = this.ApplySorts(dataSource);

            if (extendedSort != null)
            {
                results = extendedSort(results);
            }

            if (this.SubmitType == SubmitType.Export)
            {
                int count = results.Count();
                return this.Results = new StaticPagedList<TModel>(results, 1, count, count);
            }

            return this.Results = results.ToPagedList(this.Page, this.PageSize == 0 ? this.DefaultPageSize : this.PageSize);
        }

        protected virtual IOrderedQueryable<TModel> ApplySorts(IEnumerable<TModel> entities)
        {
            if (this.SortColumn.IsNullOrEmpty())
            {
                this.SetDefaultSort();
            }

            var param = Expression.Parameter(typeof(TModel));

            Expression parent = param;
            foreach (var column in this.SortColumn.Split('.'))
            {
                parent = Expression.Property(parent, column);
            }

            dynamic keySelector = Expression.Lambda(parent, param);

            if (this.SortDirection == Direction.Ascending)
            {
                return Queryable.OrderBy(entities.AsQueryable(), keySelector);
            }

            return Queryable.OrderByDescending(entities.AsQueryable(), keySelector);
        }

        protected virtual void SetDefaultSort()
        {
            var type = typeof(TModel);
            var properties = type.GetProperties()
                                 .Where(x => x.Name.EndsWith("Enum", StringComparison.OrdinalIgnoreCase) == false &&
                                             x.CustomAttributes.Any(attr => attr.AttributeType == typeof(NotMappedAttribute)) == false);

            if (!properties.IsNullOrEmpty())
            {
                this.SortColumn = properties.FirstOrDefault().Name;
                this.SortDirection = Direction.Descending;
            }
        }
    }
}