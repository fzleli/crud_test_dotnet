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
            var value = "8205401026800208";

            // Act
            var account = BankAccountNumber.Create(value);

            // Assert
            account.Value.Should().Be(value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("short123")]
        [InlineData("invalid-char@")]
        public void Create_ShouldThrowInvalidBankAccountNumberException_WhenValueIsInvalid(string invalidValue)
        {
            // Act
            Action act = () => BankAccountNumber.Create(invalidValue);

            // Assert
            act.Should().Throw<InvalidBankAccountNumberException>()
                .WithMessage($"The bank account number '{invalidValue}' is invalid.");
        }
    }
}
