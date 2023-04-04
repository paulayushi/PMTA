using System.ComponentModel.DataAnnotations;

namespace PMTA.Domain.CustomValidator
{
    public class DateRangeValidator: ValidationAttribute
    {
        public string FromDate { get; set; }
        public DateRangeValidator(string fromDate)
        {
            FromDate = fromDate;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value != null)
            {
                var fromDateRef = validationContext.ObjectType.GetProperty(FromDate);
                if(fromDateRef != null)
                {
                    var fromDate = Convert.ToDateTime(fromDateRef.GetValue(validationContext.ObjectInstance));
                    var toDate = Convert.ToDateTime(value);

                    if(fromDate > toDate)
                    {
                        return new ValidationResult(ErrorMessage, new[]
                        {
                            validationContext.MemberName
                        });
                    }
                    return ValidationResult.Success;
                }
                return null;
            }
            return null;
        }
    }
}
