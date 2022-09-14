using System.Globalization;
using System.Threading;

namespace Plataforma.Helpers;

public static class CultureHelper {
    public static void SetCulture() {
        var newCultureInfo = new CultureInfo(CultureInfo.CurrentCulture.Name, true);
        var cultureInfoClone = (CultureInfo)newCultureInfo.Clone();
        cultureInfoClone.DateTimeFormat.DateSeparator = "/";
        cultureInfoClone.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        cultureInfoClone.DateTimeFormat.LongDatePattern = "dd/MM/yyyy";
        cultureInfoClone.DateTimeFormat.FullDateTimePattern = "dd/MM/yyyy HH:mm:ss";

        cultureInfoClone.NumberFormat.CurrencySymbol = "€";
        cultureInfoClone.NumberFormat.NumberDecimalSeparator = ".";
        cultureInfoClone.NumberFormat.PercentDecimalSeparator = ".";
        cultureInfoClone.NumberFormat.CurrencyDecimalSeparator = ".";
        cultureInfoClone.NumberFormat.NumberGroupSeparator = " ";
        cultureInfoClone.NumberFormat.PercentGroupSeparator = " ";
        cultureInfoClone.NumberFormat.CurrencyGroupSeparator = " ";


        CultureInfo.DefaultThreadCurrentCulture = cultureInfoClone;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfoClone;
        CultureInfo.CurrentCulture = cultureInfoClone;
        CultureInfo.CurrentUICulture = cultureInfoClone;

        Thread.CurrentThread.CurrentCulture = cultureInfoClone;
        Thread.CurrentThread.CurrentUICulture = cultureInfoClone;
    }

}