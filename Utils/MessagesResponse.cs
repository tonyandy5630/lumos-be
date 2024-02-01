using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class MessagesResponse
    {
        public static class Success
        {
            public const string Completed = "Operation completed successfully.";
            public const string Created = "Created successfully.";
            public const string Updated = "Updated successfully.";
            public const string Deleted = "Deleted successfully.";

        }

        public static class Error
        {
            public const string InvalidInput = "Invalid input. Please check your request.";
            public const string NotFound = "Resource not found.";
            public const string OperationFailed = "Operation failed. Please try again.";
            public const string Unauthorized = "Unauthorized access.";
            public const string DuplicateResource = "Resource already exists.";
            public const string LoginFailed = "Login Failed!";
            public const string RegisterFailed = "Register Failed!";

        }

        public static class Validation
        {
            public const string FieldIsRequired = "The {0} field is required.";
            public const string InvalidEmailFormat = "Invalid email format.";
        }
    }
}
