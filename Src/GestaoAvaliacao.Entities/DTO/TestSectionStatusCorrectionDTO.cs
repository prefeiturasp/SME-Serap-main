using System;

namespace GestaoAvaliacao.Entities.DTO
{
    public class TestStatsEntitiesDTO
	{
		public long tur_id { get; set; }
		public int esc_id { get; set; }
		public Guid uad_id { get; set; }
		public Guid uad_idSuperiorGestao { get; set; }
		public long Test_Id { get; set; }
	}

	public class FinalizedTestYearDTO
	{
		public long Test_Id { get; set; }
		public int crp_ordem { get; set; }
		public int tne_id { get; set; }
		public int esc_id { get; set; }
		public Guid uad_idSuperiorGestao { get; set; }
		public long tur_id { get; set; }
	}
}
