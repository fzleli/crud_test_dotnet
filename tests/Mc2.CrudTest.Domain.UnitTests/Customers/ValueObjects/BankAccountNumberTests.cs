using FluentAssertions;
using Mc2.CrudTest.Domain.Exceptions;
using Mc2.CrudTest.Domain.ValueObjects;

namespace Mc2.CrudTest.Domain.UnitTests.Customers.ValueObjects
{
    public class BankAccountNumberTests
    {
        [Fact]
        public void Create_ShouldReturnBankAccountNumber_WhenValueIsValid()
        {
            // Arrange
            var value = "1234567890123456";

            // Act
            var accountNumber = BankAccountNumber.Create(value);

            // Assert
            accountNumber.Value.Should().Be(value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("12345")]
        [InlineData("abcde12345")]
        public void Create_ShouldThrowInvalidBankAccountNumberException_WhenValueIsInvalid(string invalidValue)
        {
            // Act
            Action act = () => BankAccountNumber.Create(invalidValue);

            // Assert
            act.Should().Throw<InvalidBankAccountNumberException>();
        }
    }
}
