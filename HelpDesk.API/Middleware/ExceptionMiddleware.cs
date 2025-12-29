using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Authentication;
using System.Diagnostics;
using Microsoft.Extensions.Localization;
using HelpDesk.Common.Constants;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Services.Interfaces;
using HelpDesk.Common.Resources;

namespace HelpDesk.API.Middleware
{
    public class ExceptionMiddleware(ILoggerService loggerService, IStringLocalizer<Messages> localizer) : IMiddleware
    {
        private readonly ILoggerService _loggerService = loggerService;

        private readonly IStringLocalizer<Messages> _localizer = localizer;

        #region Invoke

        // Middleware entry point to catch exceptions 
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(httpContext, exception);
            }
        }

        #endregion

        #region  Handle Exception

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {

            string traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;

            // Localized generic exception message for logging
            string localizedMessage = _localizer["GLOBAL_LOG_EXCEPTION_MESSAGE"];

            // Log the exception with request info and trace
            _loggerService.LogError(exception,
                localizedMessage,
                httpContext.Request.Method,
                httpContext.Request.Path + httpContext.Request.QueryString,
                Environment.MachineName,
                traceId);

            // Map exception to HTTP status and messages
            (int httpStatusCode, IReadOnlyList<string> messages, IReadOnlyDictionary<string, object>? metadata) = GetExceptionInfo(exception);

            string? sourceFile = null;

            // Extract stack frames for detailed error response
            StackTrace stackTrace = new(exception, true);
            StackFrame[] frames = stackTrace.GetFrames();

            List<StackFrameInfo>? stackFrameInfos = frames?.Select((f, index) => new StackFrameInfo
            {
                FileName = f.GetFileName() is not null ? Path.GetFileName(f.GetFileName()) : null,
                LineNumber = f.GetFileLineNumber() > 0 ? f.GetFileLineNumber() : null,
                Method = ExtractMethodName(f),
                ExceptionType = exception.GetType().Name
            }).Where(sf => sf.FileName is not null).ToList();

            ErrorApiResponse errorApiResponse = new()
            {
                Result = false,
                HttpStatusCode = httpStatusCode,
                Messages = messages,
                Metadata = metadata,
                File = sourceFile,
                ExceptionStack = stackFrameInfos
            };

            httpContext.Response.ContentType = SystemConstant.APPLICATION_JSON;
            httpContext.Response.StatusCode = errorApiResponse.HttpStatusCode;

            JsonSerializerSettings serializerSettings = new()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            // Check if request is from Angular frontend to send minimal response
            bool isAngular = httpContext.Request.Headers[SystemConstant.HEADER_CLIENT_TYPE] == SystemConstant.HEADER_CLIENT;

            if (isAngular)
            {

                ApiResponse minimalResponse = new()
                {
                    Result = errorApiResponse.Result,
                    HttpStatusCode = errorApiResponse.HttpStatusCode,
                    Messages = errorApiResponse.Messages
                };

                string jsonResponse = JsonConvert.SerializeObject(minimalResponse, serializerSettings);
                await httpContext.Response.WriteAsync(jsonResponse);
            }
            else
            {
                string jsonResponse = JsonConvert.SerializeObject(errorApiResponse, serializerSettings);
                await httpContext.Response.WriteAsync(jsonResponse);
            }
        }

        #endregion

        #region Extract Method Name

        // Extracts method name from stack frame, handles compiler-generated names
        private static string? ExtractMethodName(StackFrame frame)
        {
            System.Reflection.MethodBase? method = frame.GetMethod();
            string? typeName = method?.DeclaringType?.Name;

            if (!string.IsNullOrEmpty(typeName) && typeName.Contains('<'))
            {
                int start = typeName.IndexOf('<') + 1;
                int end = typeName.IndexOf('>');
                if (start > 0 && end > start)
                    return typeName[start..end];
            }

            return method?.Name;
        }

        #endregion

        #region Get Exception Info

        // Maps exceptions to HTTP status code 
        public (int httpStatusCode, IReadOnlyList<string> messages, IReadOnlyDictionary<string, object>? metadata) GetExceptionInfo(Exception exception)
        {
            return exception switch
            {
                CustomApiException customEx => (customEx.StatusCode, customEx.Messages, customEx.Metadata),

                HttpProtocolException => ((int)HttpStatusCode.InternalServerError, [_localizer["INTERNAL_SERVER"]], null),

                UnauthorizedAccessException uaEx => ((int)HttpStatusCode.Unauthorized, [uaEx.Message], null),

                AuthenticationException authEx => ((int)HttpStatusCode.Unauthorized, [authEx.Message], null),

                InvalidOperationException => ((int)HttpStatusCode.BadRequest, [_localizer["INTERNAL_SERVER"]], null),

                ArgumentNullException argNullEx => ((int)HttpStatusCode.BadRequest, [argNullEx.Message], null),

                ArgumentException argEx => ((int)HttpStatusCode.BadRequest, [argEx.Message], null),

                JsonException jsonEx => ((int)HttpStatusCode.BadRequest, [jsonEx.Message], null),

                TaskCanceledException or TimeoutException => ((int)HttpStatusCode.RequestTimeout, [_localizer["SESSION_TIMEOUT"]], null),

                FormatException => ((int)HttpStatusCode.NotFound, [_localizer["INVALID_SORTCOLUMN"]], null),

                SecurityTokenExpiredException => ((int)HttpStatusCode.Unauthorized, [_localizer["TOKEN_EXPIRED"]], null),

                _ => ((int)HttpStatusCode.InternalServerError, [exception.Message], null)
            };
        }

        #endregion

    }
}