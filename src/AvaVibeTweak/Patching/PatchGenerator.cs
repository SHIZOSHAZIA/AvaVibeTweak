using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AvaVibeTweak.Patching;

public static class PatchGenerator
{
    private static readonly ConcurrentDictionary<string, Dictionary<string, string>> _patches = new();

    public static IReadOnlyDictionary<string, Dictionary<string, string>> GetPatches() => _patches;

    public static void RecordChange(Control target, string property, object? value)
    {
        var key = GeneratePath(target);
        if (string.IsNullOrEmpty(key)) return;

        var elementPatches = _patches.GetOrAdd(key, _ => []);
        elementPatches[property] = value?.ToString() ?? "null";
    }

    private static string GeneratePath(Visual target)
    {
        var pathParts = new List<string>();
        Visual? current = target;

        while (current is not null)
        {
            var typeName = current.GetType().Name;
            var part = typeName;

            if (current is Control c && !string.IsNullOrEmpty(c.Name))
            {
                part += $"#{c.Name}";
            }
            else
            {
                var parent = current.GetVisualParent();
                if (parent is not null)
                {
                    int index = 0;
                    foreach (var sibling in parent.GetVisualChildren())
                    {
                        if (sibling == current) break;
                        if (sibling.GetType() == current.GetType()) index++;
                    }
                    part += $"[{index}]";
                }
            }

            pathParts.Add(part);
            current = current.GetVisualParent();
        }

        pathParts.Reverse();
        return string.Join(" > ", pathParts);
    }

    public static async ValueTask SavePatchAsync()
    {
        if (_patches.IsEmpty) return;

        var projectRoot = FindProjectRoot(AppContext.BaseDirectory);
        if (projectRoot is null)
        {
            Console.WriteLine("[AvaVibeTweak] Could not find project root to save patch.");
            return;
        }

        var path = Path.Combine(projectRoot, "vibe_patch.json");
        var json = JsonSerializer.Serialize(_patches, new JsonSerializerOptions { WriteIndented = true });
        
        await File.WriteAllTextAsync(path, json);
        Console.WriteLine($"[AvaVibeTweak] Patch saved to: {path}");
    }

    private static string? FindProjectRoot(string startPath)
    {
        var dir = new DirectoryInfo(startPath);
        while (dir is not null)
        {
            if (dir.GetFiles("*.sln").Length > 0 || dir.GetFiles("*.csproj").Length > 0)
            {
                return dir.FullName;
            }
            dir = dir.Parent;
        }
        return null;
    }
}
