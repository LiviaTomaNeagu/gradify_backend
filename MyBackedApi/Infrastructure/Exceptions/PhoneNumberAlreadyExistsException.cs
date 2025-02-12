namespace Infrastructure.Exceptions
{
    public class PhoneNumberAlreadyExistsException : Exception
    {
        public PhoneNumberAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
