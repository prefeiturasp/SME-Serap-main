using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class StudentTestAbsenceReason : EntityBase
	{
		public long alu_id { get; set; }

		public virtual Test Test { get; set; }
		public long Test_Id { get; set; }
		
		public long tur_id { get; set; }

		public virtual AbsenceReason AbsenceReason { get; set; }
		public long AbsenceReason_Id { get; set; }
	}
}
