using System;

namespace GestaoAvaliacao.MongoEntities.DTO
{
	public class TestAverageTeamResult
	{
		public Guid? Dre_id { get; set; }
		public int? Esc_id { get; set; }
		public long? Tur_id { get; set; }
		public long? Test_id { get; set; }
		public string Tur_codigo { get; set; }
        public int NumberAnswers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalCorretItems { get; set; }
		public int TotalItems { get; set; }

		public double Media
		{
			get
			{
                int tot = TotalStudents * NumberAnswers;
				if (tot == 0)
				{
					return 0;
				}
				else
				{
					return (Math.Round(((double)TotalCorretItems / tot) * 100, 2));
				}
			}
		}

	}
}
