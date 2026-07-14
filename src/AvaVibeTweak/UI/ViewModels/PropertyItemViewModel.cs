using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaVibeTweak.Patching;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaVibeTweak.UI.ViewModels;

public abstract partial class PropertyItemViewModel : ObservableObject
{
    public string Name { get; }
    public Control Target { get; }
    public PropertyInfo PropertyInfo { get; }

    public string ToolTipText => LocalizationManager.GetString($"Tooltip_{Name}");

    public PropertyItemViewModel(string name, Control target, PropertyInfo propertyInfo)
    {
        Name = name;
        Target = target;
        PropertyInfo = propertyInfo;
        LocalizationManager.LanguageChanged += (s, e) => OnPropertyChanged(nameof(ToolTipText));
    }

    protected void UpdateTarget(object? value)
    {
        if (value is not null && PropertyInfo.CanWrite)
        {
            try
            {
                PropertyInfo.SetValue(Target, value);
                PatchGenerator.RecordChange(Target, Name, value);
            }
            catch { }
        }
    }
}

public partial class StringPropertyViewModel : PropertyItemViewModel
{
    [ObservableProperty]
    private string _valueText = "";

    public bool IsSizeProperty => Name == "Width" || Name == "Height";
    public bool IsMaxSizeProperty => Name == "MaxWidth" || Name == "MaxHeight";

    [RelayCommand]
    private void SetToAuto() => ValueText = "Auto";

    [RelayCommand]
    private void SetToInfinity() => ValueText = "Infinity";

    public StringPropertyViewModel(string name, Control target, PropertyInfo propertyInfo) 
        : base(name, target, propertyInfo)
    {
        var val = propertyInfo.GetValue(target);
        
        if (val is double d)
        {
            if (double.IsNaN(d))
                _valueText = "Auto";
            else if (double.IsPositiveInfinity(d))
                _valueText = "Infinity";
            else
                _valueText = d.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        else
        {
            _valueText = val?.ToString() ?? "";
        }
    }

    partial void OnValueTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (PropertyInfo.PropertyType == typeof(double) || PropertyInfo.PropertyType == typeof(Thickness))
            {
                ValueText = "0";
            }
            return;
        }

        try
        {
            object? parsedVal = null;
            if (PropertyInfo.PropertyType == typeof(Thickness))
                parsedVal = Thickness.Parse(value);
            else if (PropertyInfo.PropertyType == typeof(double))
            {
                if (value.Equals("Auto", StringComparison.OrdinalIgnoreCase))
                    parsedVal = double.NaN;
                else if (value.Equals("Infinity", StringComparison.OrdinalIgnoreCase))
                    parsedVal = double.PositiveInfinity;
                else
                    parsedVal = double.Parse(value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (PropertyInfo.PropertyType == typeof(IBrush))
                parsedVal = Brush.Parse(value);
            else if (PropertyInfo.PropertyType == typeof(string))
                parsedVal = value;

            if (parsedVal is not null)
                UpdateTarget(parsedVal);
        }
        catch { }
    }
}

public partial class ThicknessPropertyViewModel : PropertyItemViewModel
{
    [ObservableProperty] private string _left = "0";
    [ObservableProperty] private string _top = "0";
    [ObservableProperty] private string _right = "0";
    [ObservableProperty] private string _bottom = "0";
    
    private bool _isUpdating;

    partial void OnLeftChanged(string value) { if (string.IsNullOrWhiteSpace(value)) Left = "0"; }
    partial void OnTopChanged(string value) { if (string.IsNullOrWhiteSpace(value)) Top = "0"; }
    partial void OnRightChanged(string value) { if (string.IsNullOrWhiteSpace(value)) Right = "0"; }
    partial void OnBottomChanged(string value) { if (string.IsNullOrWhiteSpace(value)) Bottom = "0"; }

    public ThicknessPropertyViewModel(string name, Control target, PropertyInfo propertyInfo) 
        : base(name, target, propertyInfo)
    {
        if (propertyInfo.GetValue(target) is Thickness t)
        {
            _isUpdating = true;
            Left = t.Left.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Top = t.Top.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Right = t.Right.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Bottom = t.Bottom.ToString(System.Globalization.CultureInfo.InvariantCulture);
            _isUpdating = false;
        }
    }

    protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (_isUpdating) return;
        
        if (e.PropertyName is nameof(Left) or nameof(Top) or nameof(Right) or nameof(Bottom))
        {
            if (double.TryParse(Left.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var l) &&
                double.TryParse(Top.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var t) &&
                double.TryParse(Right.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var r) &&
                double.TryParse(Bottom.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var b))
            {
                UpdateTarget(new Thickness(l, t, r, b));
            }
        }
    }
}

public partial class BrushPropertyViewModel : PropertyItemViewModel
{
    [ObservableProperty]
    private Color _selectedColor;

    private bool _isUpdating;

    public BrushPropertyViewModel(string name, Control target, PropertyInfo propertyInfo) 
        : base(name, target, propertyInfo)
    {
        var val = propertyInfo.GetValue(target);
        if (val is ISolidColorBrush solidBrush)
        {
            _isUpdating = true;
            SelectedColor = solidBrush.Color;
            _isUpdating = false;
        }
    }

    partial void OnSelectedColorChanged(Color value)
    {
        if (_isUpdating) return;
        UpdateTarget(new SolidColorBrush(value));
    }
}

public partial class EnumPropertyViewModel : PropertyItemViewModel
{
    public Array EnumValues { get; }
    
    [ObservableProperty]
    private object? _selectedValue;

    private bool _isUpdating;

    public EnumPropertyViewModel(string name, Control target, PropertyInfo propertyInfo) 
        : base(name, target, propertyInfo)
    {
        EnumValues = Enum.GetValues(propertyInfo.PropertyType);
        
        _isUpdating = true;
        SelectedValue = propertyInfo.GetValue(target);
        _isUpdating = false;
    }

    partial void OnSelectedValueChanged(object? value)
    {
        if (_isUpdating || value is null) return;
        UpdateTarget(value);
    }
}
