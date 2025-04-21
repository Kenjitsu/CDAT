using CDAT.core.Interfaces;
using CDAT.core.Models.TextToSpeech;
using Spectre.Console;

namespace CDAT.core.Services;
public class CSVManagement : ICSVManagement
{
    public async Task<(List<CSVFileFormat> successfulRecords, List<string> errorLines)> GetValuesFromCSV(string csvFilePath)
    {
        var successfulRecords = new List<CSVFileFormat>();
        var errorLines = new List<string>();
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots2)
            .StartAsync("Leyendo archivo CSV...", async _ =>
            {
                await Task.Delay(300);
                string[] lines = await File.ReadAllLinesAsync(csvFilePath);

                foreach (var line in lines)
                {
                    var columns = line.Split(';');
                    if (columns.Length == 2)
                    {
                        successfulRecords.Add(new CSVFileFormat
                        {
                            AudioIdentifier = columns[0].Trim(),
                            TextToConvert = columns[1].Trim()
                        });
                    }
                    else
                    {
                        errorLines.Add($"Línea con formato incorrecto: '{line}'");
                    }
                }

                return (successfulRecords, errorLines);
            });

        return (successfulRecords, errorLines);
    }
}
