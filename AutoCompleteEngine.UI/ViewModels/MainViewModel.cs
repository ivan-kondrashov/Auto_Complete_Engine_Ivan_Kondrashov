using AutoCompleteEngine.UI.Models;
using AutoCompleteEngine.UI.Services;
using AutoCompleteEngine.UI.Services.Dialog;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.IO;

namespace AutoCompleteEngine.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ISeries[] _timeSeries;

    [ObservableProperty]
    private ISeries[] _memorySeries;

    [ObservableProperty]
    private string[] _labels;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<BenchmarkInfo> _benchmarks;

    [ObservableProperty]
    private string _fileName;

    [ObservableProperty]
    private Axis[] _xAxes =
    {
        new Axis
        {
            Name = "Words count",
            NamePaint = new SolidColorPaint(SKColor.Parse("#2C3E50")),
            NameTextSize = 14,
            LabelsPaint = new SolidColorPaint(SKColor.Parse("#34495E")),
            SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#ECF0F1")) { StrokeThickness = 1 },
            Labels = null
        }
    };

    [ObservableProperty]
    private Axis[] _yTimeAxes =
    {
        new Axis
        {
            Name = "Time (ns)",
            NamePaint = new SolidColorPaint(SKColor.Parse("#2C3E50")),
            NameTextSize = 14,
            LabelsPaint = new SolidColorPaint(SKColor.Parse("#34495E")),
            SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#ECF0F1")) { StrokeThickness = 1 },
            Labeler = (value) => value.ToString("F0") + " ns"
        }
    };

    [ObservableProperty]
    private Axis[] _yMemoryAxes =
    {
        new Axis
        {
            Name = "Memory (B)",
            NamePaint = new SolidColorPaint(SKColor.Parse("#2C3E50")),
            NameTextSize = 14,
            LabelsPaint = new SolidColorPaint(SKColor.Parse("#34495E")),
            SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#ECF0F1")) { StrokeThickness = 1 },
            Labeler = (value) => value.ToString("F2") + " B"
        }
    };

    private readonly ICsvService _csvService;
    private readonly IDialogService _dialogService;
    private readonly Dictionary<string, string> _methodColors = new();
    private int _colorIndex = 0;

    private readonly string[] _availableColors = new string[]
    {
        "#4285F4", "#EA4335", "#34A853", "#FBBC05", "#673AB7",
        "#3F51B5", "#E91E63", "#009688", "#FF9800", "#795548",
        "#00BCD4", "#8BC34A", "#FFC107", "#9C27B0", "#FF5722",
        "#607D8B", "#CDDC39", "#03A9F4", "#E91E63", "#4CAF50"
    };

    public IAsyncRelayCommand LoadDataCommand { get; }

    public MainViewModel(ICsvService csvService, IDialogService dialogService)
    {
        LoadDataCommand = new AsyncRelayCommand(LoadCsvFile);
        _csvService = csvService;
        _dialogService = dialogService;
        TimeSeries = Array.Empty<ISeries>();
        MemorySeries = Array.Empty<ISeries>();
    }

    private async Task LoadCsvFile()
    {
        FileName = _dialogService.OpenCsvDialog();

        if (!string.IsNullOrWhiteSpace(FileName))
        {
            IsLoading = true;
            try
            {
                var results = await _csvService.LoadBenchmarkDataAsync(FileName);

                Labels = results.Select(r => r.Method).ToArray();
                Benchmarks = new ObservableCollection<BenchmarkInfo>(results);

                UpdateCharts();
            }
            catch (FileNotFoundException ex)
            {
                _dialogService.ShowErrorMessage(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }
        else
        {
            _dialogService.ShowMessage("Selected file path is empty!");
        }
    }

    private void UpdateCharts()
    {
        if (!Benchmarks.Any())
        {
            return;
        }

        _methodColors.Clear();
        _colorIndex = 0;

        var groups = Benchmarks
            .GroupBy(x => x.WordsCount)
            .OrderBy(g => g.Key)
            .ToList();

        var uniqueWordsCounts = groups.Select(g => g.Key).ToList();
        XAxes[0].Labels = uniqueWordsCounts.Select(x => x.ToString()).ToList();

        UpdateTimeChart(groups, uniqueWordsCounts);
        UpdateMemoryChart(groups, uniqueWordsCounts);
    }

    private void UpdateTimeChart(List<IGrouping<int, BenchmarkInfo>> groups, List<int> uniqueWordsCounts)
    {
        var series = new List<ISeries>();

        var methodGroups = groups
            .SelectMany(g => g)
            .GroupBy(x => x.Method)
            .OrderBy(x => x.Key);

        foreach (var methodGroup in methodGroups)
        {
            var orderedPoints = methodGroup
                .OrderBy(x => x.WordsCount)
                .ToList();

            var color = GetColor(methodGroup.Key);

            var lineSeries = new LineSeries<ObservablePoint>
            {
                Name = methodGroup.Key,
                Values = orderedPoints.Select(x => new ObservablePoint(uniqueWordsCounts.IndexOf(x.WordsCount), x.TimeNs)).ToArray(),
                Fill = null,
                GeometrySize = 10,
                Stroke = new SolidColorPaint(SKColor.Parse(color))
                {
                    StrokeThickness = 3
                },
                GeometryStroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 },
                GeometryFill = new SolidColorPaint(SKColor.Parse(color)),
                XToolTipLabelFormatter = (point) =>
                    $"Words: {uniqueWordsCounts[(int)point.Coordinate.SecondaryValue]}",
                YToolTipLabelFormatter = (point) =>
                    $"Time: {point.Coordinate.PrimaryValue:F1} ns",
                LineSmoothness = 0.5,
                IsHoverable = true,
                DataPadding = new LiveChartsCore.Drawing.LvcPoint(1, 1)
            };

            series.Add(lineSeries);
        }

        TimeSeries = series.ToArray();
    }

    private void UpdateMemoryChart(List<IGrouping<int, BenchmarkInfo>> groups, List<int> uniqueWordsCounts)
    {
        var series = new List<ISeries>();

        var methodGroups = groups
            .SelectMany(g => g)
            .GroupBy(x => x.Method)
            .OrderBy(x => x.Key);

        foreach (var methodGroup in methodGroups)
        {
            var orderedPoints = methodGroup
                .OrderBy(x => x.WordsCount)
                .ToList();

            var color = GetColor(methodGroup.Key);

            var columnSeries = new ColumnSeries<ObservablePoint>
            {
                Name = methodGroup.Key,
                Values = orderedPoints.Select(x => new ObservablePoint(uniqueWordsCounts.IndexOf(x.WordsCount), x.MemoryBytes)).ToArray(),
                Stroke = null,
                Fill = new SolidColorPaint(SKColor.Parse(color)),
                XToolTipLabelFormatter = (point) =>
                    $"Words: {uniqueWordsCounts[(int)point.Coordinate.SecondaryValue]}",
                YToolTipLabelFormatter = (point) =>
                    $"Memory: {point.Coordinate.PrimaryValue / 1024:F2} KB",
                IsHoverable = true,
                MaxBarWidth = 50,
                Padding = 1
            };

            series.Add(columnSeries);
        }

        MemorySeries = series.ToArray();
    }

    private string GetColor(string method)
    {
        if (_methodColors.TryGetValue(method, out var existingColor))
        {
            return existingColor;
        }

        var color = _availableColors[_colorIndex % _availableColors.Length];

        _methodColors[method] = color;

        _colorIndex++;

        return color;
    }
}
