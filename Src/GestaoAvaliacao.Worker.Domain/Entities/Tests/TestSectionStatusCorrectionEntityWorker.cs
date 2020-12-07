using GestaoAvaliacao.Util;
using GestaoAvaliacao.Worker.Domain.Base;

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