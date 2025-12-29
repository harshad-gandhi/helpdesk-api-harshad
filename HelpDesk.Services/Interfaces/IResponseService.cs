using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Services.Interfaces
{
    public interface IResponseService<T>
    {
        IActionResult GetSuccessResponse(HttpStatusCode statusCode, T? result, List<string>? messages = null);

        IActionResult GetErrorResponse(HttpStatusCode statusCode, List<string>? errors, IReadOnlyDictionary<string, object>? metadata = null);
    }

}