using System.Net;

namespace HelpDesk.Common.CustomExceptions
{
    public abstract class CustomApiException : Exception
    {
        public int StatusCode { get; }

        public IReadOnlyList<string> Messages { get; }

        public IReadOnlyDictionary<string, object>? Metadata { get; }

        // Constructor for a single message
        protected CustomApiException(int statusCode, string message, IDictionary<string, object>? metadata = null)
            : base(message)
        {
            StatusCode = statusCode;
            Messages = [message];
            Metadata = metadata is not null
                ? new Dictionary<string, object>(metadata)
                : null;
        }

        // Constructor for a multiple messages
        protected CustomApiException(int statusCode, IEnumerable<string> messages, IDictionary<string, object>? metadata = null)
            : base(messages?.FirstOrDefault())
        {
            StatusCode = statusCode;
            Messages = messages?.ToList() ?? [];
            Metadata = metadata is not null
                ? new Dictionary<string, object>(metadata)
                : null;
        }
    }

    // Specific exception when an entity is not found
    public class EntityNullException : CustomApiException
    {
        public EntityNullException(string message, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.NotFound, message, metadata) { }

        public EntityNullException(IEnumerable<string> messages, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.NotFound, messages, metadata) { }
    }

    // Exception for validation errors
    public class ValidationException : CustomApiException
    {
        public ValidationException(string message, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.BadRequest, message, metadata) { }

        public ValidationException(IEnumerable<string> messages, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.BadRequest, messages, metadata) { }
    }

    // Exception when data already exists
    public class DataAlreadyExistsException : CustomApiException
    {
        public DataAlreadyExistsException(string message, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.Conflict, message, metadata) { }

        public DataAlreadyExistsException(IEnumerable<string> messages, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.Conflict, messages, metadata) { }
    }

    // Exception when a file is missing
    public class FileNullException : CustomApiException
    {
        public FileNullException(string message, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.NotFound, message, metadata) { }

        public FileNullException(IEnumerable<string> messages, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.NotFound, messages, metadata) { }
    }

    // Forbidden operation exception
    public class ForbiddenException : CustomApiException
    {
        public ForbiddenException(string message, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.Forbidden, message, metadata) { }

        public ForbiddenException(IEnumerable<string> messages, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.Forbidden, messages, metadata) { }
    }

    // Bad request exception
    public class BadRequestException : CustomApiException
    {
        public BadRequestException(string message, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.BadRequest, message, metadata) { }

        public BadRequestException(IEnumerable<string> messages, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.BadRequest, messages, metadata) { }
    }

    // Concurrency conflict exception
    public class ConcurrencyException : CustomApiException
    {
        public ConcurrencyException(string message, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.Conflict, message, metadata) { }

        public ConcurrencyException(IEnumerable<string> messages, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.Conflict, messages, metadata) { }
    }

    // Service unavailable exception
    public class ServiceUnavailableException : CustomApiException
    {
        public ServiceUnavailableException(string message, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.ServiceUnavailable, message, metadata) { }

        public ServiceUnavailableException(IEnumerable<string> messages, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.ServiceUnavailable, messages, metadata) { }
    }

    // Internal server error exception
    public class InternalServerErrorException : CustomApiException
    {
        public InternalServerErrorException(string message, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.InternalServerError, message, metadata) { }

        public InternalServerErrorException(IEnumerable<string> messages, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.InternalServerError, messages, metadata) { }
    }

    // Not found exception
    public class NotFoundException : CustomApiException
    {
        public NotFoundException(string message, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.NotFound, message, metadata) { }

        public NotFoundException(IEnumerable<string> messages, IDictionary<string, object>? metadata = null)
            : base((int)HttpStatusCode.NotFound, messages, metadata) { }
    }
}