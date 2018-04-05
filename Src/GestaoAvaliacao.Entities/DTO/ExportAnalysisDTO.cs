using GestaoAvaliacao.Util;

namespace GestaoAvaliacao.Entities.DTO
{
    public class ExportAnalysisDTO
	{
		public long Test_Id { get; set; }
		public string TestDescription { get; set; }
		public string TestTypeDescription { get; set; }
		public EnumServiceState StateExecution { get; set; }

		public string CreateDate { get; set; }
		public string UpdateDate { get; set; }

		public long FileId { get; set; }
	}
}
