using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public interface IQueryOption<out TModel, TFilter> : IQueryOption<TModel> where TFilter : new()
    {
        TFilter Filter { get; set; }
    }
}
