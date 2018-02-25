using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Kingwaytek.TrafficFlow
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 判斷集合是否為空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static List<SelectListItem> ToSelectListItemList<T>(this IEnumerable<T> objs,
            Expression<Func<T, string>> exprText,
            Expression<Func<T, string>> exprValue,
            Expression<Func<T, bool>> defaValue = null)
        {
            var items = new List<SelectListItem>();
            if (objs.IsNullOrEmpty() == false)
            {
                items.AddRange(objs.Select(x => new SelectListItem
                {
                    Text = exprText.Compile()(x),
                    Value = exprValue.Compile()(x),
                    Selected = (defaValue != null && defaValue.Compile()(x) == true)
                }));
            }

            return items;
        }

        public static List<SelectListItem> ToSelectListItemList<T>(this IEnumerable<T> listObjects)
            where T : ISelectListItem
        {
            var items = listObjects.ToSelectListItemList(x => x.Text, x => x.Id.ToString());

            return items;
        }
    }
}