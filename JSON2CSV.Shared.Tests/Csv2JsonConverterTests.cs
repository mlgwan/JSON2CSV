using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSON2CSV.Shared.Tests
{
    public class Csv2JsonConverterTests
    {
        Csv2JsonConverter Csv2JsonConverter = new();

        #region Validation

        [Fact]
        public void ValidationCheck_EmptyInput_ReturnsFalse()
        {
             bool actual = Csv2JsonConverter.ValidationCheck("").Item1;

            Assert.False(actual);
        }

        [Theory]
        [InlineData("username, password, username\n")]
        [InlineData("username; password; username\n")]
        public void ValidationCheck_DuplicateKeys_ReturnsFalse(string input)
        {
            bool actual = Csv2JsonConverter.ValidationCheck(input).Item1;

            Assert.False(actual);
        }

        [Theory]
        [InlineData("a,b,c\n1,2,3,4")]
        public void ValidationCheck_MismatchingAmountHeadersAndValues_ReturnsFalse(string input)
        {
            bool actual = Csv2JsonConverter.ValidationCheck(input).Item1;

            Assert.False(actual);
        }

        #endregion

        #region Conversion

        [Theory]
        [InlineData("a", """{"a":""}""")]
        [InlineData("a,b", """{"a":"","b":""}""")]
        [InlineData("a,b\n1", """{"a":1,"b":""}""")]
        [InlineData("s,d\n1\n, 1", """[{"s":1,"d":""},{"s":"","d":1}]""")]
        [InlineData("var values = (valueLines.Count > 0) ? valueLines[0].Split(separationCharacter).ToList() : new List<string>();\nresult += ConvertCsvLine(headers, values);", """{"var values = (valueLines.Count > 0) ? valueLines[0].Split(separationCharacter).ToList() : new List<string>()":"result += ConvertCsvLine(headers, values)","":""}""")]
        [InlineData("username, password\nbob, hunter2\nalice, password","""[{"username":"bob","password":"hunter2"},{"username":"alice","password":"password"}]""")]
        [MemberData(nameof(TestDataGenerator.CsvFileCase), parameters: new object[] { """[{"Username":"booker12","Identifier":9012,"First name":"Rachel","Last name":"Booker"},{"Username":"grey07","Identifier":2070,"First name":"Laura","Last name":"Grey"},{"Username":"johnson81","Identifier":4081,"First name":"Craig","Last name":"Johnson"},{"Username":"jenkins46","Identifier":9346,"First name":"Mary","Last name":"Jenkins"},{"Username":"smith79","Identifier":5079,"First name":"Jamie","Last name":"Smith"}]""" }, MemberType = typeof(TestDataGenerator))]
        
        public void ConvertCsv2Json_ValidInput_ReturnsValidOutput(string input, string expected)
        {
            var actual = Csv2JsonConverter.ConvertCsv2Json(input);
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
