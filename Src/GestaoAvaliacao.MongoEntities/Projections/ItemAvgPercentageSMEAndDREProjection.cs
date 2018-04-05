using System;

namespace GestaoAvaliacao.MongoEntities.Projections
{
    public class ItemAvgPercentageSMEAndDREProjection
    {
        private double? avgSME;
        private double? avgDRE;

        public long? Item_Id { get; set; }

        public double? AvgSME
        {
            get { return RoundValues(avgSME); }
            set { avgSME = value; }
        }

        public double? AvgDRE
        {
            get { return RoundValues(avgDRE); }
            set { avgDRE = value; }
        }

        public double? RoundValues(double? value)
        {
            return value.HasValue ? (double?)Math.Round((double)value, 2) : null;

        }
    }
}
