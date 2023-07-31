namespace Domain.Exceptions
{
    public class CustomHttpException : Exception
    {
        public int ErrorCode { get; set; }

        public CustomHttpException(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
