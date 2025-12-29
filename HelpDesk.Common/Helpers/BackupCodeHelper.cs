namespace HelpDesk.Common.Helpers;

public static class BackupCodeHelper
{
    private const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static List<string> GenerateBackupCodes(int count = 8, int length = 8)
    {
        Random? rng = new();
        HashSet<string>? codes = [];

        while (codes.Count < count)
        {
            string? code = new(Enumerable.Range(0, length).Select(_ => characters[rng.Next(characters.Length)]).ToArray());
            codes.Add(code);
        }

        return codes.ToList();
    }

    public static List<string> HashBackupCodes(List<string> codes)
    {
        return codes.Select(code => BCrypt.Net.BCrypt.HashPassword(code)).ToList();
    }
}
