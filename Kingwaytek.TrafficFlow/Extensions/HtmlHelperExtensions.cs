using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kingwaytek.TrafficFlow
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// 產生HTML body tag的class名稱，該名稱會由controller的名稱和action的名稱組成
        /// </summary>
        /// <param name="helper">支援在檢視中呈現 HTML 控制項</param>
        /// <returns>{controll name} - {action name}</returns>
        public static string PageClass(this HtmlHelper helper)
        {
            string currentController = ((string)helper.ViewContext.RouteData.Values["controller"]).ToLower();
            string currentAction = ((string)helper.ViewContext.RouteData.Values["action"]).ToLower();

            return $"{currentController}-{currentAction}";
        }
    }
}
