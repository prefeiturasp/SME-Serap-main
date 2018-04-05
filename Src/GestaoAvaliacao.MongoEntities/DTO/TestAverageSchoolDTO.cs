using System;

namespace GestaoAvaliacao.MongoEntities.DTO
{
	public class TestAverageSchoolDTO
	{
		public Guid? Dre_id { get; set; }
		public int? Esc_id { get; set; }
		public long? Test_id { get; set; }
		public long Discipline_Id { get; set; }
		public int TotalItems { get; set; }
		public int TotalCorretItems { get; set; }
		public double Media
		{
			get
			{
				if (TotalItems == 0)
				{
					return 0;
				}
				else
				{
					return ((double)TotalCorretItems * 100 / TotalItems);
				}
			}
		}
	}
}
