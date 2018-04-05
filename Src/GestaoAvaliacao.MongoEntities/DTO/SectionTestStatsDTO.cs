using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities.DTO
{
    public class SectionTestStatsDTO
	{
		public List<SectionTestStatsGroupDTO> result { get; set; }
	}
	public class SectionTestStatsGroupDTO
	{
		public long _id { get; set; }
		public double Grade { get; set; }

		public double Count { get; set; }
	}
}
