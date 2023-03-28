using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GestaoAvaliacao.Util
{
    public static class StringHelper
    {
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string RemoveSpecialCharactersWithRegex(string str, string replacement, string pattern = null)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                pattern = Constants.RegexSpecialChar;
            }

            return Regex.Replace(str, pattern, replacement, RegexOptions.Compiled);
        }

        public static bool ValidateValuesAllowed(string[] valuesAllowed, string valueToValidate)
        {
            return valuesAllowed == null || valuesAllowed.ToList().Contains(valueToValidate);
        }

        private static readonly Dictionary<char, char> _specialChars = new Dictionary<char, char>
        {
            { 'á', 'a' },{ 'à', 'a' },{ 'â', 'a' },{ 'ã', 'a' },{ 'ä', 'a' },
            { 'é', 'e' },{ 'è', 'e' },{ 'ê', 'e' },{ 'ë', 'e' },
            { 'í', 'i' },{ 'ì', 'i' },{ 'î', 'i' },{ 'ï', 'i' },
            { 'ó', 'o' },{ 'ò', 'o' },{ 'õ', 'o' },{ 'ö', 'o' },
            { 'ú', 'u' },{ 'ù', 'u' },{ 'û', 'u' },{ 'ü', 'u' },
            { 'ç', 'c' },{ 'ñ', 'n' }
        };

        private static readonly Regex _spaceRegex = new Regex(@"\s", RegexOptions.Compiled);
        private static readonly Regex _numericRegex = new Regex(@"\s", RegexOptions.Compiled);

        public static string Normalize(this string str, CharType normaliar = CharType.All)
        {
            foreach (var item in _specialChars.ToList())
            {
                char key = item.Key.ToString().ToUpper()[0];
                char value = item.Value.ToString().ToUpper()[0];
                if (!_specialChars.ContainsKey(key))
                {
                    _specialChars.Add(key,value);
                }
            }

            if (normaliar.HasFlag(CharType.Spaces))
            {
                str = _spaceRegex.Replace(str, string.Empty);
            }

            if (normaliar.HasFlag(CharType.Special))
            {
                str = _specialChars.ToList()
                    .Aggregate(str, (ac, item) => ac.Replace(item.Key, item.Value));
            }

            if (normaliar.HasFlag(CharType.Numerics))
            {
                str = _numericRegex.Replace(str, string.Empty);
            }

            if (normaliar.HasFlag(CharType.UpperCase))
            {
                str = str.ToLower();
            }

            return str;
        }

        public enum CharType
        {
            Spaces = 1,
            Special = 2,
            Numerics = 4,
            UpperCase = 8,
            All = 15,
        }

        public static string ChangeAccentuation(string str)
        {
            string withAccent = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            string withoutAccent = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";
            for (int i = 0; i < withAccent.Length; i++)
            {
                str = str.Replace(withAccent[i].ToString(), withoutAccent[i].ToString());
            }
            return str;
        }

        public static string NormalizeFileName(string str)
        {
            string pattern = @"(?![a-z0-9áàâãéèêíìîóòôõúùûçñ\s_-]).";
            return Regex.Replace(str, pattern, string.Empty, RegexOptions.IgnoreCase);
        }

        public static int PositionOfNewLine(string text)
        {
            var positionOfNewLine = text.IndexOf("\r\n", StringComparison.Ordinal);

            if (positionOfNewLine < 0)
                positionOfNewLine = text.IndexOf("\r", StringComparison.Ordinal);

            if (positionOfNewLine < 0)
                positionOfNewLine = text.IndexOf("\n", StringComparison.Ordinal);

            if (positionOfNewLine < 0)
                positionOfNewLine = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);

            return positionOfNewLine;
        }
    }
}
