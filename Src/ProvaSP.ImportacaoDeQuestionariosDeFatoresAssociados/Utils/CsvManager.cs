using System;
using System.Data;
using System.Text;

namespace ImportacaoDeQuestionariosSME.Utils
{
    public static class CsvManager
    {
        public static DataTable GetCsvFile(string csv_file_path)
        {
            var csvData = new DataTable();
            if (csv_file_path.EndsWith(".csv"))
            {
                using (var csvReader = new Microsoft.VisualBasic.FileIO.TextFieldParser(csv_file_path, Encoding.GetEncoding(1252)))
                {
                    csvReader.SetDelimiters(new string[] { ";" });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    //read column
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        var datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }

            return csvData;
        }
    }
}