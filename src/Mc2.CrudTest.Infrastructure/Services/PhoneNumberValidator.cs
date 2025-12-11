using Mc2.CrudTest.Application.Common.Interfaces;
using PhoneNumbers;

namespace Mc2.CrudTest.Infrastructure.Services
{
    public class PhoneNumberValidator : IPhoneNumberValidator
    {
        public bool IsValid(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            try
            {
                var phoneNumberUtil = PhoneNumberUtil.GetInstance();
                var parsedNumber = phoneNumberUtil.Parse(phoneNumber, null);
                return phoneNumberUtil.IsValidNumber(parsedNumber);
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}
