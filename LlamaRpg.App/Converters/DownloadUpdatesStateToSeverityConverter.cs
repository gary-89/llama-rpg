using System;
using LlamaRpg.App.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace LlamaRpg.App.Converters;

internal sealed class DownloadUpdatesStateToSeverityConverter : IValueConverter
{
    public object Convert(object? value, Type? targetType, object? parameter, string? language)
    {
        return value is not DownloadUpdatesState state
            ? InfoBarSeverity.Informational
            : Convert(state);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }

    public static InfoBarSeverity Convert(DownloadUpdatesState state)
    {
        return state switch
        {
            DownloadUpdatesState.Succeeded => InfoBarSeverity.Success,
            DownloadUpdatesState.Failed => InfoBarSeverity.Warning,
            _ => InfoBarSeverity.Informational
        };
    }
}
