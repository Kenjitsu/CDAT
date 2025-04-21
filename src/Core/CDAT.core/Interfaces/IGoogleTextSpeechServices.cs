using CDAT.core.Enums.OperationOptions;

namespace CDAT.core.Interfaces;

public interface IGoogleTextSpeechServices
{
    Task ExecuteAsync(GoogleTextToSpeechOptions options);
}
