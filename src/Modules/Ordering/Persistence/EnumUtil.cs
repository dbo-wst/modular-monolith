using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;
using System.Runtime.Serialization;

namespace Acme.Core.Ordering.Persistence;

public static class EnumUtil
{
    public static PropertyBuilder<T> HasEnumConversion<T>(this PropertyBuilder<T> p) where T : struct, Enum =>
        p.HasConversion(e => ToEnumString(e), str => ToEnum<T>(str));

    public static PropertyBuilder<T?> HasNullableEnumConversion<T>(this PropertyBuilder<T?> p) where T : struct, Enum =>
        p.HasConversion(
            e => e != null ? ToEnumString(e.Value) : null,
            str => str != null ? ToEnum<T>(str) : null);


    public static string? ToEnumString<T>(T enumValue)
        where T : notnull
    {
        var type = typeof(T);
        return type.GetField(enumValue.ToString()!)?
            .GetCustomAttribute<EnumMemberAttribute>()?.Value;
    }

    public static T? ToEnum<T>(string? value)
    {
        var type = typeof(T);
        var enumMemberValue = Enum.GetNames(type)
            .Map(i => type.GetField(i))
            .Map(i => (i?.Name, i?.GetCustomAttribute<EnumMemberAttribute>()?.Value))
            .FirstOrDefault(i => i.Value == value).Name;

        if (enumMemberValue == null)
            return default;

        return (T?)Enum.Parse(type, enumMemberValue);
    }

}

