namespace GestaoAvaliacao.Entities.Projections
{
    public class ItemWithOrderAndRevoked
    {
        public long Block_Id { get; set; }

        public virtual Item Item { get; set; }
        public long Item_Id { get; set; }

        public int Order { get; set; }

        public bool Revoked { get; set; }
        public long Discipline_Id { get; set; }
    }
}
