using GestaoAvaliacao.Worker.Domain.MongoDB.Base;
using GestaoAvaliacao.Worker.Domain.MongoDB.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoAvaliacao.Worker.Domain.MongoDB.Entities.Tests
{
    [CollectionNameWorker("CorrectionResults")]
    public class CorrectionResultsEntityWorker : EntityWorkerMongoDBBase
    {
        public Guid? Dre_id { get; set; }
        public int? Esc_id { get; set; }
        public int? NumberAnswers { get; set; }
        public long? Tur_id { get; set; }
        public long? Test_id { get; set; }
        public int? Cur_id { get; set; }
        public int? Crr_id { get; set; }
        public int? Crp_id { get; set; }

        public CorrectionResultsEntityWorker()
        {
        }

        public CorrectionResultsEntityWorker(Guid ent_id, long test_id, long tur_id)
        {
            this._id = string.Format("{0}_{1}_{2}", ent_id, test_id, tur_id);
            this.Tur_id = tur_id;
            this.Test_id = test_id;
        }

        public CorrectionResultsEntityWorker(Guid ent_id, long test_id, long tur_id, Guid dre_id, int esc_id, int cur_id, int crr_id, int crp_id)
        {
            _id = string.Format("{0}_{1}_{2}", ent_id, test_id, tur_id);
            Dre_id = dre_id;
            Esc_id = esc_id;
            Tur_id = tur_id;
            Test_id = test_id;
            Cur_id = cur_id;
            Crr_id = crr_id;
            Crp_id = crp_id;
        }

        //lista com descrição das questões ( 1(A))
        public List<CorrectionResultsItems> Answers { get; set; }

        public CorrectionResultsSectionStats Statistics { get; set; }

        public List<CorrectionResultsStudents> Students { get; set; }
    }

    public class CorrectionResultsItems
    {
        public long Item_Id { get; set; }
        public long Alternative_Id { get; set; }
        public int Order { get; set; }
        public string RightChoice { get; set; }
        public bool Revoked { get; set; }
        public long Discipline_Id { get; set; }
    }

    public class CorrectionResultsSectionStats
    {
        public double GeneralHits { get; set; }
        public double GeneralGrade { get; set; }
        public List<Averages> Averages { get; set; }
    }

    public class CorrectionResultsStudents
    {
        public long alu_id { get; set; }
        public string alu_nome { get; set; }
        public int? Hits { get; set; }
        public double? Performance { get; set; }
        public string AbsenceReason { get; set; }
        public int mtu_numeroChamada { get; set; }

        public List<CorrectionResultsStudentsAnswers> Alternatives { get; set; }

    }

    public class CorrectionResultsStudentsAnswers
    {
        public long Item_Id { get; set; }
        public string Numeration { get; set; }
        public bool Correct { get; set; }
        public bool Revoked { get; set; }
        public long Alternative_Id { get; set; }
        public long Discipline_Id { get; set; }
    }

    public class Averages
    {
        public long Item_Id { get; set; }
        public double Average { get; set; }
        public bool Revoked { get; set; }
        public long Discipline_Id { get; set; }
    }
}
