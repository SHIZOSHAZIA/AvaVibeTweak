namespace TestApp.Models;

public record DashboardItem
{
    public required string Title { get; init; }
    public required string Value { get; init; }
    public required string Trend { get; init; }
    public required string TrendColor { get; init; }
}
