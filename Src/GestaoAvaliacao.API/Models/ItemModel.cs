namespace GestaoAvaliacao.API.Models
{
    public class ItemModel
    {
        public long Id { get; set; }
        public long AlternativeId { get; set; }
        public int Order { get; set; }
        public int AlternativeOrder { get; set; }
        public bool EmptyAlternative { get; set; }
        public bool DuplicateAlternative { get; set; }
    }
}