using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmountTools
{
    public enum Language
    {
        Persian,
        English
    }

    public enum CurrencyUnit
    {
        None,
        Toman,
        Rial
    }

    public static class NumberToWordsConverter
    {
        //public static string Convert(long number, bool inputIsRial = true, Language lang = Language.Persian, CurrencyUnit showAsCurrency = CurrencyUnit.Toman)
        //{
        //    // تبدیل ریال به تومان در صورت نیاز
        //    if (inputIsRial && showAsCurrency == CurrencyUnit.Toman)
        //        number = number / 10;

        //    string result = lang == Language.English
        //        ? ConvertToEnglish(number)
        //        : ConvertToPersian(number);

        //    switch (showAsCurrency)
        //    {
        //        case CurrencyUnit.Toman:
        //            result += lang == Language.English ? " Tomans" : " تومان";
        //            break;
        //        case CurrencyUnit.Rial:
        //            result += lang == Language.English ? " Rials" : " ریال";
        //            break;
        //        default:
        //            break;
        //    }

        //    return result;
        //}

        public static string Convert(long number, bool addCurrency = true, Language lang = Language.Persian, bool treatAsToman = true)
        {
            if (number == 0)
                return lang == Language.Persian
                    ? (addCurrency ? "صفر تومان" : "صفر")
                    : (addCurrency ? "Zero Toman" : "Zero");

            long toman = treatAsToman ? number / 10 : 0;
            long rial = treatAsToman ? number % 10 : number;

            string tomanText = treatAsToman && toman > 0 ? ConvertToWords(toman, lang) : "";
            string rialText = rial > 0 ? ConvertToWords(rial, lang) : "";

            if (lang == Language.Persian)
            {
                if (!treatAsToman)
                {
                    // کل عدد به ریال
                    return rialText + (addCurrency ? " ریال" : "");
                }

                // حالت تومان و ریال
                string result = "";
                if (toman > 0)
                    result += tomanText + (addCurrency ? " تومان" : "");
                if (rial > 0)
                {
                    if (toman > 0)
                        result += " و ";
                    result += rialText + " ریال";
                }
                return result;
            }
            else
            {
                if (!treatAsToman)
                    return rialText + (addCurrency ? " Rials" : "");

                string result = "";
                if (toman > 0)
                    result += tomanText + (addCurrency ? " Toman" : "");
                if (rial > 0)
                {
                    if (toman > 0)
                        result += " and ";
                    result += rialText + " Rials";
                }
                return result;
            }
        }

        private static string ConvertToWords(long number, Language lang)
        {
            return lang == Language.Persian
                ? ConvertToPersianWords(number)
                : ConvertToEnglishWords(number);
        }



        #region Persian

        private static readonly string[] Yekan = { "", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
        private static readonly string[] Dahgan = { "", "ده", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        private static readonly string[] DahYek = { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
        private static readonly string[] Sadgan = { "", "صد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        private static readonly string[] PersianScales = { "", "هزار", "میلیون", "میلیارد", "تریلیون", "کادریلیون", "کوینتیلیون" };

        private static string ConvertToPersianWords(long number)
        {
            if (number == 0) return "صفر";

            string result = "";
            int groupIndex = 0;

            while (number > 0)
            {
                int groupNumber = (int)(number % 1000);
                if (groupNumber != 0)
                {
                    string groupText = ConvertThreeDigitPersian(groupNumber);
                    if (!string.IsNullOrWhiteSpace(PersianScales[groupIndex]))
                        groupText += " " + PersianScales[groupIndex];

                    if (!string.IsNullOrWhiteSpace(result))
                        result = groupText + " و " + result;
                    else
                        result = groupText;
                }

                number /= 1000;
                groupIndex++;
            }

            return result;
        }

        private static string ConvertThreeDigitPersian(int number)
        {
            int sadgan = number / 100;
            int remainder = number % 100;
            int dahgan = remainder / 10;
            int yekan = remainder % 10;

            List<string> parts = new List<string>();

            if (sadgan != 0)
                parts.Add(Sadgan[sadgan]);

            if (remainder >= 10 && remainder <= 19)
                parts.Add(DahYek[remainder - 10]);
            else
            {
                if (dahgan != 0)
                    parts.Add(Dahgan[dahgan]);

                if (yekan != 0)
                    parts.Add(Yekan[yekan]);
            }

            return string.Join(" و ", parts);
        }

        #endregion

        #region English

        private static readonly string[] Ones = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
        private static readonly string[] Teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static readonly string[] Tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
        private static readonly string[] EnglishScales = { "", "Thousand", "Million", "Billion", "Trillion", "Quadrillion", "Quintillion" };

        private static string ConvertToEnglishWords(long number)
        {
            if (number == 0) return "Zero";

            int groupIndex = 0;
            string result = "";

            while (number > 0)
            {
                int group = (int)(number % 1000);
                if (group != 0)
                {
                    string groupText = ConvertThreeDigitEnglish(group);
                    if (!string.IsNullOrEmpty(EnglishScales[groupIndex]))
                        groupText += " " + EnglishScales[groupIndex];

                    if (!string.IsNullOrEmpty(result))
                        result = groupText + ", " + result;
                    else
                        result = groupText;
                }

                number /= 1000;
                groupIndex++;
            }

            return result.TrimEnd(',', ' ');
        }

        private static string ConvertThreeDigitEnglish(int number)
        {
            int hundreds = number / 100;
            int remainder = number % 100;
            int tens = remainder / 10;
            int ones = remainder % 10;

            List<string> parts = new List<string>();

            if (hundreds > 0)
                parts.Add(Ones[hundreds] + " Hundred");

            if (remainder >= 10 && remainder <= 19)
                parts.Add(Teens[remainder - 10]);
            else
            {
                if (tens > 0)
                    parts.Add(Tens[tens]);

                if (ones > 0)
                    parts.Add(Ones[ones]);
            }

            return string.Join(" ", parts);
        }

        #endregion
    }

    public static class NumberAmountTools
    {
        public static string FormatWithSeparator(long number, string separator = ",")
        {
            return string.Format("{0:N0}", number).Replace(",", separator);
        }
        public static string FormatWithSeparator(string input, string separator = ",")
        {
            if (long.TryParse(input.Replace(separator, "").Replace("٬", ""), out long number))
                return FormatWithSeparator(number, separator);
            return "";
        }

    }
}