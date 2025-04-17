using CDAT.core.Enums;
using CDAT.core.Helpers;
using CDAT.core.Interfaces;
using Spectre.Console;
using System.Reflection;

namespace CDAT.core.Services;

public class UserPromts : IUserPromts
{
    public string GetPromptFromUser(string message)
    {
        var dataPromptedByUser = AnsiConsole.Prompt(
    new TextPrompt<string>(message));

        return dataPromptedByUser.Trim();
    }

    public string GetPromptFromUserFromSelection<T>(string promptMessage, T obj) where T : class
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj), "El objeto no puede ser nulo.");
        }

        var properties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .Select(p => p.Name)
            .ToList();

        if (!properties.Any())
        {
            AnsiConsole.WriteLine("[yellow]El objeto no tiene propiedades públicas legibles.[/]");
            return null;
        }

        var selectedProperty = AnsiConsole.Prompt(
            new TextPrompt<string>(promptMessage)
            .InvalidChoiceMessage("[red]¡Esa no es una opción valida![/]")
            .AddChoices(properties)
        );

        return selectedProperty;
    }

    public TEnum GetPromptChoicesByEnum<TEnum>(string title) where TEnum : Enum
    {
        var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        var descriptionMap = new Dictionary<string, TEnum>();

        foreach (var enumValue in enumValues)
        {
            string description = enumValue.GetDescription();
            descriptionMap.Add(description, enumValue);
        }

        var selectedDescription = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .PageSize(10)
                .AddChoices(descriptionMap.Keys.ToList())
        );

        return descriptionMap[selectedDescription];
    }

    public bool ConfirmUserChioce(string message)
    {
        var option = AnsiConsole.Prompt(
            new SelectionPrompt<bool> { Converter = value => value ? "Si" : "No" }
            .Title(message)
            .AddChoices(true, false));

        return option;
    }

    public async Task ShowSpinnerAsync(Func<StatusContext, Task> actionToExecute, string message)
    {
        await AnsiConsole
            .Status()
            .Spinner(Spinner.Known.Dots2)
            .StartAsync(message, actionToExecute);
    }

    public void GenerateTable<T>(T obj, string title = "Detalles del Objeto") where T : class
    {
        if (obj == null)
        {
            AnsiConsole.WriteLine("[red]El objeto proporcionado es nulo.[/]");
            return;
        }

        var table = new Table()
            .Title($"[white bold]{title}[/]")
            .BorderColor(Color.White)
            .AddColumn("[white bold]Propiedad[/]")
            .AddColumn("[white bold]Valor[/]");

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var propertyName = property.Name;
            var propertyValue = property.GetValue(obj)?.ToString() ?? "N/A";
            table.AddRow($"[white]{propertyName}[/]", $"[white]{propertyValue}[/]");
        }

        AnsiConsole.Write(table);
    }
}
