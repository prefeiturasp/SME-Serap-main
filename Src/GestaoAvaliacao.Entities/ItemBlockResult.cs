
namespace GestaoAvaliacao.Entities
{
    public class ItemBlockResult
	{
        public long ItemId { get; set; }

		public string ItemCode { get; set; }

		public int ItemVersion { get; set; }

		public string BaseTextDescription { get; set; }

		public string Statement { get; set; }

		public string MatrixDescription { get; set; }

		public string DescriptorSentence { get; set; }

		public long? BaseTextId { get; set; }

		public long MatrixId { get; set; }
	
		public bool LastVersion { get; set; }

        public string ItemLevelDescription { get; set; }

        public int? ItemLevelValue { get; set; }

        public int TypeCurriculumGradeId { get; set; }

        public int? Order { get; set; }
        public string DisciplineDescription { get; set; }

        public bool? Revoked { get; set; }
        public virtual long? KnowledgeArea_Id { get; set; }
    }
}
