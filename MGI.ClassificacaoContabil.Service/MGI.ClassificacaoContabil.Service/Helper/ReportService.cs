using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace Service.Helper
{
    public static class ReportService
    {
        public static byte[] GerarExcel<T>(IEnumerable<T> data)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, new UTF8Encoding(false));
            var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };
            using var csv = new CsvWriter(writer, config);

            if (data != null && data.Any())
            {
                csv.WriteRecords(data);
            }
            else
            {
                data = data.Append(Activator.CreateInstance<T>());
                csv.WriteRecords(data);
            }

            writer.Flush();

            memoryStream.Position = 0;

            return memoryStream.ToArray();
        }
    }
}
