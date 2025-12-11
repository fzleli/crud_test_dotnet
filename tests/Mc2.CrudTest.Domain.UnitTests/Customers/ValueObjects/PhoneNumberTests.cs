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
            var expectedNumericValue = 989123456789UL;

            // Act
            var phone = PhoneNumber.Create(value); 

            // Assert
            phone.Value.Should().Be(expectedNumericValue);
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("+1-123-456-7890-invalid")]
        public void Create_ShouldThrowInvalidPhoneNumberException_WhenValueIsInvalid(string invalidValue)
        {
            // Act
            Action act = () => PhoneNumber.Create(invalidValue);

            // Assert
            act.Should().Throw<InvalidPhoneNumberException>()
                .WithMessage($"Invalid phone number: {invalidValue}");
        }
    }
}
