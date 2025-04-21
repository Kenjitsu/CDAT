using System.Text.Json.Serialization;

namespace CDAT.core.Models.TextToSpeech.Google;
public class GoogleTextToSpeechApiResponse
{
    [JsonPropertyName("audioContent")]
    public string? AudioContent { get; set; }

    public bool IsSuccessfulResponse { get; set; }

    public string Error { get; set; } = string.Empty;
}
