namespace JSON2CSV.Shared
{
    using System.Text.Json;
    public class Json2CsvConverter
    {
        public bool IsValidJson(string json)
        {
            try
            {
                var document = JsonDocument.Parse(json);
            }
            catch
            {
                return false;
            }

        return true;
        }
    }
}
