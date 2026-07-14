using System;
using System.IO;
using System.Text.Json;
using Avalonia.Input;

namespace AvaVibeTweak;

public class AvaVibeTweakSettings
{
    public Key ToggleKey { get; set; } = Key.F11;
    public Key SavePatchKey { get; set; } = Key.S;
    public KeyModifiers SavePatchModifiers { get; set; } = KeyModifiers.Control;
    public string Language { get; set; } = "en-US";
}

public static class SettingsManager
{
    private static readonly string SettingsFilePath;
    
    public static AvaVibeTweakSettings Instance { get; private set; } = new();

    static SettingsManager()
    {
        var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        SettingsFilePath = Path.Combine(dir ?? "", "AvaVibeTweakSettings.json");
        Load();
    }

    public static void Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                var settings = JsonSerializer.Deserialize<AvaVibeTweakSettings>(json);
                if (settings != null)
                {
                    Instance = settings;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AvaVibeTweak] Failed to load settings: {ex.Message}");
        }
    }

    public static void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(Instance, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AvaVibeTweak] Failed to save settings: {ex.Message}");
        }
    }

    public static void ResetToDefaults()
    {
        Instance = new AvaVibeTweakSettings();
        Save();
    }
}
