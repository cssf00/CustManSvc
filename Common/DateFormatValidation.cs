using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace CustManSvc.Common
{
    public class DateFormatValidationAttribute : ValidationAttribute
    {
        private readonly string _dateFormat;

        // Allow setting of date format
        public DateFormatValidationAttribute(string dateFormat)
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
            if (DateTime.TryParseExact(value.ToString(), Constants.DateFormatRFC3339,
                    DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dateParsed))
            {
                return ValidationResult.Success;
            }
            
            return new ValidationResult($"{vContext.MemberName} does not conform to {_dateFormat} format");
        }
    }
}
