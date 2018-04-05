using GestaoAvaliacao.MongoEntities.Attribute;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities
{
    [CollectionName("SectionTestStats")]
	public class SectionTestStats : EntityBase
	{
		public long Test_Id { get; set; }
		public long tur_id { get; set; }
		public double GeneralGrade { get; set; }
		public double GeneralHits { get; set; }
        public Guid dre_id { get; set; }
        public int esc_id { get; set; }
        public int NumberAnswers { get; set; }
		public List<SectionTestStatsAnswes> Answers { get; set; }

        public SectionTestStats()
		{

		}
		public SectionTestStats(long test_id, long tur_id, Guid ent_id, Guid dre_id, int esc_id)
		{
			this._id = string.Format("{0}_{1}_{2}", ent_id, test_id, tur_id);
			this.Test_Id = test_id;
			this.tur_id = tur_id;
            this.dre_id = dre_id;
            this.esc_id = esc_id;
            this.Answers = new List<SectionTestStatsAnswes>();
		}
		
	}

	public class SectionTestStatsAnswes
	{
		public long Item_Id { get; set; }
		public double Grade { get; set; }
    }
}
