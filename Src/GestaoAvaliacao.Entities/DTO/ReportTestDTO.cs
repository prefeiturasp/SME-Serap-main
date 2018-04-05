using System.Collections.Generic;

namespace GestaoAvaliacao.Entities.DTO
{
    public class ReportTestPerformanceItemDTO
	{
		public List<ReportTestPerformanceItemItemsDTO> Items { get; set; }
		public List<ReportTestPerformanceItemEntityDTO> Schools { get; set; }
		public List<ReportTestPerformanceItemEntityDTO> AdministrativeUnityPerformance { get; set; }
		public ReportTestPerformanceItemEntityDTO EntityPerformance { get; set; }
	}

	public class ReportTestPerformanceItemItemsDTO
	{
		public long Item_Id { get; set; }
		public string ItemIndex { get; set; }
	}

	public class ReportTestPerformanceItemEntityDTO
	{
		public string EntityName { get; set; }

		public List<ReportTestPerformanceItemItemDTO> ItemHits { get; set; }

		public int TotalSubEntity { get; set; }
	}

	public class ReportTestPerformanceItemItemDTO
	{
		public long Item_Id { get; set; }
		public double ItemGrade { get; set; }
	}
}
