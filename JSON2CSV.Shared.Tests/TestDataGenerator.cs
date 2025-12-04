using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSON2CSV.Shared.Tests
{
    public static class TestDataGenerator
    {


        public static IEnumerable<object[]> JsonFileCase(string expected)
        {

            var path = Path.Combine(AppContext.BaseDirectory, "Files", "json2.txt");
            var json = File.ReadAllText(path);
            yield return new object[] { json, expected };
        }

        public static IEnumerable<object[]> CsvFileCase(string expected)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Files", "username.csv");
            var csv = File.ReadAllText(path);
            yield return new object[] { csv, expected };

        }
    }

}
