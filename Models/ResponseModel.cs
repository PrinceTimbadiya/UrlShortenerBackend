using System.Net;

namespace UrlShortenerBackend.Models
{
    public class ResponseModel
    {
        public bool Status { get; set; }
        public HttpStatusCode HttpStatus { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}