using CDAT.core.Models.TextToSpeech.Google;
using CDAT.core.Models.TextToSpeech.Google.Configurations;

namespace CDAT.core.Interfaces.Infrastructure;
public interface IGoogleApiServices
{
    Task<GoogleTextToSpeechApiResponse> TextToSpeechApi(GoogleTextToSpeechApiRequest request, GoogleTextToSpeechConfig config);
}
