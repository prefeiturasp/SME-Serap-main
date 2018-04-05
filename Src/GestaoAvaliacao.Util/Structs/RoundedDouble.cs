using System;

namespace GestaoAvaliacao.Util.Structs
{
    /// <summary>
    /// Arredonda valores double
    /// </summary>
    public struct RoundedDecimal
    {
        public double Value { get; private set; }

        public RoundedDecimal(double value) : this()
        {
            this.Value = value;
        }

        public static implicit operator double(RoundedDecimal d)
        {
            return Math.Round(d.Value, 2);
        }
    }
}
