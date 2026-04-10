using System.ComponentModel;
using System.Globalization;
using Godot;

namespace BaseLib.Config;

public class GodotColorConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) =>
        destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is not string strVal) return base.ConvertFrom(context, culture, value);
        strVal = strVal.Trim('[', ']');
        var parts = strVal.Split(',');

        if (parts.Length == 4)
        {
            if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var r) &&
                float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var g) &&
                float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var b) &&
                float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var a))
            {
                return new Color(r, g, b, a);
            }
        }

        throw new FormatException($"String '{value}' is not in a valid format. Expected format (as string): [r, g, b, a].");
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is Color color)
        {
            return $"[{color.R.ToString(CultureInfo.InvariantCulture)}, " +
                   $"{color.G.ToString(CultureInfo.InvariantCulture)}, " +
                   $"{color.B.ToString(CultureInfo.InvariantCulture)}, " +
                   $"{color.A.ToString(CultureInfo.InvariantCulture)}]";
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}