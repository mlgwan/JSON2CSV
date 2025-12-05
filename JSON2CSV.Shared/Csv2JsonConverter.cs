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
        public string ConvertCsv2Json(string csv)
        {
            string result = string.Empty;                      
            var validationCheckResult = ValidationCheck(csv);

            if (!validationCheckResult.Item1)
            {
                throw new InvalidDataException($"{validationCheckResult.Item2}");
            }
            var lines = csv.Trim().Split('\n');
            char separationCharacter = DetermineSeparationCharacter(lines[0]);
            var headers = lines[0].Split(separationCharacter).ToList();
            var valueLines = lines.Skip(1).ToList();

            if (valueLines.Count > 1)
            {
                result = "[";

                for (int i = 0; i < valueLines.Count; i++)
                {
                    var values = (valueLines.Count > 0) ? valueLines[i].Split(separationCharacter).ToList() : new List<string>();
                    result += ConvertCsvLine(headers, values);
                }
                result = result.Substring(0, result.Length - 1);
                result += "]";
            }
            else
            {
                var values = (valueLines.Count > 0) ? valueLines[0].Split(separationCharacter).ToList() : new List<string>();                
                result += ConvertCsvLine(headers, values);
                result = result.Substring(0, result.Length - 1);
            }

            return result;
        }

        private string ConvertCsvLine(List<string> headers, List<string> values)
        {
            var paddedValues = values.Concat(Enumerable.Repeat(string.Empty, headers.Count - values.Count)).Take(headers.Count).ToList();
            string newJsonLine = "{";
            for (int j = 0; j < headers.Count; j++)
            {
                var trimmedHeader = headers[j].Trim();
                var newLineHeader = string.Empty;
                for (int i = 0; i < trimmedHeader.Length; i++)
                {
                    if (trimmedHeader[i] == '"')
                    {
                        newLineHeader += $"\\{trimmedHeader[i]}";
                    }
                    else
                    {
                        newLineHeader += $"{trimmedHeader[i]}";
                    }
                }

                var trimmedPaddedValues = paddedValues[j].Trim();
                var newLinePaddedValue = string.Empty;
                for (int i = 0; i < trimmedPaddedValues.Length; i++)
                {
                    if (trimmedPaddedValues[i] == '"')
                    {
                        newLinePaddedValue += $"\\{trimmedPaddedValues[i]}";
                    }
                    else
                    {
                        newLinePaddedValue += $"{trimmedPaddedValues[i]}";
                    }
                }

                if (int.TryParse(newLinePaddedValue, out int _) || (double.TryParse(newLinePaddedValue, out double _))){
                    newJsonLine += $"\"{newLineHeader}\":{newLinePaddedValue},";
                }
                else
                {
                    newJsonLine += $"\"{newLineHeader}\":\"{newLinePaddedValue}\",";
                }                    
            }
            newJsonLine = newJsonLine.Substring(0, newJsonLine.Length - 1);
            newJsonLine += "}";
            return newJsonLine + ',';
        }

        public (bool,string) ValidationCheck(string csv)
        {
            if (string.IsNullOrEmpty(csv))
            {
                return (false, ErrorMessages.InputIsEmpty);
            }

            var csvLines = csv.Split('\n');
            var csvHeaderLine = csvLines[0];

            char separationCharacter = DetermineSeparationCharacter(csvHeaderLine);

            var duplicatesCheckResult = DuplicatesCheck(csvHeaderLine.Split(separationCharacter));
            if (duplicatesCheckResult.Item1)
            {
                return (false, string.Format(ErrorMessages.InputContainsDuplicateKey, duplicatesCheckResult.Item2));
            }

            var headers = csvHeaderLine.Split(separationCharacter);
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

        private char DetermineSeparationCharacter(string csv)
        {
            return csv.Contains(',') ? ',' : csv.Contains(';') ? ';' : csv.Contains('\t') ? '\t' : '|';
        }
    }
}
