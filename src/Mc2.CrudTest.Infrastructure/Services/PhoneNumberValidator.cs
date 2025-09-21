using Mc2.CrudTest.Application.Common.Interfaces;

namespace Mc2.CrudTest.Infrastructure.Services
{
    public class PhoneNumberValidator : IPhoneNumberValidator
    {
        public bool IsValid(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.StartsWith("+98");
        }
    }
}
