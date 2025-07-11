using System;
using System.Globalization;
using Avalonia.Data.Converters;
using System.Collections.Generic;

namespace App1.Converters;

public class ArchiveEditTextConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count >= 1 && values[0] is bool value)
        {
            return parameter switch
            {
                "Edit" => value ? "Edit" : "Restore",
                "Title" => value ? "Archived" : "Lists",
                _ => "Unknown"
            };
        }

        return "N/A";
    }
}