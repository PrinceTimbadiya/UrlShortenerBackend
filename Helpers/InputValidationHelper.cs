using System.Text.RegularExpressions;

namespace UrlShortenerBackend.Helpers
{
    public class InputValidationHelper
    {
        public static void ValidateEmail(string? email)
        {
            if (!string.IsNullOrWhiteSpace(email) &&
                !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.(com|in)$"))
            {
                throw new Exception("Invalid Email");
            }
        }
    }
}