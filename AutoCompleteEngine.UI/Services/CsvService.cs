using AutoCompleteEngine.UI.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace AutoCompleteEngine.UI.Services;

public class CsvService : ICsvService
{
    public async Task<List<BenchmarkInfo>> LoadBenchmarkDataAsync(string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null,
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true,
            Delimiter = ";"
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        var records = await csv.GetRecordsAsync<BenchmarkInfo>().ToListAsync();

        foreach (var record in records)
        {
            record.TimeNs = ParseBenchmarkNumber(record.MeanRaw);
            record.MemoryBytes = ParseMemoryBytes(record.AllocatedRaw);
        }

        return records;
    }

    private double ParseBenchmarkNumber(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return 0;
        }

        var match = Regex.Match(input, @"^([\d\.,]+)");
        if (!match.Success)
        {
            return 0;
        }

        var numberStr = match.Groups[1].Value
            .Replace(",", "")
            .Replace(" ", "");

        if (double.TryParse(numberStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        else
        {
            return 0;
        }
    }

    private double ParseMemoryBytes(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return 0;
        }

        var match = Regex.Match(input, @"^([\d\.]+)\s*([KMG]?B?)", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            return 0;
        }

        var valueStr = match.Groups[1].Value.Replace(",", ".");
        var unit = match.Groups[2].Value.ToUpper();

        if (!double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            return 0;
        }

        return unit switch
        {
            "KB" => value * 1024,
            "MB" => value * 1024 * 1024,
            "GB" => value * 1024 * 1024 * 1024,
            _ => value
        };
    }
}
