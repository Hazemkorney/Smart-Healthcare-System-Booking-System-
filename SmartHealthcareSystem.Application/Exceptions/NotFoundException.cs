using System;
using System.Collections.Generic;
using System.Text;
namespace SmartHealthcareSystem.Application.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string message)
      : base(message)
        {
        }


        public NotFoundException(string resource, int id) : base($"{resource} with id {id}is not found!")
        {

        }
    }
}
