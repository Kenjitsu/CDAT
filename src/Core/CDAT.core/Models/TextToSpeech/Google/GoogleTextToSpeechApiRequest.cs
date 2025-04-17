using System.Text.Json.Serialization;

namespace CDAT.core.Models.TextToSpeech.Google;
public class GoogleTextToSpeechApiRequest
{
    [JsonPropertyName("text")]
    public required Input Input { get; set; }

    [JsonPropertyName("voice")]
    public required Voice Voice { get; set; }

    [JsonPropertyName("audioConfig")]
    public required AudioConfig AudioConfig { get; set; }
}

public class Input
{
    [JsonPropertyName("text")]
    public required string Text { get; set; }
}

public class Voice
{
    [JsonPropertyName("languageCode")]
    public string LanguageCode { get; set; } = "es-US";

    [JsonPropertyName("ssmlGender")]
    public string SsmlGender { get; set; } = "MALE";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "es-US-Studio-B";
}

public class AudioConfig
{
    [JsonPropertyName("audioEncoding")]
    public string AudioEncoding { get; set; } = "LINEAR16";

    [JsonPropertyName("sampleRateHertz")]
    public int SampleRateHertz { get; set; } = 8000;
}