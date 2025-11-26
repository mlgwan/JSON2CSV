namespace JSON2CSV.Shared.Tests
{
    public class Json2CsvConverterTests
    {
        #region Validation
        Json2CsvConverter Json2CsvConverter = new Json2CsvConverter();

        [Theory]
        [InlineData("")]
        [InlineData(""" Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. """)]
        [InlineData(""" [{"name":"Shawn";"age":12}] """)]
        [InlineData(""" [{"name":"Shawn","age":12},{"name"}] """)]
        [InlineData(""" [{"name":"Shawn","age":12},{"name":"Ethan":"age":30}] """)]
        public void IsValidJson_NonSensicalInputs_ReturnsFalse(string input)
        {
            var actual = Json2CsvConverter.IsValidJson(input);

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
            var actual = Json2CsvConverter.IsValidJson(input);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Nesting

        #endregion
    }
}