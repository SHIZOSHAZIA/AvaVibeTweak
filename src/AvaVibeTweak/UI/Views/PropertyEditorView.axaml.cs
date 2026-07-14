using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaVibeTweak.UI.ViewModels;

namespace AvaVibeTweak.UI.Views;

public partial class PropertyEditorView : Window
{
    public PropertyEditorView()
    {
        InitializeComponent();
    }

    private void OnTitlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnSettingsClicked(object? sender, RoutedEventArgs e)
    {
        var settingsView = new SettingsView();
        var pos = this.Position;
        settingsView.Position = new Avalonia.PixelPoint(pos.X - 320, pos.Y);
        settingsView.Show(this);
    }
}
