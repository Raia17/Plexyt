using System.Globalization;

namespace BstHelpers;

public static class Humanizer {

    #region Numbers
    public static string CurrencyWriting(double number) {
        var intValue = (int)Math.Truncate(number);
        var decValue = (int)((number - (int)number) * 100);

        var word = string.Concat(Hundreds(intValue), " euro", intValue == 1 ? "" : "s");
        if (decValue > 0) word = string.Concat(word, " e ", Tens(decValue), " cêntimos");

        return word.Trim();
    }

    private static string Hundreds(int value) {
        var hundreds = "";
        switch (value) {
            case 100:
                hundreds = "cem";
                break;
            case > 100: {
                    var hundred = Convert.ToInt32(value.ToString()[..1]);
                    hundreds = hundred switch {
                        1 => "cento",
                        2 => "duzentos",
                        3 => "trezentos",
                        4 => "quatrocentos",
                        5 => "quinhentos",
                        6 => "seiscentos",
                        7 => "setecentos",
                        8 => "oitocentos",
                        9 => "novecentos",
                        _ => hundreds
                    };
                    var tens = Tens(value - hundred * 100);
                    if (tens != "") hundreds += " e " + tens;

                    break;
                }
            default:
                hundreds = Tens(value);
                break;
        }

        return hundreds;
    }

    private static string Tens(int value) {
        var tens = "";
        switch (value) {
            case 0:
                tens = "";
                break;
            case 1:
                tens = "um";
                break;
            case 2:
                tens = "dois";
                break;
            case 3:
                tens = "três";
                break;
            case 4:
                tens = "quatro";
                break;
            case 5:
                tens = "cinco";
                break;
            case 6:
                tens = "seis";
                break;
            case 7:
                tens = "sete";
                break;
            case 8:
                tens = "oito";
                break;
            case 9:
                tens = "nove";
                break;
            case 10:
                tens = "dez";
                break;
            case 11:
                tens = "onze";
                break;
            case 12:
                tens = "doze";
                break;
            case 13:
                tens = "treze";
                break;
            case 14:
                tens = "quatorze";
                break;
            case 15:
                tens = "quinze";
                break;
            case 16:
                tens = "dezasseis";
                break;
            case 17:
                tens = "dezassete";
                break;
            case 18:
                tens = "dezoito";
                break;
            case 19:
                tens = "dezanove";
                break;
            default: {
                    var tensTemp = Convert.ToInt32(value.ToString()[..1]);

                    tens = tensTemp switch {
                        2 => "vinte",
                        3 => "trinta",
                        4 => "quarenta",
                        5 => "cinquenta",
                        6 => "sessenta",
                        7 => "setenta",
                        8 => "oitenta",
                        9 => "noventa",
                        _ => tens
                    };
                    var unity = Convert.ToInt32(value.ToString().Substring(1, 1));
                    switch (unity) {
                        case 1:
                            tens += " e um";
                            break;
                        case 2:
                            tens += " e dois";
                            break;
                        case 3:
                            tens += " e três";
                            break;
                        case 4:
                            tens += " e quatro";
                            break;
                        case 5:
                            tens += " e cinco";
                            break;
                        case 6:
                            tens += " e seis";
                            break;
                        case 7:
                            tens += " e sete";
                            break;
                        case 8:
                            tens += " e oito";
                            break;
                        case 9:
                            tens += " e nove";
                            break;
                    }
                    break;
                }
        }
        return tens;
    }
    #endregion

    #region Dates
    public static string GetWeekDay(DateTime date) {
        return date.ToString("dddd", CultureInfo.CreateSpecificCulture("pt-PT"));
    }
    public static string GetMonthName(int? month) {
        return month == null ? "" : GetMonthName((int)month);
    }
    public static string GetMonthName(int month) {
        return GetMonthName(new DateTime(DateTime.Now.Year, month, 1));
    }
    public static string GetMonthName(DateTime date) {
        var ci = new CultureInfo("pt-PT");
        return date.ToString("MMMM", ci).ToTitleCase();
    }
    public static string GetMonthName(DateTime? date) {
        return date == null ? "" : GetMonthName((DateTime)date);
    }
    #endregion

    #region Greeting
    public static string GetGreeting() {
        var greeting = "Boa noite";
        if (DateTime.Now.Hour > 6) greeting = "Bom dia";

        if (DateTime.Now.Hour > 12) greeting = "Boa tarde";

        if (DateTime.Now.Hour > 19) greeting = "Boa noite";

        return greeting;
    }
    #endregion

}