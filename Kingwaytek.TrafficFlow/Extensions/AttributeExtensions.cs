using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public static class AttributeExtensions
    {
        public static string GetDisplay(this DisplayAttribute attr, string propName = "Name")
        {
            var result = string.Empty;
            var tp = attr.GetType();
            var prop = tp.GetProperty(propName);
            if (prop != null)
            {
                result = (string)prop.GetValue(attr);
                if (attr.ResourceType != null)
                {
                    result = result.GetLocalString(attr.ResourceType);
                }
            }

            return result;
        }
    }
}