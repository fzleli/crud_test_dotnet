using FluentAssertions;
using Mc2.CrudTest.Domain.Exceptions;
using Mc2.CrudTest.Domain.ValueObjects;

namespace Mc2.CrudTest.Domain.UnitTests.Customers.ValueObjects
{
    public class PhoneNumberTests
    {
        [Fact]
        public void Create_ShouldReturnPhoneNumber_WhenValueIsValid()
        {
            // Arrange
            var value = "+989123456789";

            // Act
            var phoneNumber = PhoneNumber.Create(value);

            // Assert
            phoneNumber.Value.Should().Be(value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        public void Create_ShouldThrowInvalidPhoneNumberException_WhenValueIsInvalid(string invalidValue)
        {
            // Act
            Action act = () => PhoneNumber.Create(invalidValue);

            // Assert
            act.Should().Throw<InvalidPhoneNumberException>();
        }
    }
}
