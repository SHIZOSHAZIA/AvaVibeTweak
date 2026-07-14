using Avalonia.Controls;
using AvaVibeTweak.Patching;
using FluentAssertions;
using Xunit;

namespace AvaVibeTweak.Tests.Unit;

public class PatchGeneratorTests
{
    [Fact]
    public void RecordChange_WithValidTarget_RecordsPropertyChange()
    {
        // Arrange
        var target = new Border { Name = "TestBorder" };
        var property = "Margin";
        var value = "10,20,30,40";

        // Act
        PatchGenerator.RecordChange(target, property, value);

        // Assert
        var patches = PatchGenerator.GetPatches();
        patches.Should().ContainKey("TestBorder");
        patches["TestBorder"].Should().ContainKey("Margin");
        patches["TestBorder"]["Margin"].Should().Be("10,20,30,40");
    }

    [Fact]
    public void RecordChange_WithNullName_DoesNotRecord()
    {
        // Arrange
        var target = new Border(); // Name is null
        var property = "Padding";
        var value = "5";
        var initialCount = PatchGenerator.GetPatches().Count;

        // Act
        PatchGenerator.RecordChange(target, property, value);

        // Assert
        PatchGenerator.GetPatches().Count.Should().Be(initialCount);
    }
}
