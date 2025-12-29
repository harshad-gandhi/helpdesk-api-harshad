namespace HelpDesk.Common.DTOs.ResponseDTOs
{

    #region Api Response

    public class ApiResponse
    {
        public bool Result { get; set; }

        public int HttpStatusCode { get; set; }

        public IReadOnlyList<string>? Messages { get; set; } = [];
    }

    #endregion

    #region  Success Api Response 

    public class SuccessApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        public SuccessApiResponse(int httpStatusCode, List<string> message, T? data)
        {
            Result = true;
            HttpStatusCode = httpStatusCode;
            Messages = message;
            Data = data;
        }
    }

    #endregion

    #region Error Api Response

    public class ErrorApiResponse : ApiResponse
    {
        public IReadOnlyDictionary<string, object>? Metadata { get; set; }

        public string? File { get; set; }

        public List<StackFrameInfo>? ExceptionStack { get; set; }
    }

    #endregion

    #region Stack Frame Info

    public class StackFrameInfo
    {
        public string? FileName { get; set; }

        public int? LineNumber { get; set; }

        public string? Method { get; set; }

        public string? ExceptionType { get; set; }
    }

    #endregion

}