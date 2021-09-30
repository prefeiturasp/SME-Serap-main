using GestaoAvaliacao.Worker.Domain.MongoDB.Base;
using GestaoAvaliacao.Worker.Domain.MongoDB.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoAvaliacao.Worker.Domain.MongoDB.Entities.Tests
{
    [CollectionNameWorker("SectionTestStats")]
    public class SectionTestStatsEntityWorker : EntityWorkerMongoDBBase
    {
        public long Test_Id { get; set; }
        public long tur_id { get; set; }
        public double GeneralGrade { get; set; }
        public double GeneralHits { get; set; }
        public Guid dre_id { get; set; }
        public int esc_id { get; set; }
        public int NumberAnswers { get; set; }
        public List<SectionTestStatsAnswes> Answers { get; set; }

        public SectionTestStatsEntityWorker()
        {

        }

        public SectionTestStatsEntityWorker(long test_id, long tur_id, Guid ent_id, Guid dre_id, int esc_id)
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
