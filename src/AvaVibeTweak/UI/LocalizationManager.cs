using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;

namespace AvaVibeTweak.UI;

public static class LocalizationManager
{
    public static event EventHandler? LanguageChanged;

    public static string CurrentLanguage { get; private set; } = "en-US";

    public static void Initialize(string defaultLocale = "en-US")
    {
        ChangeLanguage(defaultLocale);
    }

    public static string GetString(string key)
    {
        if (Application.Current != null && Application.Current.TryGetResource(key, Application.Current.ActualThemeVariant, out var res) && res is string s)
        {
            return s;
        }
        return key; // Fallback to key if not found
    }

    public static void ChangeLanguage(string locale)
    {
        CurrentLanguage = locale;
        if (Application.Current == null) return;

        var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
        
        // Remove existing language dictionary if any
        var existingLang = mergedDictionaries.FirstOrDefault(d => d is ResourceInclude ri && ri.Source!.OriginalString.Contains("/Langs/"));
        if (existingLang != null)
        {
            mergedDictionaries.Remove(existingLang);
        }

        // Add new language dictionary
        var newLang = new ResourceInclude(new Uri("avares://AvaVibeTweak/UI/Resources/Langs/en-US.axaml"))
        {
            Source = new Uri($"avares://AvaVibeTweak/UI/Resources/Langs/{locale}.axaml")
        };
        
        mergedDictionaries.Add(newLang);
        
        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }
}
