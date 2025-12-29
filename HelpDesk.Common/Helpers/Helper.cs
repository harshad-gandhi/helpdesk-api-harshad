namespace HelpDesk.Common.Helpers;

public static class Helper
{
    public static bool IsSuccess(int result) => result >= 0;

    public static class FileConstants
    {
        public static readonly string[] AllowedFileTypes =
        {
                "image/jpeg",
                "image/png",
                "application/pdf",
                "text/plain"
            };
    }

}
