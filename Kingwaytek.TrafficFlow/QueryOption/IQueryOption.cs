using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;

namespace Kingwaytek.TrafficFlow
{
    public interface IQueryOption<out TModel>
    {
        string Query { get; set; }

        bool HasQuery { get; set; }

        string SortColumn { get; set; }

        string LastActive { get; set; }

        SubmitType SubmitType { get; set; }

        Direction SortDirection { get; set; }

        IPagedList<TModel> Results { get; }

        int Page { get; set; }

        int PageSize { get; set; }

        int DefaultPageSize { get; set; }
    }
}
