using CDAT.core.Enums.OperationOptions;
using CDAT.core.Helpers;
using CDAT.core.Interfaces;
using CDAT.core.Interfaces.Infrastructure;
using CDAT.core.Models.TextToSpeech;
using CDAT.core.Models.TextToSpeech.Google;
using CDAT.core.Models.TextToSpeech.Google.Configurations;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace CDAT.core.Services;
public class GoogleTextSpeechServices : IGoogleTextSpeechServices
{
    private readonly IGoogleApiServices _googleApiServices;
    private readonly ICSVManagement _cSVManagement;
    private readonly IUserPromts _userPrompts;
    private readonly IConfiguration _configuration;
    public GoogleTextToSpeechConfig googleTextToSpeechConfig;


    public GoogleTextSpeechServices(IGoogleApiServices googleApiServices, ICSVManagement cSVManagement, IConfiguration configuration, IUserPromts userPrompts)
    {
        _googleApiServices = googleApiServices;
        _cSVManagement = cSVManagement;
        _userPrompts = userPrompts;
        _configuration = configuration;

        googleTextToSpeechConfig = configuration.GetSection("GoogleTextToSpeechConfig").Get<GoogleTextToSpeechConfig>()!;
    }

    public async Task ExecuteAsync(GoogleTextToSpeechOptions options)
    {

        switch (options)
        {
            case GoogleTextToSpeechOptions.Instructions:
                Instructions();
                break;
            case GoogleTextToSpeechOptions.Configurations:
                Configurations();
                break;
            case GoogleTextToSpeechOptions.ExecuteProcess:
                var results = await GetTextToSpeechAudio();
                _userPrompts.RenderBarChartWithResult(results, "Resultados de textos procesados");
                break;
        }
    }

    //private async IAsyncEnumerable<GoogleTextToSpeechApiResponse> GetTextToSpeechAudio()
    //{
    //    var (records, errors) = _cSVManagement.GetValuesFromCSV(googleTextToSpeechConfig.CSVFilePath!);
    //    bool continueProcess = _userPrompts.ConfirmUserChioce($"Se encontraron [yellow]{records.Count}[/] registros a procesar y se encontraron [yellow]{errors.Count}[/] que no fue posible leer. ¿Desea continuar?");
    //    if (continueProcess)
    //    {
    //        foreach (var record in records) 
    //        {
    //            await Task.Delay(TimeSpan.FromMilliseconds(350));
    //            GoogleTextToSpeechApiRequest request = GetRequestForGoogleApi(record);
    //            var response = await _googleApiServices.TextToSpeechApi(request, googleTextToSpeechConfig);

    //            if (response != null)
    //            {
    //                yield return response;
    //            }
    //        }
    //    }
    //}

    private async Task<(int successes, int failures)> GetTextToSpeechAudio()
    {
        var (records, errors) = await _cSVManagement.GetValuesFromCSV(googleTextToSpeechConfig.CSVFilePath!);

        if (errors.Count != 0)
        {
            foreach (var error in errors)
            {
                AnsiConsole.MarkupLine("[yellow]{0}.[/]", error);
            }
        }

        bool continueProcess = _userPrompts.ConfirmUserChioce($"Se encontraron [yellow]{records.Count}[/] registros a procesar y se encontraron [yellow]{errors.Count}[/] que no fue posible leer. ¿Desea continuar?");

        int succecess = 0;
        int failures = 0;

        if (continueProcess)
        {
            var executionResults = await AnsiConsole
                .Progress()
                .Columns(
                [
                    new TaskDescriptionColumn(),    // Task description
                    new ProgressBarColumn(),        // Progress bar
                    new PercentageColumn(),         // Percentage
                    new RemainingTimeColumn(),      // Remaining time
                    new SpinnerColumn(),            // Spinner
                    new DownloadedColumn(),         // Downloaded
                    new TransferSpeedColumn(),      // Transfer speed
                ])
                .StartAsync(async ctx =>
                {
                    var executionTask = ctx.AddTask("Convirtiendo texto a audio...", maxValue: records.Count);

                    foreach (var record in records)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(350));
                        GoogleTextToSpeechApiRequest request = GetRequestForGoogleApi(record);
                        var response = await _googleApiServices.TextToSpeechApi(request, googleTextToSpeechConfig);

                        if (response.IsSuccessfulResponse)
                        {
                            succecess++;
                            var audioBase64 = response.AudioContent;
                            byte[] audioBytes = Convert.FromBase64String(audioBase64!);
                            File.WriteAllBytes($"{googleTextToSpeechConfig.OutputPathAudioFiles!}{record.AudioIdentifier}.wav", audioBytes);
                        }
                        else if(!string.IsNullOrWhiteSpace(response.Error))
                        {
                            AnsiConsole.MarkupLine($"Respuesta del servicio de google: [red]{response.Error}[/], por favor validar configuraciones o estado de los servicios");
                            break;
                        }
                        else
                        {
                            failures++;
                        }

                        executionTask.Increment(1);
                    }

                    return (succecess, failures);

                });
        }

        return (succecess, failures);
    }

    private void Configurations()
    {
        _userPrompts.GenerateTable(googleTextToSpeechConfig, "Configuración de Google Text-to-Speech");
        var confirmConfigurations = _userPrompts.ConfirmUserChioce("¿Desea editar estos campos de configuración?");
        if (confirmConfigurations)
        {
            bool continueEditing = true;

            while (continueEditing)
            {
                string propertyToEdit = _userPrompts.GetPromptFromUserFromSelection("¿Cual configuración quiere editar?", googleTextToSpeechConfig);
                if (string.IsNullOrEmpty(propertyToEdit))
                {
                    continueEditing = false;
                    continue;
                }

                PropertyInfo propertyInfo = typeof(GoogleTextToSpeechConfig).GetProperty(propertyToEdit);
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    string newValueOfProperty = _userPrompts.GetPromptFromUser($"Por favor, asigne un nuevo valor para la propiedad de configuración [cyan]{propertyToEdit}[/] (actual: [grey]{propertyInfo.GetValue(googleTextToSpeechConfig)?.ToString() ?? "N/A"}[/]):");

                    if (newValueOfProperty != null)
                    {
                        try
                        {
                            object convertedValue = Convert.ChangeType(newValueOfProperty, propertyInfo.PropertyType);
                            propertyInfo.SetValue(googleTextToSpeechConfig, convertedValue);
                            AnsiConsole.MarkupLine($"Propiedad: [yellow]{propertyToEdit}[/] asignada con el nuevo valor [yellow]{newValueOfProperty}[/]");

                            ConfigurationEditor.UpdateAppSettingsAsync("GoogleTextToSpeechConfig", propertyToEdit, newValueOfProperty);
                        }
                        catch (FormatException)
                        {
                            AnsiConsole.WriteLine($"[red]El valor ingresado no tiene el formato correcto para el tipo de la propiedad '{propertyToEdit}'.[/]");
                        }
                        catch (InvalidCastException)
                        {
                            AnsiConsole.WriteLine($"[red]No se puede convertir el valor ingresado al tipo de la propiedad '{propertyToEdit}'.[/]");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.WriteLine($"[red]Ocurrió un error al intentar actualizar la propiedad: {ex.Message}[/]");
                        }
                    }
                }
                else
                {
                    AnsiConsole.WriteLine($"[yellow]La propiedad '{propertyToEdit}' no se puede editar.[/]");
                }

                continueEditing = _userPrompts.ConfirmUserChioce("¿Desea continuar editando las configuraciones?");
            }
        }
    }

    private void Instructions()
    {
        var instructionsTree = new Tree("Instrucciones")
        .Style(Style.Parse("red"))
        .Guide(TreeGuide.Line);

        var csvInfo = instructionsTree.AddNode("Generar un CSV delimitado por comas con el siguiente formato de ejemplo:");
        csvInfo.AddNodes("IdentificadorDAudio;Texto a convertir en voz con el api de google");
        instructionsTree.AddNode("Definir la ruta del archivo CSV y configurarla en el archivo appsettings.json o en la opción de configuraciones");
        instructionsTree.AddNode("Definir la ruta donde se alojaran los audios creados y configurarla en el archivo appsettings.json o en la opción de configuraciones");
        var bearerInfo = instructionsTree.AddNode("Obtener el bearer token del consumo del api en la página de google:");
        bearerInfo.AddNode("Dirigirse a la url del servicio de google");
        bearerInfo.AddNode("Abrir las herramientas de desarrollador");
        bearerInfo.AddNode("Hacer el consumo del servicio directamente desde la página");
        bearerInfo.AddNode("En las herramientas de desarrollo, buscar el tab 'Network'");
        bearerInfo.AddNode("Buscar la petición con el siguiente nombre 'text:synthesize'");
        bearerInfo.AddNode("En la sección headers, buscar el header 'Authorization' y copiar el token");
        bearerInfo.AddNode("Configurar el token en el appsettings.json o directamente en la sección de configuraciones");
        instructionsTree.AddNode("Una vez confirmados los pasos anteriores, ejecutar el proceso y esperar los resultados.");

        AnsiConsole.Write(instructionsTree);
    }

    private GoogleTextToSpeechApiRequest GetRequestForGoogleApi(CSVFileFormat record)
    {
        var request = new GoogleTextToSpeechApiRequest()
        {
            Input = new Input() { Text = record.TextToConvert! },
            AudioConfig = new AudioConfig(),
            Voice = new Voice(),
        };

        return request;
    }
}
