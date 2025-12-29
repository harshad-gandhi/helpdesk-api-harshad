using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using HelpDesk.Common.Constants;
using HelpDesk.Services.Interfaces;

namespace HelpDesk.Services.Implementations;

/// <summary>
/// Provides functionality for generating JWT access tokens and refresh tokens for authentication.
/// </summary>
public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Generates a JWT access token containing the user's email and ID as claims.
    /// </summary>
    /// <param name="userEmail">The email of the user.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A JWT token string valid for 15 minutes.</returns>
    public string GenerateJwtToken(string userEmail, int userId)
    {
        Claim[]? claims =
        {
            new Claim(ClaimTypes.Email, userEmail ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString() ?? string.Empty)
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SymmetricSecurityKey? key = new(Encoding.ASCII.GetBytes(_configuration[SystemConstant.JWT_KEY]!));
        SigningCredentials? creds = new(key, SecurityAlgorithms.HmacSha256Signature);

        SecurityTokenDescriptor? tokenDescriptor = new()
        {
            Issuer = _configuration[SystemConstant.JWT_ISSUER],
            Audience = _configuration[SystemConstant.JWT_AUDIENCE],
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = creds
        };

        SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Generates a secure random refresh token.
    /// </summary>
    /// <param name="expiryDays">Number of days until the refresh token expires.</param>
    /// <returns>A tuple containing the Base64-encoded token and its expiration time.</returns>
    public (string Token, DateTime Expires) GenerateRefreshToken(int expiryDays)
    {
        string? token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        DateTime expires = DateTime.UtcNow.AddDays(expiryDays);
        return (token, expires);
    }

}
