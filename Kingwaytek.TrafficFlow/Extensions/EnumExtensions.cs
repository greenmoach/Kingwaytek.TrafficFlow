using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    /// <summary>
    /// EnumExtensions
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());

            if (memInfo.Length > 0)
            {
                var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
                return (T)attributes.FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// 經由 DisplayAttribute 取得顯示名稱
        /// </summary>
        /// <param name="enumVal"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum enumVal, string prop = "Name")
        {
            var type = enumVal.GetType();

            if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
            {
                var flags = type.GetEnumValues();

                var sb = new StringBuilder();
                var sep = string.Empty;

                foreach (var flag in flags.Cast<Enum>())
                {
                    if (enumVal.HasFlag(flag) &&
                        (0 < (int)(object)enumVal && 0 == (int)(object)flag) == false)
                    {
                        var displayAttr = flag.GetAttributeOfType<DisplayAttribute>();

                        sb.Append(sep);
                        sb.Append(displayAttr == null ? flag.ToString() : displayAttr.GetDisplay(prop));

                        sep = ", ";
                    }
                }

                return sb.ToString();
            }
            else
            {
                var displayAttr = enumVal.GetAttributeOfType<DisplayAttribute>();

                return displayAttr == null ? enumVal.ToString() : displayAttr.GetDisplay(prop);
            }
        }
    }
}