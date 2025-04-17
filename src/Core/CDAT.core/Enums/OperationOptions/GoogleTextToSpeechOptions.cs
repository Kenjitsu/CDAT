using System.ComponentModel;

namespace CDAT.core.Enums.OperationOptions;
public enum GoogleTextToSpeechOptions
{
    [Description("Instrucciones")]
    Instructions,

    [Description("Configuraciones")]
    Configurations,

    [Description("Ejecutar proceso")]
    ExecuteProcess,

    [Description("Salir")]
    Exit
}
