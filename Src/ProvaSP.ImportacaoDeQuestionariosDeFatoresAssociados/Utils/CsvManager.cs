using System.Data;
using System.Text;

namespace ImportacaoDeQuestionariosSME.Utils
{
    public static class CsvManager
    {
        public static DataTable GetCsvFile(string csv_file_path)
        {
            var csvData = new DataTable();

            if (!csv_file_path.EndsWith(".csv")) 
                return csvData;

            using (var csvReader = new Microsoft.VisualBasic.FileIO.TextFieldParser(csv_file_path, Encoding.GetEncoding(1252)))
            {
                csvReader.SetDelimiters(";");
                csvReader.HasFieldsEnclosedInQuotes = true;

                //read column
                var colFields = csvReader.ReadFields();
                foreach (var column in colFields)
                {
                    var datecolumn = new DataColumn(column);
                    datecolumn.AllowDBNull = true;
                    csvData.Columns.Add(datecolumn);
                }

                while (!csvReader.EndOfData)
                {
                    var fieldData = csvReader.ReadFields();

                    for (var i = 0; i < fieldData.Length; i++)
                    {
                        if (fieldData[i] == "")
                            fieldData[i] = null;
                    }

                    csvData.Rows.Add(fieldData);
                }
            }

            return csvData;
        }
    }
}