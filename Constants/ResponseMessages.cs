namespace UrlShortenerBackend.Constants
{
    public static class ResponseMessages
    {
        // ?? Success Messages
        public static string SaveSuccess = "Record saved successfully.";
        public static string UpdateSuccess = "Record updated successfully.";
        public static string DeleteSuccess = "Record deleted successfully.";
        public static string GetSuccess = "Record retrieved successfully.";
        public static string GetListSuccess = "Records retrieved successfully.";

        // ?? Error Messages
        public static string SaveError = "Failed to save the record. Please try again.";
        public static string UpdateError = "Failed to update the record. Please try again.";
        public static string DeleteError = "Failed to delete the record. Please try again.";
        public static string GetError = "Failed to retrieve the record. Please try again.";

        // ?? Not Found Messages
        public static string NotFound = "No record found.";
        public static string UpdateNotFound = "No record found to update.";
        public static string DeleteNotFound = "No record found to delete.";

        // ?? Validation Messages
        public static string InvalidRequest = "Invalid request. Please check the provided data.";
        public static string AlreadyExists = "Record already exists.";
        public static string Unauthorized = "You are not authorized to perform this action.";
        public static string SessionExpired = "Your session has ended. Please log in again to continue.";
    }
}