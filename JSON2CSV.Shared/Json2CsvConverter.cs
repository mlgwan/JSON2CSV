namespace JSON2CSV.Shared
{
    using JSON2CSV.Shared.Resources;
    using System.Reflection.PortableExecutable;
    using System.Text.Json;
    public class Json2CsvConverter
    {
        public string ConvertJsonToCsv(string json)
        {
            if (!IsValidJson(json))
            {
                throw new InvalidDataException(ErrorMessages.InvalidJson);
            }
            else if (IsValidJson(json) && IsNestedJson(json))
            {
                throw new InvalidDataException(ErrorMessages.NestedJson);
            }

            string csvResult = string.Empty;

            json = json.Trim();

            string headers = GetHeaderStringOfJson(json);
            string values = GetValuesStringOfJson(json);

            csvResult = $"""
                {headers}
                {values}
                """;

            return csvResult;
        }

        public string GetValuesStringOfJson(string json)
        {
            string result = string.Empty;


            return result;
        }
        public string GetHeaderStringOfJson(string json)
        {
            string result = string.Empty;

            var headers = new HashSet<string>();
            var root = JsonDocument.Parse(json).RootElement;

            IEnumerable<JsonElement> elements = root.ValueKind == JsonValueKind.Array
                ? root.EnumerateArray()
                : new[] { root };

            foreach(var element in elements)
            {
                headers.UnionWith(GetHeadersOfJsonObject(element));
            }

            result = string.Join(',', headers) + '\n';

            return result;
        }
        private HashSet<string> GetHeadersOfJsonObject(JsonElement root)
        {
            if (root.ValueKind == JsonValueKind.Object)
            {
                var headers = new HashSet<string>();
                foreach (var property in root.EnumerateObject())
                {
                    headers.Add(property.Name);
                }
                return headers;
            }
            else
            {
                throw new InvalidDataException("GetHeadersOfJsonObject must be called with JsonElement of ValueKind JsonValueKind.Object.");
            }

        }

        public bool IsValidJson(string json)
        {
            try
            {
                var document = JsonDocument.Parse(json);

                if(DoesNotContainDuplicateKeys(json))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
          }

        private bool DoesNotContainDuplicateKeys(string json)
        {
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
            var stack = new Stack<HashSet<string>>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        stack.Push(new HashSet<string>(StringComparer.Ordinal));
                        break;

                    case JsonTokenType.EndObject:
                        stack.Pop();
                        break;

                    case JsonTokenType.PropertyName:
                        if(stack.Count > 0)
                        {
                            var current = stack.Peek();
                            var propName = reader.GetString();
                            if (!current.Add(propName))
                            {
                                // Duplicate key found
                                return false;
                            }
                        }
                    break;
                }
            }

            return true;
        }

        public bool IsNestedJson(string json)
        {
            if (!IsValidJson(json))
            {
                throw new InvalidDataException(ErrorMessages.InvalidJson);
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
