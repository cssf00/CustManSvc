using System;

namespace CustManSvc.Common
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
