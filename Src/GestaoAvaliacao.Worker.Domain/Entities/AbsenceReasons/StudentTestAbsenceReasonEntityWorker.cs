namespace GestaoAvaliacao.Worker.Domain.Entities.AbsenceReasons
{
    public class StudentTestAbsenceReasonEntityWorker
    {
        public long alu_id { get; set; }
        public long Test_Id { get; set; }
        public long tur_id { get; set; }
        public virtual AbsenceReasonEntityWorker AbsenceReason { get; set; }
        public long AbsenceReason_Id { get; set; }
    }
}