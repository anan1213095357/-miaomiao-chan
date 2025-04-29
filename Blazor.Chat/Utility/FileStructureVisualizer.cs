namespace Blazor.Chat.Utility;

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class FileStructureVisualizer
{
    private const string INDENT = "    ";
    private const string BRANCH = "├── ";
    private const string LAST_BRANCH = "└── ";
    private const string VERTICAL = "│   ";
    private const string EMPTY_SPACE = "    ";

    public static string GetStructureString(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }

        var builder = new StringBuilder();
        builder.AppendLine(Path.GetFileName(path) + "/");
        VisualizeDirectory(path, "", true, builder);

        return builder.ToString();
    }

    private static void VisualizeDirectory(string path, string indent, bool isRoot, StringBuilder builder)
    {
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);

        var items = new List<string>();
        items.AddRange(directories);
        items.AddRange(files);

        for (int i = 0; i < items.Count; i++)
        {
            bool isLast = (i == items.Count - 1);
            string item = items[i];
            bool isDirectory = Directory.Exists(item);

            if (!isRoot)
            {
                builder.Append(indent);
            }

            builder.Append(isLast ? LAST_BRANCH : BRANCH);

            if (isDirectory)
            {
                builder.AppendLine(Path.GetFileName(item) + "/");
                string newIndent = indent + (isLast ? EMPTY_SPACE : VERTICAL);
                VisualizeDirectory(item, newIndent, false, builder);
            }
            else
            {
                builder.AppendLine(Path.GetFileName(item));
            }
        }
    }

    public static string GetAIFriendlyString(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }

        var builder = new StringBuilder();
        builder.AppendLine($"Root Directory: {Path.GetFileName(path)}");
        GetAIFriendlyStructure(path, 1, builder);

        return builder.ToString();
    }

    private static void GetAIFriendlyStructure(string path, int level, StringBuilder builder)
    {
        var indent = new string(' ', level * 2);

        // Process directories
        foreach (var dir in Directory.GetDirectories(path))
        {
            builder.AppendLine($"{indent}Directory: {Path.GetFileName(dir)}");
            GetAIFriendlyStructure(dir, level + 1, builder);
        }

        // Process files
        var files = Directory.GetFiles(path);
        if (files.Length > 0)
        {
            builder.AppendLine($"{indent}Files:");
            foreach (var file in files)
            {
                builder.AppendLine($"{indent}  - {Path.GetFileName(file)}");
            }
        }
    }
}