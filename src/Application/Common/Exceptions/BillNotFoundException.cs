using System;

namespace Application.Common.Exceptions
{
    public class BillNotFoundException : Exception
    {
        public BillNotFoundException() : base("Bill Not found")
        {
            
        }
    }
}