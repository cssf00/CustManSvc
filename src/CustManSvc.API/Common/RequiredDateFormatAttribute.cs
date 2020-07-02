using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace CustManSvc.API.Common
{
    public class RequiredDateFormatAttribute : ValidationAttribute
    {
        private readonly string _dateFormat;

        // Allow setting of date format
        public RequiredDateFormatAttribute(string dateFormat)
            : base()
        {
            _dateFormat = dateFormat;
        }

        protected override ValidationResult IsValid(object value, ValidationContext vContext)
        {
            if (value == null)
            {
                return new ValidationResult($"{vContext.MemberName} cannot be null");
            }

            DateTime dateParsed;
            if (DateTime.TryParseExact(value.ToString(), _dateFormat,
                    DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dateParsed))
            {
                return ValidationResult.Success;
            }
            
            return new ValidationResult($"{vContext.MemberName} does not conform to {_dateFormat} format");
        }
    }
}
