using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace BstHelpers;

#region Strings
public static class StringExtensions {
    public static string Truncate(this string value, int maxLength, string suffix = "") {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength) + suffix;
    }

    public static string StripHtml(this string input) {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }

    public static bool IsValidPortugueseVat(this string vat) {
        if (vat.Length != 9) return false;
        var added =
            (int)char.GetNumericValue(vat[7]) * 2 +
            (int)char.GetNumericValue(vat[6]) * 3 +
            (int)char.GetNumericValue(vat[5]) * 4 +
            (int)char.GetNumericValue(vat[4]) * 5 +
            (int)char.GetNumericValue(vat[3]) * 6 +
            (int)char.GetNumericValue(vat[2]) * 7 +
            (int)char.GetNumericValue(vat[1]) * 8 +
            (int)char.GetNumericValue(vat[0]) * 9;
        var mod = added % 11;
        var control = 11 - mod;
        if (mod is 0 or 1) {
            control = 0;
        }

        return (int)char.GetNumericValue(vat[8]) == control;
    }

    public static bool IsValidPortugueseZipCode(this string zipCode) {
        if (zipCode.Length != 8) return false;
        var zipCodeRegex = new Regex(@"^[0-9]{4}-[0-9]{3}$");
        return zipCodeRegex.IsMatch(zipCode);
    }

    public static bool IsValidEmail(this string email) {
        if (string.IsNullOrWhiteSpace(email)) return false;

        email = email.Trim();
        try {
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
        } catch (RegexMatchTimeoutException) {
            return false;
        } catch {
            return false;
        }
        try {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        } catch (RegexMatchTimeoutException) {
            return false;
        }
    }
    public static string NormalizeEmail(this string email) {
        return !IsValidEmail(email) ? "" : Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200)).ToLower();
    }

    public static string ToTitleCase(this string value) {
        var ci = new CultureInfo("pt-PT");
        return ci.TextInfo.ToTitleCase(value.ToLower());
    }
    public static string GetEmailLink(this string email, string classes = "text-body text-underline-hover") {
        return email.IsValidEmail() ? $"<a href=\"mailto:{email}\" class=\"{classes}\">{email}</a>" : email;
    }

    public static string FormatPhoneNumber(this string phoneNumber) {
        if (string.IsNullOrEmpty(phoneNumber))
            return "";
        if (phoneNumber.Length == 9) {
            return $"{phoneNumber.Substring(0, 3)} {phoneNumber.Substring(3, 3)} {phoneNumber.Substring(6, 3)}";
        }
        return phoneNumber;
    }

    private static string DomainMapper(Match match) {
        // Use IdnMapping class to convert Unicode domain names.
        var idn = new IdnMapping();

        // Pull out and process domain name (throws ArgumentException on invalid)
        var domainName = idn.GetAscii(match.Groups[2].Value);

        return match.Groups[1].Value + domainName;
    }
}
#endregion

#region Dates
public static class DateTimeExtensions {
    public static string FormatDate(this DateTime date, bool minutes = false, bool monthName = false) {
        var formattingString = "dd-MM-yyyy";
        if (monthName) {
            var dateString = "";
            dateString = date.Day + " de " + Humanizer.GetMonthName(date) + " de " + date.Year;
            if (!minutes)
                return dateString;
            return dateString + " " + date.ToString("HH:mm");
        }
        if (minutes) formattingString = string.Concat(formattingString, " HH:mm");
        return date.ToString(formattingString);
    }
    public static string FormatDate(this DateTime? date, bool minutes = false, bool monthName = false, string replace = "") {
        if (date == null) return replace == "" ? "" : replace;
        return ((DateTime)date).FormatDate(minutes, monthName);
    }
    public static string FormatTime(this DateTime time, bool seconds = false, bool spacing = false) {
        var spacingFormat = ":";
        if (spacing)
            spacingFormat = " : ";
        var formatString = $"HH{spacingFormat}mm";
        if (seconds)
            formatString = $"HH{spacingFormat}mm{spacingFormat}ss";
        return time.ToString(formatString);
    }
}
#endregion

#region Time
public static class TimeSpanExtensions {
    public static string GetDaysDiff(this TimeSpan time) {
        var returnString = "";
        if (time.Days > 0)
            returnString += $"{time.Days} dias";
        return returnString;
    }
    public static string FormatTime(this TimeSpan time, bool seconds = false, bool spacing = false) {
        return (new DateTime(time.Ticks)).FormatTime(seconds, spacing);
    }
    public static string FormatTime(this TimeSpan? time, bool seconds = false, bool spacing = false) {
        return time == null ? "" : ((TimeSpan)time).FormatTime(seconds, spacing);
    }
}
#endregion

#region Numbers
public static class NumericExtensions {
    public static string FormatNumber(this decimal number, bool currencySign = false, bool percentageSign = false, int decimalPlaces = 2, bool autoDecimalPlaces = false, bool replaceZero = false, bool javascript = false) {
        if (replaceZero && number == 0) return "";

        var newCultureInfo = new CultureInfo(CultureInfo.CurrentCulture.Name, true);
        var cultureInfoClone = (CultureInfo)newCultureInfo.Clone();

        cultureInfoClone.NumberFormat.NumberDecimalSeparator = ",";
        cultureInfoClone.NumberFormat.PercentDecimalSeparator = ",";
        cultureInfoClone.NumberFormat.CurrencyDecimalSeparator = ",";
        cultureInfoClone.NumberFormat.NumberGroupSeparator = " ";
        cultureInfoClone.NumberFormat.PercentGroupSeparator = " ";
        cultureInfoClone.NumberFormat.CurrencyGroupSeparator = " ";


        string formattingString;
        if (autoDecimalPlaces) {
            formattingString = "#,###,##0.######";
        } else if (decimalPlaces <= 0) {
            formattingString = "#,###,##0";
        } else {
            formattingString = "#,###,##0.";
            for (var i = 1; i <= decimalPlaces; i++) formattingString += "0";
        }
        var numberString = javascript ? number.ToString(formattingString, CultureInfo.InvariantCulture) : number.ToString(formattingString, cultureInfoClone);
        return string.Concat(numberString, currencySign ? "€" : "", percentageSign ? "%" : "");
    }

    public static string FormatNumber(this decimal? number, bool currencySign = false, bool percentageSign = false, int decimalPlaces = 2, bool autoDecimalPlaces = false, bool replaceZero = false, bool javascript = false) {
        return (number ?? 0M).FormatNumber(currencySign, percentageSign, decimalPlaces, autoDecimalPlaces, replaceZero, javascript);
    }
    public static string FormatNumber(this double number, bool currencySign = false, bool percentageSign = false, int decimalPlaces = 2, bool autoDecimalPlaces = false, bool replaceZero = false, bool javascript = false) {
        return ((decimal)number).FormatNumber(currencySign, percentageSign, decimalPlaces, autoDecimalPlaces, replaceZero, javascript);
    }
    public static string FormatNumber(this double? number, bool currencySign = false, bool percentageSign = false, int decimalPlaces = 2, bool autoDecimalPlaces = false, bool replaceZero = false, bool javascript = false) {
        return ((decimal)(number ?? 0d)).FormatNumber(currencySign, percentageSign, decimalPlaces, autoDecimalPlaces, replaceZero, javascript);
    }
}
#endregion

#region Boolean
public static class BooleanExtensions {

    public static string FormatBool(this bool? b, string yes = "Sim", string no = "Não") {
        return b == null ? "" : ((bool)b.Value).FormatBool(yes, no);
    }
    public static string FormatBool(this bool b, string yes = "Sim", string no = "Não") {
        return b ? yes : no;
    }

}
#endregion

#region Object
public static class ObjectExtensions {
    public static string SerializeObject(this object obj, bool formatted = true, int maxDepth = 1) {
        return JsonConvert.SerializeObject(
            obj, new JsonSerializerSettings { MaxDepth = maxDepth, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = formatted ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None });
    }
    public static void LogObject(this object obj, bool error = false, bool formatted = true, int maxDepth = 1) {
        FunctionsHelper.LogMessage(
            obj.SerializeObject(formatted: formatted, maxDepth: maxDepth), error: error
            );
    }
    public static void LogString(this object obj) {
        Console.WriteLine(obj.ToString());
    }
    public static string ToXml(this object obj) {
        using var stringWriter = new StringWriter();
        var serializer = new XmlSerializer(obj.GetType());
        serializer.Serialize(stringWriter, obj);
        var doc = new XmlDocument();
        doc.LoadXml(stringWriter.ToString());
        return doc.DocumentElement?.InnerXml ?? "";
    }
}
#endregion

#region Types
public static class TypeExtensions {
    public static int? GetMaxLength(this Type model, string property) {
        return model.GetProperty(property)?.GetCustomAttribute<MaxLengthAttribute>()?.Length;
    }
    public static string GetControllerName(this Type controller) {
        return controller.Name.Replace("Controller", "");
    }
}
#endregion

#region Enum
public static class EnumExtensions {
    public static string NameForHumans(this Enum enumValue) {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.GetName() ?? enumValue.ToString();
    }
    public static string DescriptionForHumans(this Enum enumValue) {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.GetDescription() ?? enumValue.ToString();
    }
    public static IEnumerable<T> GetValues<T>() where T : Enum {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}
#endregion

#region Enumerable
public static class EnumerableExtensions {
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) => self?.Select((item, index) => (item, index)) ?? new List<(T, int)>();
    public static List<(T item, int index)> WithIndexList<T>(this IEnumerable<T> self) => self?.Select((item, index) => (item, index)).ToList() ?? new List<(T, int)>();
    public static T? GetRandomElement<T>(this IEnumerable<T> self) {
        var enumerable = self as T[] ?? self.ToArray();
        var count = enumerable.Length;
        var rand = new Random();
        return enumerable.ElementAtOrDefault(rand.Next(Math.Max(count, 1)));
    }
}
#endregion

#region Database
public static class DatabaseExtensions {
    public static string GetSqlTableName(this Type type, DbContext? dbContext = null) {
        foreach (var item in type.GetCustomAttributes(false)) {
            if (item.GetType() == typeof(TableAttribute)) {
                return ((TableAttribute)item).Name;
            }
        }
        if (dbContext != null) {
            var dbType = dbContext.GetType();
            var dbSetProp = dbType.GetProperties()
                .FirstOrDefault(p => p.PropertyType.GenericTypeArguments.Any(t => t == type));
            if (dbSetProp != null) {
                return dbSetProp.Name;
            }
        }
        return type.Name.Replace(type?.Namespace ?? string.Empty, "");
    }
    public static string GetSqlPropertyName(this Type type, string propertyName) {
        var stampProperty = type.GetProperties().FirstOrDefault(p => p.Name == propertyName);
        if (stampProperty == null) return string.Empty;
        foreach (var item in stampProperty.GetCustomAttributes(false)) {
            if (item.GetType() == typeof(ColumnAttribute)) {
                return ((ColumnAttribute)item)?.Name ?? string.Empty;
            }
        }
        return propertyName;
    }
}
#endregion