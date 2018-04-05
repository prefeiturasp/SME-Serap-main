using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class StudentTestAbsenceReasonMap : EntityBaseMap<StudentTestAbsenceReason>
	{
		public StudentTestAbsenceReasonMap()
		{
			ToTable("StudentTestAbsenceReason");

			HasRequired(i => i.AbsenceReason)
				.WithMany()
				.HasForeignKey(f => f.AbsenceReason_Id);


			HasRequired(i => i.Test)
				.WithMany()
				.HasForeignKey(f => f.Test_Id);
		}
	}
}
