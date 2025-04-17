using CDAT.core.Enums.OperationOptions;

namespace CDAT.core.Interfaces;

public interface IGoogleTextSpeechServices
{
    void ExecuteAsync(GoogleTextToSpeechOptions options);
}
