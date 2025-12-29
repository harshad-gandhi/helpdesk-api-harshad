namespace HelpDesk.Services.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(string userEmail, int userId);
    (string Token, DateTime Expires) GenerateRefreshToken(int expiryDays);
}
