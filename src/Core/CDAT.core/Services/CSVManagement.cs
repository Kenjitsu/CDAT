using CDAT.core.Interfaces;
using CDAT.core.Models.TextToSpeech;

namespace CDAT.core.Services;
public class CSVManagement : ICSVManagement
{
    public (List<CSVFileFormat> successfulRecords, List<string> errorLines) GetValuesFromCSV(string csvFilePath)
    {
        var successfulRecords = new List<CSVFileFormat>();
        var errorLines = new List<string>();
        string[] lines = File.ReadAllLines(csvFilePath);

        if(lines.Length > 0)
        {
            lines = lines.Skip(1).ToArray();
        }

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
                errorLines.Add($"Línea con número incorrecto de columnas: '{line}'");
            }
        }

        return (successfulRecords, errorLines);
    }
}
