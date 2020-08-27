using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace ImportacaoDeQuestionariosSME.Utils
{
    public static class Extensions
    {
        public static string ReplaceFirst(this string value, string search, string replaceValue)
        {
            var regex = new Regex(Regex.Escape(search));
            return regex.Replace(value, replaceValue, 1);
        }

        public static void ToCSV(this DataTable dtDataTable, string strFilePath)
        {
            var sw = new StreamWriter(strFilePath, false);
            //headers  
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(";");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(","))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(";");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }
}