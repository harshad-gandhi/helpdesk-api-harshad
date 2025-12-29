using System.Net;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HelpDesk.Services.Implementations
{
    public class ResponseService<T>(IStringLocalizer<Messages> localizer) : IResponseService<T>
    {
        private readonly IStringLocalizer<Messages> _localizer = localizer;


        #region GetSuccessResponse

        public IActionResult GetSuccessResponse(HttpStatusCode statusCode, T? result, List<string>? messages)
        {
            messages ??= [_localizer["REQUEST_PROCESSED_SUCCESSFULLY"]];

            SuccessApiResponse<T> apiResponse = new(
                (int)statusCode,
                messages,
                result
            );

            return new ObjectResult(apiResponse) { StatusCode = (int)statusCode };
        }

        #endregion

        #region GetErrorResponse

        public IActionResult GetErrorResponse(HttpStatusCode statusCode, List<string>? errors, IReadOnlyDictionary<string, object>? metadata = null)
        {
            ErrorApiResponse apiResponse = new()
            {
                Result = false,
                HttpStatusCode = (int)statusCode,
                Messages = errors ?? [_localizer["UNEXPECTED_ERROR_OCCURRED"]],
                Metadata = metadata
            };

            return new ObjectResult(apiResponse) { StatusCode = (int)statusCode };
        }

        #endregion

    }
}
