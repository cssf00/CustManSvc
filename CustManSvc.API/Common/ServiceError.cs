using System;

namespace CustManSvc.API.Common
{
    // User defined service error
    public class ServiceError
    {
        public string ErrorMessage { get; set; }

        public ServiceError(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
