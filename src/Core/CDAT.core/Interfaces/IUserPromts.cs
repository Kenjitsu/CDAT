using CDAT.core.Enums;
using Spectre.Console;

namespace CDAT.core.Interfaces;

public interface IUserPromts
{
    string GetPromptFromUser(string message);
    string GetPromptFromUserFromSelection<T>(string promptMessage, T obj) where T : class;
    TEnum GetPromptChoicesByEnum<TEnum>(string title) where TEnum : Enum;
    bool ConfirmUserChioce(string message);
    Task ShowSpinnerAsync(Func<StatusContext, Task> actionToExecute, string message);
    void GenerateTable<T>(T obj, string title = "Detalles del Objeto") where T : class;
    Task ShowProgressAsync(string title, Func<ProgressContext, Task> action, int? totalSteps = null);
    void RenderBarChartWithResult((int success, int failures) result, string label);
}
