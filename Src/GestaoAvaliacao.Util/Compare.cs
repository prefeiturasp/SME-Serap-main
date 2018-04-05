using System;

namespace GestaoAvaliacao.Util
{
    public static class Compare
    {
        public static bool ValidateEqualsInt(long? valor1, long? valor2)
        {
            return ((valor1 == null && valor2 == null) || valor1 == valor2);
        }

        public static bool ValidateEqualsDecimal(decimal? valor1, decimal? valor2)
        {
            return ((valor1 == null && valor2 == null) || valor1 == valor2);
        }

        public static bool ValidateEqualsString(string valor1, string valor2)
        {
            return ((string.IsNullOrEmpty(valor1) && string.IsNullOrEmpty(valor2)) || valor1.Equals(valor2));
        }

        /// <summary>
        /// dt < hoje
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool ValidateShortDateLessThan(DateTime dt)
        {
            DateTime date = Convert.ToDateTime(dt.ToShortDateString());
            DateTime today = Convert.ToDateTime(DateTime.Now.ToShortDateString());

            return date < today;
        }

        /// <summary>
        /// dt > hoje
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool ValidateShortDateGreatherThan(DateTime dt)
        {
            DateTime date = Convert.ToDateTime(dt.ToShortDateString());
            DateTime today = Convert.ToDateTime(DateTime.Now.ToShortDateString());

            return date > today;
        }

        /// <summary>
        /// dt <= hoje
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool ValidateShortDateLessEqual(DateTime dt)
        {
            DateTime date = Convert.ToDateTime(dt.ToShortDateString());
            DateTime today = Convert.ToDateTime(DateTime.Now.ToShortDateString());

            return date <= today;
        }

        /// <summary>
        /// dt >= hoje
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool ValidateShortDateGreatherEqual(DateTime dt)
        {
            DateTime date = Convert.ToDateTime(dt.ToShortDateString());
            DateTime today = Convert.ToDateTime(DateTime.Now.ToShortDateString());

            return date >= today;
        }

        /// <summary>
        /// dt1 <= hoje && dt2 >= hoje
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static bool ValidateBetweenShortDates(DateTime dt1, DateTime dt2)
        {
            return ValidateShortDateGreatherEqual(dt2) && ValidateShortDateLessEqual(dt1);
        }
    }
}
