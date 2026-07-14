using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Media;
using AvaVibeTweak.Adorners;
using AvaVibeTweak.Patching;
using AvaVibeTweak.UI.ViewModels;
using AvaVibeTweak.UI.Views;

namespace AvaVibeTweak;

public static class VibeOverlayManager
{
    public static bool IsDesignMode { get; private set; }
    private static HighlightAdorner? _hoverAdorner;
    private static Control? _currentHoveredControl;

    private static PropertyEditorView? _propertiesPanel;
    private static Window? _indicatorWindow;
    private static Border? _indicatorBorder;
    private static TextBlock? _indicatorText;

    public static void Initialize()
    {
        InputElement.KeyDownEvent.AddClassHandler<TopLevel>(OnKeyDown, RoutingStrategies.Tunnel);
        InputElement.PointerMovedEvent.AddClassHandler<TopLevel>(OnPointerMoved, RoutingStrategies.Tunnel);
        InputElement.PointerPressedEvent.AddClassHandler<TopLevel>(OnPointerPressed, RoutingStrategies.Tunnel);
    }

    private static async void OnKeyDown(TopLevel sender, KeyEventArgs e)
    {
        if (e.Key == SettingsManager.Instance.ToggleKey)
        {
            await ToggleDesignModeAsync(sender);
            e.Handled = true;
        }
        else if (e.Key == SettingsManager.Instance.SavePatchKey && e.KeyModifiers.HasFlag(SettingsManager.Instance.SavePatchModifiers))
        {
            if (IsDesignMode)
            {
                await PatchGenerator.SavePatchAsync();
                ShowSaveNotification();
                e.Handled = true;
            }
        }
    }

    private static void OnPointerMoved(TopLevel sender, PointerEventArgs e)
    {
        if (!IsDesignMode) return;

        var sourceVisual = e.Source as Visual;
        if (IsPanelOrPopupElement(sourceVisual)) return;

        var hit = sender.GetVisualsAt(e.GetPosition(sender))
                        .OfType<Control>()
                        .FirstOrDefault(c => c != _hoverAdorner);

        if (hit is not null && hit != _currentHoveredControl)
        {
            _currentHoveredControl = hit;
            _hoverAdorner?.Attach(hit);
        }
    }

    private static void OnPointerPressed(TopLevel sender, PointerPressedEventArgs e)
    {
        if (!IsDesignMode) return;

        var sourceVisual = e.Source as Visual;
        if (IsPanelOrPopupElement(sourceVisual)) return;

        var hit = sender.GetVisualsAt(e.GetPosition(sender)).OfType<Control>().FirstOrDefault(c => c != _hoverAdorner);
        if (hit is null) return;

        if (hit != _currentHoveredControl)
        {
            _currentHoveredControl = hit;
            _hoverAdorner?.Attach(hit);
        }

        if (_currentHoveredControl is not null)
        {
            if (_propertiesPanel == null)
            {
                _propertiesPanel = new PropertyEditorView();
                _propertiesPanel.DataContext = new PropertyEditorViewModel(_currentHoveredControl);
                
                var pos = e.GetCurrentPoint(null).Position;
                if (sender is Window w)
                {
                    _propertiesPanel.Position = new PixelPoint(
                        w.Position.X + (int)pos.X + 20, 
                        w.Position.Y + (int)pos.Y);
                }
                
                _propertiesPanel.Closed += (s, ev) => _propertiesPanel = null;
                _propertiesPanel.Show();
            }
            else
            {
                _propertiesPanel.DataContext = new PropertyEditorViewModel(_currentHoveredControl);
            }
            e.Handled = true;
        }
    }

    public static async Task ToggleDesignModeAsync(TopLevel topLevel)
    {
        IsDesignMode = !IsDesignMode;
        Console.WriteLine($"[AvaVibeTweak] Design Mode: {(IsDesignMode ? "ON" : "OFF")}");

        if (IsDesignMode)
        {
            AvaVibeTweak.UI.LocalizationManager.Initialize(SettingsManager.Instance.Language);
            _hoverAdorner ??= new HighlightAdorner();
            
            if (_indicatorWindow is null && topLevel is Window mainWindow)
            {
                _indicatorText = new TextBlock 
                { 
                    Text = "AVA VIBE TWEAK: ON", 
                    Foreground = Brushes.White, 
                    FontWeight = FontWeight.Bold,
                    LetterSpacing = 1.2,
                    FontSize = 13
                };
                
                _indicatorBorder = new Border
                {
                    Background = new SolidColorBrush(Color.Parse("#F43F5E")), // Rose-500 from premium-ui
                    CornerRadius = new CornerRadius(0, 0, 0, 16),
                    Padding = new Thickness(24, 12),
                    BoxShadow = new BoxShadows(new BoxShadow { Blur = 24, Color = Color.Parse("#40F43F5E"), OffsetX = 0, OffsetY = 8 }),
                    Child = _indicatorText
                };

                _indicatorWindow = new Window
                {
                    SystemDecorations = SystemDecorations.None,
                    Background = Brushes.Transparent,
                    Topmost = true,
                    ShowInTaskbar = false,
                    IsHitTestVisible = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Content = _indicatorBorder
                };
                
                var pos = mainWindow.Position;
                _indicatorWindow.Position = new PixelPoint(pos.X + (int)mainWindow.Bounds.Width - 190, pos.Y);
                _indicatorWindow.Show(mainWindow);
            }
        }
        else
        {
            _hoverAdorner?.Detach();
            if (_propertiesPanel != null)
            {
                _propertiesPanel.Close();
                _propertiesPanel = null;
            }
            _indicatorWindow?.Close();
            _indicatorWindow = null;
            _currentHoveredControl = null;
            await PatchGenerator.SavePatchAsync();
        }
    }

    private static bool IsPanelOrPopupElement(Visual? visual)
    {
        if (visual is null) return false;
        
        var root = visual.GetVisualRoot();
        if (root is AvaVibeTweak.UI.Views.PropertyEditorView) return true;
        if (root is AvaVibeTweak.UI.Views.SettingsView) return true;
        if (_indicatorWindow != null && root == _indicatorWindow) return true;

        var current = visual as Avalonia.LogicalTree.ILogical;
        while (current is not null)
        {
            if (current is AvaVibeTweak.UI.Views.PropertyEditorView) return true;
            if (current is AvaVibeTweak.UI.Views.SettingsView) return true;
            if (_indicatorWindow != null && current == _indicatorWindow) return true;
            current = current.LogicalParent;
        }

        return false;
    }

    private static bool IsChildOf(Visual? child, Visual? parent)
    {
        if (parent is null || child is null) return false;
        var current = child;
        while (current is not null)
        {
            if (current == parent) return true;
            current = current.GetVisualParent();
        }
        return false;
    }

    private static async void ShowSaveNotification()
    {
        if (_indicatorBorder is null || _indicatorText is null) return;
        
        var oldBg = _indicatorBorder.Background;
        var oldText = _indicatorText.Text;
        
        _indicatorBorder.Background = new SolidColorBrush(Color.Parse("#10B981")); // Emerald-500
        _indicatorText.Text = "PATCH SAVED!";
        
        await Task.Delay(1500);
        
        _indicatorBorder.Background = oldBg;
        _indicatorText.Text = oldText;
    }
}
