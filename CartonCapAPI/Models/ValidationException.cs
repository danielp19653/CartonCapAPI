using System;

namespace CartonCapAPI.Models
{
    public class ValidationException : Exception
    {
        public ValidationException(string validationMessage)
        {
            ValidationMessage = validationMessage;
        }

        public string ValidationMessage { get; set; }
    }
}
