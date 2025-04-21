using CDAT.core.Enums.OperationOptions;
using CDAT.core.Enums.UserOptions;
using CDAT.core.Helpers;
using CDAT.core.Interfaces;
using Spectre.Console;

namespace CDAT.core.Services;
public class ProcessService : IProcessService
{
    private readonly IUserPromts _userPromts;
    private readonly IGoogleTextSpeechServices _googleTextSpeechServices;
    private bool _isAppRunning = true;
    private bool _isOperationRunning = false; // Nuevo estado para controlar las operaciones asíncronas

    public ProcessService(IUserPromts operationToPerformService, IGoogleTextSpeechServices googleTextSpeechServices)
    {
        _userPromts = operationToPerformService;
        _googleTextSpeechServices = googleTextSpeechServices;
    }

    public async Task ProcessAsync()
    {
        try
        {
            while (_isAppRunning)
            {
                if (!_isOperationRunning)
                {
                    var operationToPerform = _userPromts.GetPromptChoicesByEnum<Operation>("¡Hola!, ¿Qué operaciónes deseas realizar?");
                    await HandleSelectedOperation(operationToPerform);
                }
                else
                {
                    await Task.Delay(200);
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        }
        finally
        {
            await AnsiConsole
                .Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("Finalizando la aplicación...", async _ =>
                {
                    await Task.Delay(1000);
                });
        }
    }

    private async Task HandleSelectedOperation(Operation operation)
    {
        _isOperationRunning = true;
        bool isMenuActive = true;
        try
        {
            switch (operation)
            {
                case Operation.TextToSpeech:
                    while (isMenuActive)
                    {
                        var textToSpeechOption = _userPromts.GetPromptChoicesByEnum<TextToSpeechServicesOptions>("Elige el servicio de TextToSpeech que deseas usar");

                        if (textToSpeechOption == TextToSpeechServicesOptions.OtherTextToSpeechService || textToSpeechOption == TextToSpeechServicesOptions.Return)
                        {
                            isMenuActive = false;
                            continue;
                        }

                        if (textToSpeechOption == TextToSpeechServicesOptions.TextToSpeechGooogle)
                        {
                            isMenuActive = true;
                            while (isMenuActive)
                            {
                                var serviceOptions = _userPromts.GetPromptChoicesByEnum<GoogleTextToSpeechOptions>($"Elige la opción que deseas ver del servicio de texto a voz de {textToSpeechOption.GetDescription()}");

                                if (serviceOptions == GoogleTextToSpeechOptions.Instructions)
                                {
                                    await _googleTextSpeechServices.ExecuteAsync(serviceOptions);
                                }
                                else if (serviceOptions == GoogleTextToSpeechOptions.Configurations)
                                {
                                    await _googleTextSpeechServices.ExecuteAsync(serviceOptions);
                                }
                                else if (serviceOptions == GoogleTextToSpeechOptions.ExecuteProcess)
                                {
                                    await _googleTextSpeechServices.ExecuteAsync(serviceOptions);
                                }
                                else if (serviceOptions == GoogleTextToSpeechOptions.Exit)
                                {
                                    isMenuActive = false;
                                }
                            }
                        }
                        else
                        {
                            isMenuActive = false;
                        }
                    }
                    break;
                case Operation.CloseApp:
                    bool isCloseAppRequested = _userPromts.ConfirmUserChioce("¿Está seguro que desea cerrar la aplicación?");
                    if (isCloseAppRequested)
                    {
                        _isAppRunning = false;
                    }
                    break;
                default:
                    throw new NotImplementedException($"Operación no implementada: {operation}");
            }
        }
        finally
        {
            _isOperationRunning = false;
        }
    }
}