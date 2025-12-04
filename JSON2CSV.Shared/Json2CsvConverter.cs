using JSON2CSV.Shared.Resources;
using System.Text.Json;

namespace JSON2CSV.Shared
{
    public class Json2CsvConverter
    {
        public string ConvertJsonToCsv(string json)
        {
            var validationCheckResult = ValidationCheck(json);

            if (!validationCheckResult.Item1)
            {
                throw new InvalidDataException(validationCheckResult.Item2);
            }

            string csvResult = string.Empty;

            json = json.Trim();

            string headers = GetHeaderStringOfJson(json);
            string values = GetValuesStringOfJson(json);

            csvResult = $"{headers}{values}";

            return csvResult;
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

        public string GetValuesStringOfJson(string json)
        {
            string result = string.Empty;

            var root = JsonDocument.Parse(json).RootElement;

            IEnumerable<JsonElement> elements = (root.ValueKind == JsonValueKind.Array)
                ? root.EnumerateArray()
                : new[] { root };

            foreach (var element in elements)
            {
                var values = GetValuesOfJsonObject(element);
                result += string.Join(',', values) + '\n';
            }

            return result;
        }
        private List<JsonElement> GetValuesOfJsonObject(JsonElement root)
        {
            if (root.ValueKind == JsonValueKind.Object)
            {
                var values = new List<JsonElement>();
                foreach (var property in root.EnumerateObject())
                {
                    values.Add(property.Value);
                }
                return values;
            }
            else
            {
                throw new InvalidDataException("GetValuesOfJsonObject must be called with JsonElement of ValueKind JsonValueKind.Object.");
            }
        }

        public (bool,string) ValidationCheck(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    return (false, ErrorMessages.InputIsEmpty);
                }
                
                if (IsNestedJson(json))
                {
                    return (false, ErrorMessages.NestedJson);
                }

                var duplicationCheckResult = DuplicationCheck(json);
                if (!duplicationCheckResult.Item1)
                {
                    return (false, string.Format(ErrorMessages.InputContainsDuplicateKey, duplicationCheckResult.Item2));
                }
    
                //JSON is valid
                return (true, string.Empty);
            }
            catch
            {
                return (false,ErrorMessages.InvalidJson);
            }
          }

        private (bool,string) DuplicationCheck(string json)
        {
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
            var stack = new Stack<HashSet<string>>();
            var duplicates = string.Empty;

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
                                duplicates += propName + ", ";
                            }
                        }
                    break;
                }
            }
            if (!String.IsNullOrEmpty(duplicates))
            {
                return (false, duplicates.Substring(0, duplicates.Length - 2));
            }

            return (true,string.Empty);
        }

        public bool IsNestedJson(string json)
        {
            var root = JsonDocument.Parse(json).RootElement;

            IEnumerable<JsonElement> elements = (root.ValueKind == JsonValueKind.Array)
            ? root.EnumerateArray()
            : new[] { root };

            foreach (var element in elements)
            {
                if (IsNestedJsonObject(element))
                {
                    return true;
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
