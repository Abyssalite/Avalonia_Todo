using System;
using System.Globalization;
using Avalonia.Data.Converters;
using System.Collections.Generic;

namespace App1.Converters;

public class BoolToTextConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count >= 1 && values[0] is bool value)
        {
            return parameter switch
            {
                "Title" => value ? "Archived" : "Lists",
                "Archive" => value ? "Archive" : "Restore",
                _ => "Unknown"
            };
        }
        return "N/A";
    }
}