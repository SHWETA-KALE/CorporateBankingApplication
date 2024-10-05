using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Models
{
    public class ConvertAmountToWords
    {
        private static readonly string[] UnitsMap = {
        "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
        "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
        "Seventeen", "Eighteen", "Nineteen"
    };

        private static readonly string[] TensMap = {
        "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    };

        private static readonly string[] ThousandsMap = {
        "", "Thousand", "Million", "Billion"
    };

        public string Convert(double number)
        {
            if (number < 0)
                return "Minus " + Convert(-number);

            if (number == 0)
                return "Zero";

            string words = "";

            // Process whole number part
            int wholePart = (int)number;
            words += ConvertWholeNumber(wholePart);

            // Process decimal part
            int decimalPart = (int)((number - wholePart) * 100);
            if (decimalPart > 0)
            {
                words += " and " + ConvertWholeNumber(decimalPart) + " Cents";
            }

            return words.Trim();
        }

        private string ConvertWholeNumber(int number)
        {
            if (number == 0)
                return "";

            if (number < 20)
                return UnitsMap[number] + " ";

            if (number < 100)
                return TensMap[number / 10] + " " + ConvertWholeNumber(number % 10);

            if (number < 1000)
                return UnitsMap[number / 100] + " Hundred " + ConvertWholeNumber(number % 100);

            for (int i = 0; i < ThousandsMap.Length; i++)
            {
                int power = (int)Math.Pow(1000, i);
                if (number < power * 1000)
                    return ConvertWholeNumber(number / power) + ThousandsMap[i] + " " + ConvertWholeNumber(number % power);
            }

            return "";
        }
    }

}