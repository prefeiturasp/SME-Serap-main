using ProvaSP.Model.Utils.Attributes;
using System;
using System.ComponentModel;

namespace ProvaSP.Model.Utils
{
    public static class Extensions
    {
        public static string GetDescription(this Enum value)
        {
            System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string GetPath(this Enum value)
        {
            System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());

            PathAttribute[] attributes = (PathAttribute[])fi.GetCustomAttributes(typeof(PathAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Caminho;
            else
                return value.ToString();
        }
    }
}