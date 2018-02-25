using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public interface ISelectListItem
    {
        int Id { get; set; }

        string Text { get; set; }
    }
}