using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Entities
{
    public class ReportCorrectionTestResult
    {
        public ReportCorrectionTestResult()
        {
            Validate = new Validate();
        }

        public long Test_Id { get; set; }
        public string Description { get; set; }
        public DateTime ApplicationStartDate { get; set; }
        public DateTime ApplicationEndDate { get; set; }
        public DateTime CorrectionStartDate { get; set; }
        public DateTime CorrectionEndDate { get; set; }
        public DateTime? ProcessedCorrectionDate { get; set; }
        public Guid uad_id { get; set; }
        public string uad_nome { get; set; }
        public long esc_id { get; set; }
        public string esc_nome { get; set; }
        public long tur_id { get; set; }
        public string tur_descricao { get; set; }
        public Validate Validate { get; set; }
    }
}
