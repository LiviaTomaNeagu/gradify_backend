using System;

namespace Infrastructure.Exceptions
{
    public class ResourceRequiredException : Exception
    {
        public ResourceRequiredException(string message) : base(message)
        {
        }
    }
}
