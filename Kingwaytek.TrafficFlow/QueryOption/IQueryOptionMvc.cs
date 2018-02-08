using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public interface IQueryOptionMvc<out TModel> : IQueryOption<TModel>
    {
        string ActionName { get; set; }

        string ControllerName { get; set; }

        object RouteValues { get; set; }
    }
}
