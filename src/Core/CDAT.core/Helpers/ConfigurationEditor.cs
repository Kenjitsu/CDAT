using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CDAT.core.Helpers;
public static class ConfigurationEditor
{
    public static void UpdateAppSettingsAsync(string sectionName, string propertyName, string newValue)
    {
        var path = Directory.GetCurrentDirectory();
        string appSettingsPath = $"{path}/appsettings.json";

        try
        {
            string jsonString = File.ReadAllText(appSettingsPath);
            var jsonObject = JsonNode.Parse(jsonString);

            if (jsonObject is not null)
            {
                var sectionNode = jsonObject[sectionName];

                if (sectionNode is JsonObject sectionObject)
                {
                    if (sectionObject.ContainsKey(propertyName))
                    {
                        sectionObject[propertyName] = JsonValue.Create(newValue);

                        var options = new JsonSerializerOptions { WriteIndented = true };
                        var updatedJsonString = jsonObject.ToJsonString(options);
                        File.WriteAllText(appSettingsPath, updatedJsonString);
                    }
                    else
                    {
                        AnsiConsole.WriteLine($"[yellow]La propiedad '{propertyName}' no se encontró en la sección '{sectionName}' del appsettings.json.[/]");
                    }
                }
                else
                {
                    AnsiConsole.WriteLine($"[yellow]La sección '{sectionName}' en appsettings.json no es un objeto.[/]");
                }
            }
            else
            {
                AnsiConsole.WriteLine($"[yellow]No se pudo parsear el archivo appsettings.json.[/]");
            }
        }
        catch (FileNotFoundException)
        {
            AnsiConsole.WriteLine($"[red]El archivo appsettings.json no se encontró en la ruta: '{appSettingsPath}'.[/]");
        }
        catch (JsonException ex)
        {
            AnsiConsole.WriteLine($"[red]Error al leer o parsear el archivo appsettings.json: {ex.Message}[/]");
        }
        catch (IOException ex)
        {
            AnsiConsole.WriteLine($"[red]Error de E/S al acceder al archivo appsettings.json: {ex.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"[red]Ocurrió un error inesperado al actualizar appsettings.json: {ex.Message}[/]");
        }
    }
}
