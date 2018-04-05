
namespace GestaoAvaliacao.Entities
{
    /// <summary>
    /// Objeto de retorno para a listagem do banco de itens
    /// </summary>
    public class ItemResult
    {
        public virtual string ItemCode { get; set; }

        public virtual bool? Revoked { get; set; }

        public virtual int ItemVersion { get; set; }

        public virtual string BaseTextDescription { get; set; }

        public virtual string Statement { get; set; }

        public virtual string MatrixDescription { get; set; }

        public virtual string DescriptorSentence { get; set; }

        public virtual long ItemId { get; set; }

        public virtual long? BaseTextId { get; set; }

        public virtual long MatrixId { get; set; }

        public virtual bool LastVersion { get; set; }

        public virtual bool? ItemNarrated { get; set; }

        public virtual long DisciplineId { get; set; }

        public virtual string DisciplineDescription { get; set; }
    }
}
