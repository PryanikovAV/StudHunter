using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace StudHunter.API.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var memberInfo = enumValue
            .GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault();

        if (memberInfo == null)
        {
            return enumValue.ToString();
        }
        
        var displayAttribute = memberInfo
            .GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? enumValue.ToString();
    }

    public static TEnum FromDisplayName<TEnum>(string displayName) where TEnum : Enum
    {
        foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
        {
            if (value.GetDisplayName() == displayName)
                return value;
        }
        throw new ArgumentException($"Invalid display name: {displayName}");
    }
}