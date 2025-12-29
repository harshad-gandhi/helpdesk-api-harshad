namespace HelpDesk.Common.Constants
{
    public class SystemConstant
    {

        #region General

        public const string CORS_POLICY_NAME = "AllowAllPolicy";

        // Change these URL in production
        public const string ANGULAR_BASE_URL = "http://localhost:4200";

        public const string API_BASE_URL = "http://localhost:5093";

        public const string HEADER_CLIENT_TYPE = "X-Client-Type";

        public const string HEADER_CLIENT = "Angular";

        public const string CONNECTION_STRING_NAME = "DefaultConnection";

        public const string APPLICATION_JSON = "application/json";

        public const string REFRESH_TOKEN_COOKIE_NAME = "refreshToken";

        public const string ROOT_FOLDER = "wwwroot";

        public const string UPLOADS_FOLDER = "uploads";

        public const string ROOT_FOLDER_Path = "wwwroot\\";

        #endregion

        #region JWT

        public const string JWT_KEY = "JwtSettings:Key";

        public const string JWT_ISSUER = "JwtSettings:Issuer";

        public const string JWT_AUDIENCE = "JwtSettings:Audience";

        #endregion

        #region Logs

        public const string OUTPUT_TEMPLATE = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        public const string LOGS_FOLDER_NAME = "logs";

        public const string LOGS_FILE_NAME = "log-.txt";

        #endregion

        #region  Entity

        public const string USERS = "Users";

        public const string USER = "User";

        public const string DIRECT_MESSAGE_ATTACHMENT = "Direct Message attachment";

        public const string KEYWORD = "Keyword";

        public const string ROLES = "Roles";

        #endregion

        #region MimeType

        public const string JPEG = "image/jpeg";

        public const string PNG = "image/png";

        public const string PDF = "application/pdf";

        public const string TEXT_PLAN = "text/plain";

        public const string TEXT_HTML = "text/html";

        public const string UNKNOWN = "unknown";

        #endregion

        #region API Endpoints

        public const string API_COUNTRIES = "api/countries";

        public const string API_DIRECT_MESSAGES = "api/direct-messages";

        public const string API_REPORTING = "api/reporting";

        public const string API_CHAT_SHORTCUT_MESSAGES = "api/chat-shortcut-messages";

        public const string API_AUTHENTICATION = "api/auth";

        public const string API_ROLES = "api/roles";

        public const string API_USERS = "api/users";

        #endregion

    }
}