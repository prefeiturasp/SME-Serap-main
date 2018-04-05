namespace GestaoAvaliacao.Entities.Projections
{
    public class AlternativesWithNumerationAndOrderProjection
    {
        public long Alternative_Id { get; set; }
        public string Numeration { get; set; }
        public bool Correct { get; set; }
        public int Order { get; set; }
        public long Item_Id { get; set; }
        public bool ItemRevoked { get; set; }
        public int ItemOrder { get; set; }
    }
}
