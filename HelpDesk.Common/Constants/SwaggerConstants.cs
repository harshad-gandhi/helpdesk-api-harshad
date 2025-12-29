namespace HelpDesk.Common.Constants
{
    public class SwaggerConstants
    {

        #region  Swagger Constant

        public const string API_TITLE = "HelpDesk API";

        public const string API_VERSION = "v1";

        public const string API_SECURITY_SCHEME = "Bearer";

        public const string API_SECURITY_SCHEME_FORMAT = "JWT";

        public const string API_SECURITY_SCHEME_NAME = "Authorization";

        public const string API_SECURITY_SCHEME_DESCRIPTION = "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
                                      "Enter your token in the text input below.\r\n\r\n" +
                                      "Example: \"1234abcedf\" ";

        public const string API_END_POINT = "/swagger/v1/swagger.json";

        #endregion

        #region Multi Language Constants

        public const string OPEN_API_PARAMETER_NAME = "Accept-Language";

        public const string OPEN_API_PARAMETER_SCHEMA_TYPE = "string";

        public const string OPEN_API_DEFAULT_LANGUAGE = "en";

        public const string OPEN_API_DESCRIPTION = "Language preference (e.g., 'en' or 'hi')";

        #endregion

        #region Language Code

        public const string ENGLISH = "en";

        public const string HINDI = "hi";

        #endregion

    }
}