using GestaoAvaliacao.Worker.Domain.Base;
using GestaoAvaliacao.Worker.Domain.Enums;

namespace GestaoAvaliacao.Worker.Domain.Entities.Tests
{
    public class TestSectionStatusCorrectionEntityWorker : EntityWorkerBase
    {
        public TestSectionStatusCorrectionEntityWorker()
            : base()
        {
        }

        public long Test_Id { get; set; }
        public long tur_id { get; set; }
        public EnumStatusCorrection StatusCorrection { get; set; }
    }
}