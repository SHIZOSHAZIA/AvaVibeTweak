using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TestApp.Models;

namespace TestApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _greeting = "Welcome to AvaVibeTweak Dashboard!";

    public ObservableCollection<DashboardItem> Items { get; } =
    [
        new DashboardItem { Title = "Total Revenue", Value = "$45,231.89", Trend = "+20.1%", TrendColor = "#10b981" },
        new DashboardItem { Title = "Active Users", Value = "2,350", Trend = "+15.2%", TrendColor = "#10b981" },
        new DashboardItem { Title = "Bounce Rate", Value = "12.5%", Trend = "-2.3%", TrendColor = "#f43f5e" },
        new DashboardItem { Title = "New Signups", Value = "894", Trend = "+5.4%", TrendColor = "#10b981" }
    ];

    public ObservableCollection<string> Transactions { get; } =
    [
        "Payment from John Doe - $120.00",
        "Refund to Jane Smith - $45.00",
        "Subscription Renewed - $15.00",
        "Payment from Acme Corp - $1,500.00"
    ];
}
