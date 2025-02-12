namespace Infrastructure.Exceptions
{
    public class InvalidPhoneNumberException : Exception
    {
        public InvalidPhoneNumberException(string message) : base(message)
        {
        }
    }
}
