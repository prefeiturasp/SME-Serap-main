using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Entities.DTO
{
	[Serializable]
    public class ExportAnalysisDTO
	{
		public long Test_Id { get; set; }
		public string TestDescription { get; set; }
		public string ApplicationStartDate { get; set; }
		public string ApplicationEndDate { get; set; }
        public string TestTypeDescription { get; set; }
		public EnumServiceState StateExecution { get; set; }

		public string CreateDate { get; set; }
		public string UpdateDate { get; set; }

		public long FileId { get; set; }
	}
}
