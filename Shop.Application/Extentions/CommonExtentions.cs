using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Extentions
{
    public static class CommonExtentions
    {
        public static string GetEnumName(this Enum DataEnum)
        {
            var enumDisplayName = DataEnum.GetType().GetMember(DataEnum.ToString()).FirstOrDefault();

            if (enumDisplayName != null)
            {
                return enumDisplayName.GetCustomAttribute<DisplayAttribute>()?.GetName();
            }

            return "";
        }
    }
}
