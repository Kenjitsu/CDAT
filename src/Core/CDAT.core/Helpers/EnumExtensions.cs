using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CDAT.core.Helpers;
public static class EnumExtensions
{
    public static string GetDescription<T>(this T enumValue) where T : Enum
    {
        var displayAttribute = enumValue.GetType()
            .GetField(enumValue.ToString())
            ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault() as DescriptionAttribute;

        return displayAttribute?.Description ?? enumValue.ToString();
    }
}