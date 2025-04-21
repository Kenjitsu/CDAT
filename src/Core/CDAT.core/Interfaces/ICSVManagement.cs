using CDAT.core.Models.TextToSpeech;

namespace CDAT.core.Interfaces;
public interface ICSVManagement
{
    Task<(List<CSVFileFormat> successfulRecords, List<string> errorLines)> GetValuesFromCSV(string csvFilePath);
}
