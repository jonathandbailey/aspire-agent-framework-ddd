﻿using System.Text.Json;
using System.Text.RegularExpressions;
using Domain;

namespace Application;

public static class JsonOutputParser
{
    private static readonly Regex JsonBlockRegex = new(@"```json\s*(.*?)\s*```", RegexOptions.Singleline | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex JsonBlockRegexForRemoval = new(@"```json\s*.*?\s*```", RegexOptions.Singleline | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Checks if the provided content contains a JSON block enclosed in ```json tags.
    /// </summary>
    /// <param name="content">The input string to check for a JSON block.</param>
    /// <returns>True if a JSON block is found; otherwise, false.</returns>
    public static bool HasJson(string content)
    {
        Verify.NotNullOrWhiteSpace(content);

        var match = JsonBlockRegex.Match(content);

        return match.Success;
    }

    /// <summary>
    /// Parses the JSON block enclosed in ```json tags from the provided content.
    /// </summary>
    /// <typeparam name="T">The type to which the JSON content should be deserialized.</typeparam>
    /// <param name="content">The input string containing the JSON block.</param>
    /// <returns>An object of type T deserialized from the JSON block.</returns>
    /// <exception cref="FormatException">Thrown when no JSON block is found in the content.</exception>
    /// <exception cref="InvalidOperationException">Thrown when deserialization results in a null object.</exception>
    public static T Parse<T>(string content)
    {
        Verify.NotNullOrWhiteSpace(content);

        var match = JsonBlockRegex.Match(content);

        if (!match.Success)
            throw new FormatException("No JSON block found between ```json tags.");

        var json = match.Groups[1].Value;

        var output = JsonSerializer.Deserialize<T>(json, JsonSerializerOptions)
            ?? throw new InvalidOperationException("Deserialization resulted in a null object.");

        return output;
    }

    /// <summary>
    /// Removes all JSON blocks enclosed in ```json tags from the provided content.
    /// </summary>
    /// <param name="content">The input string from which JSON blocks should be removed.</param>
    /// <returns>The content string with JSON blocks removed.</returns>
    public static string Remove(string content)
    {
        Verify.NotNullOrWhiteSpace(content);

        var result = JsonBlockRegexForRemoval.Replace(content, string.Empty);
        return result;
    }
}