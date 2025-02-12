using System;

namespace Infrastructure.Exceptions
{
    public class ResourceAlreadyExistsException : Exception
    {
        public ResourceAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
