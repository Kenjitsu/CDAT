using CDAT.core.Enums.OperationOptions;
using CDAT.core.Interfaces;
using CDAT.core.Interfaces.Infrastructure;
using CDAT.core.Models.TextToSpeech.Google;
using CDAT.core.Models.TextToSpeech.Google.Configurations;
using RestSharp;
using Spectre.Console;
using Spectre.Console.Json;
using System.Text.Json;

namespace CDAT.infrastructure.ThirdParty;
public class GoogleApiServices : IGoogleApiServices
{
    public async Task<GoogleTextToSpeechApiResponse> TextToSpeechApi(GoogleTextToSpeechApiRequest bodyRequest, GoogleTextToSpeechConfig config)
    {
        RestResponse response = new();
        try
        {
            var client = new RestClient();
            var request = new RestRequest(config.BaseUrl, Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", $"Bearer {config.BearerToken!}")
                .AddJsonBody(bodyRequest);

            //AnsiConsole.WriteLine("request: {0}", JsonSerializer.Serialize(bodyRequest));
            response = await client.PostAsync(request);

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var googleResponse = JsonSerializer.Deserialize<GoogleTextToSpeechApiResponse>(response.Content!);
                googleResponse!.IsSuccessfulResponse = true;

                return googleResponse!;

            }

            return new GoogleTextToSpeechApiResponse() { IsSuccessfulResponse = false};
        }
        catch (Exception ex) 
        {
            AnsiConsole.WriteException(ex);
            return new GoogleTextToSpeechApiResponse() { IsSuccessfulResponse = false, Error = ex.Message };
        }
    }
}
