using System;

namespace CustManSvc.API.Service.Database
{
    public class RecordNotFoundException: Exception
    {
        public RecordNotFoundException() {}

        public RecordNotFoundException(string message)
            : base(message)
        {}
    }
}