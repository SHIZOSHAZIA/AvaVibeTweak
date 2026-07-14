using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaVibeTweak.UI.ViewModels;

public partial class PropertyEditorViewModel : ObservableObject
{
    [ObservableProperty]
    private string _targetName = "";

    public ObservableCollection<PropertyItemViewModel> LayoutProperties { get; } = new();
    public ObservableCollection<PropertyItemViewModel> AppearanceProperties { get; } = new();
    public ObservableCollection<PropertyItemViewModel> TextProperties { get; } = new();
    public ObservableCollection<PropertyItemViewModel> MiscProperties { get; } = new();

    public PropertyEditorViewModel(Control target)
    {
        TargetName = target.Name ?? target.GetType().Name;
        LoadProperties(target);
    }

    private void LoadProperties(Control target)
    {
        var type = target.GetType();
        var props = type.GetProperties().Where(p => p.CanWrite && p.CanRead).ToList();

        // Allowed properties to edit currently
        var allowedNames = new[] { 
            "Margin", "Padding", "Width", "Height", "MinWidth", "MinHeight", "MaxWidth", "MaxHeight",
            "Background", "Foreground", "BorderBrush", "BorderThickness", "CornerRadius", "Opacity",
            "Text", "FontSize", "FontWeight", "FontFamily",
            "HorizontalAlignment", "VerticalAlignment", "TextWrapping", "TextAlignment"
        };

        foreach (var p in props)
        {
            if (!allowedNames.Contains(p.Name)) continue;

            PropertyItemViewModel vm;
            if (p.PropertyType == typeof(Thickness))
            {
                vm = new ThicknessPropertyViewModel(p.Name, target, p);
            }
            else if (typeof(Avalonia.Media.IBrush).IsAssignableFrom(p.PropertyType))
            {
                vm = new BrushPropertyViewModel(p.Name, target, p);
            }
            else if (p.PropertyType.IsEnum)
            {
                vm = new EnumPropertyViewModel(p.Name, target, p);
            }
            else
            {
                vm = new StringPropertyViewModel(p.Name, target, p);
            }

            if (new[] { "Margin", "Padding", "Width", "Height", "MinWidth", "MinHeight", "MaxWidth", "MaxHeight", "HorizontalAlignment", "VerticalAlignment" }.Contains(p.Name))
                LayoutProperties.Add(vm);
            else if (new[] { "Background", "Foreground", "BorderBrush", "BorderThickness", "CornerRadius", "Opacity" }.Contains(p.Name))
                AppearanceProperties.Add(vm);
            else if (new[] { "Text", "FontSize", "FontWeight", "FontFamily", "TextWrapping", "TextAlignment" }.Contains(p.Name))
                TextProperties.Add(vm);
            else
                MiscProperties.Add(vm);
        }
    }
}
