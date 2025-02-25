namespace Infrastructure.Exceptions
{
    public class OperationNotAllowedException : Exception
    {
        public OperationNotAllowedException(string message) : base(message) { }
    }
}
