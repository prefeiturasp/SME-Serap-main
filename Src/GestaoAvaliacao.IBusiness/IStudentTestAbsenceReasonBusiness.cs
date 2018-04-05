using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IStudentTestAbsenceReasonBusiness
	{
		IEnumerable<CorrectionStudentGrid> GetByTestSection(long test_id, long tur_id, IEnumerable<long> aluMongoList, bool ignoreBlocked);
        IEnumerable<CorrectionStudentGrid> GetByTestSectionByAluId(long test_id, long tur_id, long alu_id, bool ignoreBlocked);
        StudentTestAbsenceReason Save(StudentTestAbsenceReason entity, Guid usu_id, Guid ent_id, bool manual);
        int CountAbsencesByTestSection(long test_id, long tur_id);
		IEnumerable<StudentTestAbsenceReason> GetAbsencesByTestSection(long test_id, long tur_id);
        IEnumerable<StudentTestAbsenceReason> GetAbsencesByTest(long test_id);
        IEnumerable<long> StudentAbsencesByTestSection(long test_id, long tur_id);
        StudentTestAbsenceReason GetByTestStudent(long test_id, long tur_id, long alu_id);
        StudentTestAbsenceReason Remove(StudentTestAbsenceReason entity);
        SchoolDTO GetEscIdDreIdByTeam(long tur_id);

    }
}
