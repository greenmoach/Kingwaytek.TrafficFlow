using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class PagedQueryModel<TModel> : QueryOption<TModel>, IQueryOptionMvc<TModel>
    {
        public PagedQueryModel() : base()
        {
            ActionName = "List";
        }

        public string ActionName { get; set; }

        public string ControllerName { get; set; }

        public object RouteValues { get; set; }
    }
}
