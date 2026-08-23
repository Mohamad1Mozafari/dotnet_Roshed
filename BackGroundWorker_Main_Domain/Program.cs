using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/cdx_file_extractor", () =>
{
    List<string> listOfUrls = new List<string>();
    string inputFile = "cdx-00003";
    string outputFile = "com_fa_urls.json";
    int limit = 10000000;
    int count = 0;

    if (!File.Exists(inputFile))
    {
        return Results.NotFound($"Input file '{inputFile}' not found.");
    }

    using var reader = new StreamReader(inputFile);
    using var writer = new StreamWriter(outputFile);

    string? line;
    while (count < limit && (line = reader.ReadLine()) != null)
    {
        var parts = line.Split(' ', 3);
        if (parts.Length < 3) continue;

        string jsonPart = parts[2];

        try
        {
            using var doc = JsonDocument.Parse(jsonPart);
            var root = doc.RootElement;

            // --- URL check ---
            if (!root.TryGetProperty("url", out var urlProp)) continue;
            string? url = urlProp.GetString();

            if (string.IsNullOrEmpty(url)) continue;
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) continue;

            string host = uri.Host;

            if (IPAddress.TryParse(host, out _)) continue;
            if (!host.EndsWith(".com", StringComparison.OrdinalIgnoreCase)) continue;

            // --- Language check ---
            if (!root.TryGetProperty("languages", out var langProp)) continue;

            string? languages = langProp.GetString();
            if (string.IsNullOrEmpty(languages)) continue;

            if (!IsPersian(languages)) continue;

            // --- Write to file & collect response data ---
            writer.WriteLine(jsonPart);
            listOfUrls.Add(url);
            count++;
        }
        catch (JsonException)
        {
            continue;
        }
    }

    return Results.Ok(new
    {
        TotalExtracted = count,
        OutputFile = outputFile,
        ExtractedUrls = listOfUrls
    });
})
.WithName("ExtractCdxUrls")
.WithOpenApi();

app.Run();

// --- Helper Method ---
static bool IsPersian(string languages)
{
    var parts = languages.Split(',', StringSplitOptions.RemoveEmptyEntries);

    foreach (var lang in parts)
    {
        var l = lang.Trim().ToLower();
        if (l == "fas" || l == "fa" || l == "per")
            return true;
    }

    return false;
}