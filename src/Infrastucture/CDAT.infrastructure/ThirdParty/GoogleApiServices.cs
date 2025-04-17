using CDAT.core.Enums.OperationOptions;
using CDAT.core.Interfaces;
using CDAT.core.Interfaces.Infrastructure;
using CDAT.core.Models.TextToSpeech.Google;
using CDAT.core.Models.TextToSpeech.Google.Configurations;
using RestSharp;
using Spectre.Console;
using Spectre.Console.Json;
using System.Text.Json;

namespace CDAT.core.Services;
public class GoogleApiServices : IGoogleApiServices
{
    public async Task<GoogleTextToSpeechApiResponse> TextToSpeechApi(GoogleTextToSpeechApiRequest bodyRequest, GoogleTextToSpeechConfig config)
    {
        try
        {
            var client = new RestClient();
            var request = new RestRequest(config.BaseUrl, Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", config.BearerToken!);

            var response = await client.PostAsync(request);

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var googleResponse = JsonSerializer.Deserialize<GoogleTextToSpeechApiResponse>(response.Content!);

                return googleResponse!;

            }

            return new GoogleTextToSpeechApiResponse();
        }
        catch (Exception ex) 
        {
            throw new Exception("A ocurrido un error al consumir el api de google");
        }
    }
}
