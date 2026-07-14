using Avalonia.Controls;
using TestApp.ViewModels;

namespace TestApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}