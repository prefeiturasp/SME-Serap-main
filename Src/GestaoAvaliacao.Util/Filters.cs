using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Util
{
    public class AnswerSheetBatchFilter
    {
        public AnswerSheetBatchFilter()
        {
            if (StartDate != null && StartDate.Equals(DateTime.MinValue))
                StartDate = null;

            if (EndDate != null && EndDate.Equals(DateTime.MinValue))
                EndDate = null;

            FilterDateUpdate = false;
        }
        public Guid EntId { get; set; }
        public long TestId { get; set; }
        public long BatchId { get; set; }
        public long BatchQueueId { get; set; }
        public Guid? SupAdmUnitId { get; set; }
        public int? SchoolId { get; set; }
        public long? SectionId { get; set; }
        public bool FilterDateUpdate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Processing { get; set; }
        public Guid UserId { get; set; }
        public Guid? PesId { get; set; }
        public int CoreVisionId { get; set; }
        public int CoreSystemId { get; set; }
        public Guid CoreGroupId { get; set; }
        public int CurriculumTypeId { get; set; }
        public int ShiftTypeId { get; set; }
        public IEnumerable<string> UadList { get; set; }
        public EnumFollowUpIdentificationDataType Type { get; set; }
        public EnumFollowUpIdentificationView View { get; set; }
        public string AluNome { get; set; }
        public string Turma { get; set; }
        public bool ShowAllStudentsLot { get; set; }
    }

    public class AnswerSheetBatchQueueFilter
    {
        public AnswerSheetBatchQueueFilter()
        {
            if (StartDate != null && StartDate.Equals(DateTime.MinValue))
                StartDate = null;

            if (EndDate != null && EndDate.Equals(DateTime.MinValue))
                EndDate = null;
        }
        public long BatchId { get; set; }
        public Guid? SupAdmUnitId { get; set; }
        public int? SchoolId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public EnumBatchQueueSituation? Situation { get; set; }
        public Guid UserId { get; set; }
        public Guid? PesId { get; set; }
        public int VisionId { get; set; }
        public int SystemId { get; set; }
        public Guid GroupId { get; set; }
        public long? top { get; set; }
        public IEnumerable<string> uads { get; set; }
    }

    public class ExportAnalysisFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long Code { get; set; }
    }

    public class AnswerSheetLotFilter
    {
        public long Lot_Id { get; set; }
        public long Test_Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public EnumServiceState StateExecution { get; set; }
        public EnumAnswerSheetBatchOwner Type { get; set; }
        public Guid? SupAdmUnitId { get; set; }
        public int? SchoolId { get; set; }
        public long? TestType_Id { get; set; }
    }

    public class StudentResponseFilter
    {
        public long Test_Id { get; set; }
        public DateTime CorrectionEndDate { get; set; }
        public long TestType_Id { get; set; }
        public int School_Id { get; set; }
        public bool AllAdhered { get; set; }
        public int? ttn_id { get; set; }
        public Guid? usu_id { get; set; }
        public Guid? pes_id { get; set; }
        public Guid? uad_id { get; set; }
        public int? crp_ordem { get; set; }
        public int? vis_id { get; set; }
        public int? sis_id { get; set; }
        public string StatusCorrection { get; set; }
    }
}
