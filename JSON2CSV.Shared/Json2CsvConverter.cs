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

        public bool IsNestedJson(string json)
        {
            if (!IsValidJson(json))
            {
                throw new InvalidDataException("Input is not valid JSON.");
            }

            var root = JsonDocument.Parse(json).RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                return IsNestedJsonObject(root);
            }

            else if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in root.EnumerateArray())
                {
                    if (IsNestedJsonObject(item))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsNestedJsonObject(JsonElement obj)
        {
            foreach (var property in obj.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Object ||
                    property.Value.ValueKind == JsonValueKind.Array)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
