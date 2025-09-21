using FluentAssertions;
using Mc2.CrudTest.Domain.Exceptions;
using Mc2.CrudTest.Domain.ValueObjects;

namespace Mc2.CrudTest.Domain.UnitTests.Customers.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Create_ShouldReturnEmail_WhenValueIsValid()
        {
            // Arrange
            var value = "test@example.com";

            // Act
            var email = Email.Create(value);

            // Assert
            email.Value.Should().Be(value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalid-email")]
        [InlineData("missing@domain")]
        public void Create_ShouldThrowInvalidEmailException_WhenValueIsInvalid(string invalidValue)
        {
            // Act
            Action act = () => Email.Create(invalidValue);

            // Assert
            act.Should().Throw<InvalidEmailException>();
        }
    }
}
