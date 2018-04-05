using System;

namespace GestaoAvaliacao.Util.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Compare two strings with trim and ignoring case
        /// </summary>
        /// <param name="text">First string</param>
        /// <param name="textToCompare">Second string</param>
        /// <returns></returns>
        public static bool CompareWithTrim(this string text, string textToCompare)
        {
            return text.Trim().Equals(textToCompare.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Verify if the string is null or empty or white space
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyOrWhiteSpace(this string text)
        {
            return string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text);
        }
    }
}
