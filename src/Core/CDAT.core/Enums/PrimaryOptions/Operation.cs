using System.ComponentModel;

namespace CDAT.core.Enums.UserOptions;
public enum Operation
{
    [Description("Convertir Texto a Voz")]
    TextToSpeech,

    [Description("Cerrar la Aplicación")]
    CloseApp
}
