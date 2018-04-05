using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class TestTemplate
	{
		public int Order { get; set; }
		public long Item_Id { get; set; }
		public long Alternative_Id { get; set; }
		public string Numeration { get; set; }
        [NotMapped]
        public int? KnowledgeArea_Id { get; set; }
        [NotMapped]
        public string KnowledgeArea_Description { get; set; }
    }
}
