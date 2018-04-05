using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IStudentTestAbsenceReasonRepository
	{
		IEnumerable<CorrectionStudentGrid> GetByTestSection(long test_id, long tur_id, IEnumerable<long> aluMongoList, bool ignoreBlocked);
        IEnumerable<CorrectionStudentGrid> GetByTestSectionByAluId(long test_id, long tur_id, long alu_id, bool ignoreBlocked);

        StudentTestAbsenceReason GetByTestStudent(long test_id, long tur_id, long alu_id);
		StudentTestAbsenceReason Save(StudentTestAbsenceReason entity);
		StudentTestAbsenceReason Update(StudentTestAbsenceReason entity);
		StudentTestAbsenceReason Remove(StudentTestAbsenceReason entity);
		IEnumerable<StudentTestAbsenceReason> GetAbsencesByTestSection(long test_id, long tur_id);
        IEnumerable<StudentTestAbsenceReason> GetAbsencesByTest(long test_id);
        int CountAbsencesByTestSection(long test_id, long tur_id);
		IEnumerable<long> StudentAbsencesByTestSection(long test_id, long tur_id);
        SchoolDTO GetEscIdDreIdByTeam(long tur_id);
    }
}
