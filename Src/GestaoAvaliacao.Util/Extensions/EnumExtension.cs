using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Util.Extensions
{
    public class EnumExtension
    {
        public static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (descriptionAttributes != null && descriptionAttributes.Length > 0)
            {
                return descriptionAttributes[0].Description;
            }

            return value.ToString();
        }
    }
}
