using GestaoAvaliacao.Worker.Domain.Entities.AbsenceReasons;
using GestaoAvaliacao.Worker.Domain.Entities.Schools;
using GestaoAvaliacao.Worker.Domain.Entities.StudentCorrections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Contracts
{
    public interface IStudentCorrectionAuxiliarRepository
    {
        Task<IEnumerable<StudentCorrectionAnswerGridEntityWorker>> GetTestQuestionsAsync(long Id);

        Task<IEnumerable<CorrectionStudentGridEntityWorker>> GetByTestSectionAsync(long test_id, long tur_id, IEnumerable<long> aluMongoList, bool ignoreBlocked);

        Task<IEnumerable<StudentTestAbsenceReasonEntityWorker>> GetAbsencesByTestSectionAsync(long test_id, long tur_id);

        Task<IEnumerable<DisciplineItemEntityWorker>> GetDisciplineItemByTestIdAsync(long test_id);

        Task<SchoolEntityWorker> GetEscIdDreIdByTeamAsync(long tur_id);

        Task<IEnumerable<long>> GetRevokedItemsByTestAsync(long test_id);
    }
}