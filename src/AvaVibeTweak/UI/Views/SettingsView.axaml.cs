using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaVibeTweak.UI;
using System.Linq;

namespace AvaVibeTweak.UI.Views;

public partial class SettingsView : Window
{
    private bool _isInitializing = true;
    private TextBox? _toggleKeyBox;
    private TextBox? _saveKeyBox;
    private ComboBox? _langCombo;

    public SettingsView()
    {
        InitializeComponent();
        
        _langCombo = this.FindControl<ComboBox>("LanguageCombo");
        _toggleKeyBox = this.FindControl<TextBox>("ToggleKeyBox");
        _saveKeyBox = this.FindControl<TextBox>("SaveKeyBox");

        UpdateUI();

        _isInitializing = false;
    }

    private void UpdateUI()
    {
        if (_langCombo != null)
        {
            var selectedItem = _langCombo.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => (string)i.Tag! == SettingsManager.Instance.Language);
            if (selectedItem != null)
                _langCombo.SelectedItem = selectedItem;
        }

        if (_toggleKeyBox != null)
            _toggleKeyBox.Text = SettingsManager.Instance.ToggleKey.ToString();

        if (_saveKeyBox != null)
            _saveKeyBox.Text = FormatHotkey(SettingsManager.Instance.SavePatchModifiers, SettingsManager.Instance.SavePatchKey);
    }

    private string FormatHotkey(KeyModifiers mods, Key key)
    {
        var result = "";
        if (mods.HasFlag(KeyModifiers.Control)) result += "Ctrl + ";
        if (mods.HasFlag(KeyModifiers.Shift)) result += "Shift + ";
        if (mods.HasFlag(KeyModifiers.Alt)) result += "Alt + ";
        result += key.ToString();
        return result;
    }

    private void OnTitlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnLanguageChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isInitializing) return;
        if (sender is ComboBox cb && cb.SelectedItem is ComboBoxItem item && item.Tag is string lang)
        {
            SettingsManager.Instance.Language = lang;
            SettingsManager.Save();
            LocalizationManager.ChangeLanguage(lang);
        }
    }

    private void OnToggleKeyDown(object? sender, KeyEventArgs e)
    {
        if (IsModifierKey(e.Key)) return; // Wait for an actual key
        
        SettingsManager.Instance.ToggleKey = e.Key;
        SettingsManager.Save();
        
        if (_toggleKeyBox != null)
            _toggleKeyBox.Text = e.Key.ToString();
            
        e.Handled = true;
    }

    private void OnSaveKeyDown(object? sender, KeyEventArgs e)
    {
        if (IsModifierKey(e.Key)) return; // Wait for an actual key
        
        SettingsManager.Instance.SavePatchKey = e.Key;
        SettingsManager.Instance.SavePatchModifiers = e.KeyModifiers;
        SettingsManager.Save();
        
        if (_saveKeyBox != null)
            _saveKeyBox.Text = FormatHotkey(e.KeyModifiers, e.Key);
            
        e.Handled = true;
    }

    private bool IsModifierKey(Key key)
    {
        return key == Key.LeftCtrl || key == Key.RightCtrl || 
               key == Key.LeftShift || key == Key.RightShift || 
               key == Key.LeftAlt || key == Key.RightAlt || 
               key == Key.LWin || key == Key.RWin;
    }

    private void OnResetClicked(object? sender, RoutedEventArgs e)
    {
        SettingsManager.ResetToDefaults();
        
        _isInitializing = true;
        UpdateUI();
        _isInitializing = false;
        
        LocalizationManager.ChangeLanguage(SettingsManager.Instance.Language);
    }
}
