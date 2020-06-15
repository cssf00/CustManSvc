using System;

namespace CustManSvc.Service.Database
{
    public class RecordNotFoundException: Exception
    {
        public RecordNotFoundException() {}

        public RecordNotFoundException(string message)
            : base(message)
        {}
    }
}