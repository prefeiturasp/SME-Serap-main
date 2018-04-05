using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities.Projections
{
    public class StudentTestInformationProjection
    {
        public StudentTestInformationProjection()
        {
            Items = new List<Projections.ItemProjection>();
        }
        public long Alu_id { get; set; }
        public int Mtu_numeroChamada { get; set; }
        public string Alu_nome { get; set; }
        public int? Hits { get; set; }
        public double? Avg { get; set; }
        public List<ItemProjection> Items { get; set; }
    }

    public class ItemProjection
    {
        public long Item_Id { get; set; }
        public string Numeration { get; set; }
        public bool Correct { get; set; }
        public bool Revoked { get; set; }
        public long Alternative_Id { get; set; }
        public long Discipline_Id { get; set; }
    }
}
