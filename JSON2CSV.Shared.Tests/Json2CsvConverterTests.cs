using System.Text.Json;

namespace JSON2CSV.Shared.Tests
{
    public class Json2CsvConverterTests
    {
        Json2CsvConverter Json2CsvConverter = new Json2CsvConverter();

        #region Validation

        [Theory]
        [InlineData("")]
        [InlineData(""" Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. """)]
        [InlineData(""" [{"name":"Shawn";"age":12}] """)]
        [InlineData(""" [{"name":"Shawn","age":12},{"name"}] """)]
        [InlineData(""" [{"name":"Shawn","age":12},{"name":"Ethan":"age":30}] """)]
        public void IsValidJson_NonSensicalInputs_ReturnsFalse(string input)
        {
            bool actual = Json2CsvConverter.IsValidJson(input);

            Assert.False(actual);
        }
        
        [Theory]
        [InlineData(""" {"name":"Shawn","age":12] """, false)]
        [InlineData(""" ["name":"Shawn","age":12} """, false)]
        [InlineData(""" ["name":"Shawn","age":12] """, false)]
        [InlineData(""" [{"name":"Shawn","age":12] """, false)]
        [InlineData(""" ["name":"Shawn","age":12}] """, false)]
        [InlineData(""" {"name":"Shawn","age":12}} """, false)]
        [InlineData(""" [{"name":"Shawn","age":12}}] """, false)]
        [InlineData(""" [{"name":"Shawn","age":12}]] """, false)]
        [InlineData(""" [{"name":"Shawn","age":12}] """, true)]
        [InlineData(""" {"name":"Shawn","age":12} """, true)]
        public void IsValidJson_InvalidOpeningAndClosingBrackets_ReturnsExpectedValue(string input, bool expected)
        {
            bool actual = Json2CsvConverter.IsValidJson(input);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Nesting
        [Theory]
        [InlineData(""" {"name":"Shawn","age":12} """)]
        [InlineData(""" [{"name":"Shawn","age":12},{"name":"Ethan","age":30}] """)]
        public void IsNestedJson_InputIsNotNested_ReturnsFalse(string json)
        {
            bool actual = Json2CsvConverter.IsNestedJson(json);

            Assert.False(actual);
        }

        [Theory]

        [InlineData(""" {"name":"Shawn","age":{"value":12}} """)]
        [InlineData(""" [{"name":"Shawn","age":12},{"name":"Ethan","age":30}, {"a":[]}] """)]

        public void IsNestedJson_InputIsNested_ReturnsTrue(string json)
        {
            bool actual = Json2CsvConverter.IsNestedJson(json);

            Assert.True(actual);
        }


        #endregion
    }
}