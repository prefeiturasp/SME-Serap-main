using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Contracts
{
    public interface IStudentCorrectionAuxiliarRepository
    {
        Task<IEnumerable<StudentCorrectionAnswerGrid>> GetTestQuestionsAsync(long Id);
        Task<IEnumerable<CorrectionStudentGrid>> GetByTestSectionAsync(long test_id, long tur_id, IEnumerable<long> aluMongoList, bool ignoreBlocked);
        Task<IEnumerable<StudentTestAbsenceReason>> GetAbsencesByTestSectionAsync(long test_id, long tur_id);
        Task<IEnumerable<DisciplineItem>> GetDisciplineItemByTestIdAsync(long test_id);
        Task<SchoolDTO> GetEscIdDreIdByTeamAsync(long tur_id);
        Task<IEnumerable<long>> GetRevokedItemsByTestAsync(long test_id);
    }
}