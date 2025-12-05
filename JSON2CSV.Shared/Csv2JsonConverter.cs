using JSON2CSV.Shared.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSON2CSV.Shared
{
    public class Csv2JsonConverter
    {
        public string[] GetCsvLines(string csv)
        {
            return csv.Trim().Split('\n');
        }
        public string ConvertCsv2Json(string csv, char? separationCharacter = null)
        {
            string result = string.Empty;
            StringBuilder sb = new StringBuilder();

            var lines = GetCsvLines(csv);
            if (separationCharacter == null)
            {
                separationCharacter = GuessSeparationCharacter(lines[0]);
            }
            var validationCheckResult = ValidationCheck(lines, separationCharacter.Value);

            if (!validationCheckResult.Item1)
            {
                throw new InvalidDataException($"{validationCheckResult.Item2}");
            }

             
            var headers = lines[0].Split(separationCharacter.Value).ToList();
            var valueLines = lines.Skip(1).ToList();

            if (valueLines.Count > 1)
            {
                sb.Append("[");

                for (int i = 0; i < valueLines.Count; i++)
                {
                    var values = (valueLines.Count > 0) ? valueLines[i].Split(separationCharacter.Value).ToList() : new List<string>();
                    sb.Append(ConvertCsvLine(headers, values));
                }
                sb.Length--;
                sb.Append("]");
            }
            else
            {
                var values = (valueLines.Count > 0) ? valueLines[0].Split(separationCharacter.Value).ToList() : new List<string>();                
                sb.Append(ConvertCsvLine(headers, values));
                sb.Length--;
            }

            return sb.ToString();
        }

        private string ConvertCsvLine(List<string> headers, List<string> values)
        {
            var paddedValues = values
                .Concat(Enumerable.Repeat(string.Empty, headers.Count - values.Count))
                .Take(headers.Count)
                .ToList();

            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            for (int j = 0; j < headers.Count; j++)
            {
                var newLineHeader = headers[j].Trim().Replace("\"", "\\\"");
                var newLinePaddedValue = paddedValues[j].Trim().Replace("\"", "\\\"");

                if (int.TryParse(newLinePaddedValue, out int _) || (double.TryParse(newLinePaddedValue, out double _))){
                    sb.Append($"\"{newLineHeader}\":{newLinePaddedValue},");
                }
                else
                {
                    sb.Append($"\"{newLineHeader}\":\"{newLinePaddedValue}\",");
                }                    
            }
            sb.Length--;
            sb.Append("},");
            return sb.ToString();
        }

        public (bool,string) ValidationCheck(string[] csvLines, char separationCharacter)
        {
            // Check for empty input
            if (csvLines.Length == 0 || csvLines[0] == string.Empty)
            {
                return (false, ErrorMessages.InputIsEmpty);
            }

            var csvHeaderLine = csvLines[0];
            var headers = csvHeaderLine.Split(separationCharacter);

            // Check for duplicate headers
            var duplicatesCheckResult = DuplicatesCheck(headers);
            if (duplicatesCheckResult.Item1)
            {
                return (false, string.Format(ErrorMessages.InputContainsDuplicateKey, duplicatesCheckResult.Item2));
            }

            // Check for lines with more entries than headers
            string faultyLines = string.Empty;
            for (int i = 1; i < csvLines.Length; i++)
            {
                var entries = csvLines[i].Split(separationCharacter);
                if (entries.Length > headers.Length)
                {
                    faultyLines += $"{i},";
                }
            }
            if (faultyLines.Length > 0)
            {
                return (false, string.Format(ErrorMessages.CsvHasMoreFieldsThanHeaders, faultyLines.Substring(0, faultyLines.Length-1)));
            }

            // Input passed validation check
            return (true, string.Empty);
        }

        private (bool, string) DuplicatesCheck(string[] headers)
        {
            var duplicates = headers.GroupBy(x => x.Trim()).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

            if (duplicates.Count > 0) {
                var duplicateString = string.Empty;
                foreach (var duplicate in duplicates)
                {
                    duplicateString +=  $"{duplicate},";
                }
                duplicateString = duplicateString.Substring(0, duplicateString.Length - 1);
                return (true, duplicateString);
            }
            return (false, string.Empty);
        }

        private char GuessSeparationCharacter(string csv)
        {
            return csv.Contains(',') ? ',' : csv.Contains(';') ? ';' : csv.Contains('\t') ? '\t' : '|';
        }
    }
}
