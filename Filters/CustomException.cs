namespace UrlShortenerBackend.Filters
{
    public class CustomException : Exception
    {
        public string CustomMessage { get; }

        public CustomException(string customMessage, Exception? innerException = null)
            : base(innerException?.Message, innerException)
        {
            CustomMessage = customMessage;
        }
    }
}