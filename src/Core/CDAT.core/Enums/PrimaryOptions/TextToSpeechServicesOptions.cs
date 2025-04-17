using System.ComponentModel;

namespace CDAT.core.Enums.UserOptions;
public enum TextToSpeechServicesOptions
{
    [Description("Google")]
    TextToSpeechGooogle,

    [Description("Otro Servicio de Text-to-Speech (no implementado)")]
    OtherTextToSpeechService,

    [Description("Volver")]
    Return
}
