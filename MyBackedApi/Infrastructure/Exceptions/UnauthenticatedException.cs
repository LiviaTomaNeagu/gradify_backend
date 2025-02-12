using System;

namespace Infrastructure.Exceptions
{
    public class UnauthenticatedException : Exception
    {
        public UnauthenticatedException(string message) : base(message) { }
    }
}
