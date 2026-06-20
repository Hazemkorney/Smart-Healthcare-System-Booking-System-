using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Application.Exceptions
{
    public class DuplicateException : Exception
    {
        public DuplicateException(string message) : base(message)
        {

        }
    }
}
