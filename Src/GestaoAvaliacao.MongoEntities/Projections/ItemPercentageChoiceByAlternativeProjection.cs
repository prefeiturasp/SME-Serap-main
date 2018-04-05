using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities.Projections
{
    public class ItemPercentageChoiceByAlternativeProjection
    {
        public long Item_Id { get; set; }
        public long Discipline_Id { get; set; }
        public List<AlternativeAverageProjection> Alternatives { get; set; }
    }

    public class AlternativeAverageProjection
    {
        public long Alternative_Id { get; set; }
        public string Numeration { get; set; }
        public double Avg { get; set; }
    }
}
