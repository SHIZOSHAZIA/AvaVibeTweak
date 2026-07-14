using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using AvaVibeTweak.Patching;

namespace AvaVibeTweak.UI;

public class PropertiesPanelPopup
{
    public Popup? Popup { get; private set; }
    public Border Container => _container;
    private Control? _target;
    private readonly Border _container;
    private readonly StackPanel _panel;

    public bool IsPopupElement(Visual? v)
    {
        var current = v;
        while (current is not null)
        {
            if (current == Popup || current == _container || current.GetType().Name.Contains("PopupRoot") || current.GetType().Name.Contains("PopupHost"))
                return true;
            current = current.GetVisualParent();
        }
        return false;
    }

    public PropertiesPanelPopup()
    {
        _panel = new StackPanel { Spacing = 12 };
        _container = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#0F172A")), // Slate-900
            BorderBrush = new SolidColorBrush(Color.Parse("#1E293B")), // Slate-800
            BorderThickness = new Thickness(1),
            Padding = new Thickness(24),
            CornerRadius = new CornerRadius(16),
            Child = _panel,
            Width = 280,
            BoxShadow = new BoxShadows(new BoxShadow { Blur = 24, Color = Color.Parse("#40000000"), OffsetX = 0, OffsetY = 8 })
        };
    }

    public void Attach(Control target)
    {
        Detach();
        _target = target;
        BuildUI();

        Popup = new Popup
        {
            PlacementTarget = target,
            Placement = PlacementMode.RightEdgeAlignedTop,
            Child = _container,
            IsLightDismissEnabled = false
        };

        var layer = AdornerLayer.GetAdornerLayer(target);
        if (layer is not null)
        {
            layer.Children.Add(Popup);
        }
        Popup.IsOpen = true;
    }

    public void Detach()
    {
        if (Popup is not null)
        {
            Popup.IsOpen = false;
            Popup.Child = null; // ОТВЯЗЫВАЕМ КОНТЕЙНЕР ОТ СТАРОГО ПОПАПА!
            
            var layer = _target is not null ? AdornerLayer.GetAdornerLayer(_target) : null;
            if (layer is not null && layer.Children.Contains(Popup))
            {
                layer.Children.Remove(Popup);
            }
            Popup = null;
        }
        _target = null;
    }

    private void BuildUI()
    {
        _panel.Children.Clear();
        if (_target is null) return;
        
        var header = new TextBlock 
        { 
            Text = $"Edit: {_target.Name ?? _target.GetType().Name}", 
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
            FontSize = 14,
            Margin = new Thickness(0, 0, 0, 12),
            LetterSpacing = 0.5
        };
        _panel.Children.Add(header);

        AddEditorForProperty("Margin");
        AddEditorForProperty("Padding");
        AddEditorForProperty("FontSize");
        AddEditorForProperty("Width");
        AddEditorForProperty("Height");
        AddEditorForProperty("Background");
    }

    private void AddEditorForProperty(string propertyName)
    {
        if (_target is null) return;
        var propInfo = _target.GetType().GetProperty(propertyName);
        if (propInfo is null || !propInfo.CanWrite) return;

        var val = propInfo.GetValue(_target);
        var strVal = val?.ToString() ?? "";
        if (val is double d && double.IsNaN(d)) strVal = "";

        var sp = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12, Margin = new Thickness(0, 4) };
        sp.Children.Add(new TextBlock 
        { 
            Text = propertyName, 
            Width = 80, 
            VerticalAlignment = VerticalAlignment.Center, 
            Foreground = new SolidColorBrush(Color.Parse("#94A3B8")), // Slate-400
            FontSize = 12,
            FontWeight = FontWeight.Medium
        });
        
            var tb = new TextBox 
            { 
                Text = strVal, 
                Width = 130,
                Background = new SolidColorBrush(Color.Parse("#1E293B")), // Slate-800
                Foreground = new SolidColorBrush(Color.Parse("#F8FAFC")), // Slate-50
                BorderBrush = new SolidColorBrush(Color.Parse("#334155")), // Slate-700
                MinHeight = 28,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8, 4),
                FontSize = 12
            };

            var originalText = strVal;

            tb.TextChanged += (s, e) => 
            {
                var input = tb.Text ?? "";
                if (string.IsNullOrWhiteSpace(input) || input == originalText) return;

                try 
                {
                    object? parsedVal = null;
                    if (propInfo.PropertyType == typeof(Thickness))
                        parsedVal = Thickness.Parse(input);
                    else if (propInfo.PropertyType == typeof(double))
                        parsedVal = double.Parse(input);
                    else if (propInfo.PropertyType == typeof(IBrush))
                        parsedVal = Brush.Parse(input);

                    if (parsedVal is not null)
                    {
                        propInfo.SetValue(_target, parsedVal);
                        PatchGenerator.RecordChange(_target, propertyName, parsedVal);
                        originalText = input; // Обновляем, чтобы не было повторных срабатываний
                    }
                } 
                catch { }
            };
        
        sp.Children.Add(tb);
        _panel.Children.Add(sp);
    }
}
