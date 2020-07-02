using System;
using System.Text.Json.Serialization;

namespace CustManSvc.API.Common
{
    // User defined service error
    public class ServiceError
    {
        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonPropertyName("detailedMessage")]
        public string DetailedMessage { get; set; }

        public ServiceError(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
