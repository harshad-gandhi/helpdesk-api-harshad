using System.Text.Json.Serialization;

namespace HelpDesk.Common.DTOs.ResponseDTOs;
public class TurnstileVerifyResponseDTO
{
    public bool Success { get; set; }

    // Single timestamp string
    public string Challenge_ts { get; set; } = string.Empty;

    public string Hostname { get; set; } = string.Empty;

    [JsonPropertyName("error-codes")]
    public string[] ErrorCodes { get; set; } = Array.Empty<string>();
}

