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
                var operationToPerform = _userPromts.GetPromptChoicesByEnum<Operation>("¡Hola!, ¿Qué operaciónes deseas realizar?");
                HandleSelectedOperation(operationToPerform);
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
            .Spinner(Spinner.Known.Dots2)
            .StartAsync("Finalizando la aplicación...", async _ =>
            {
                await Task.Delay(1000);
            });
        }


    }

    private void HandleSelectedOperation(Operation operation)
    {
        bool isMenuActive = true;
        switch (operation)
        {
            case Operation.TextToSpeech:
                while(isMenuActive)
                {
                    var textToSpeechOption = _userPromts.GetPromptChoicesByEnum<TextToSpeechServicesOptions>("Elige el servicio de TextToSpeech que deseas usar");

                    if (textToSpeechOption == TextToSpeechServicesOptions.OtherTextToSpeechService)
                    {
                        isMenuActive = false;
                    }

                    if (textToSpeechOption == TextToSpeechServicesOptions.Return)
                    {
                        isMenuActive = false;
                    }

                    isMenuActive = false;

                    if (textToSpeechOption == TextToSpeechServicesOptions.TextToSpeechGooogle)
                    {
                        var serviceOptions = _userPromts.GetPromptChoicesByEnum<GoogleTextToSpeechOptions>($"Elige la opción que deseas ver del servicio de texto a voz de {textToSpeechOption.GetDescription()}");

                        if (serviceOptions == GoogleTextToSpeechOptions.Instructions)
                        {
                            _googleTextSpeechServices.ExecuteAsync(serviceOptions);
                        }

                        if (serviceOptions == GoogleTextToSpeechOptions.Configurations)
                        {
                            _googleTextSpeechServices.ExecuteAsync(serviceOptions);
                        }

                        if (serviceOptions == GoogleTextToSpeechOptions.Exit)
                        {
                            isMenuActive = true;
                        }
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


}
