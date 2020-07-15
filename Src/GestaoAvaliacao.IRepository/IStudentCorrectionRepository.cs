using GestaoAvaliacao.MongoEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface IStudentCorrectionRepository
    {
        Task<StudentCorrection> GetEntity(StudentCorrection entity);

        Task<StudentCorrection> Insert(StudentCorrection entity);

        Task<long> Count(StudentCorrection entity);

        Task<StudentCorrection> Replace(StudentCorrection entity);

        Task<List<StudentCorrection>> GetByTest(long test_id, long tur_id);

        Task<List<StudentCorrection>> GetByTest(List<long> testId);

        Task<bool> Delete(StudentCorrection entity);

        Task<long> CountInconsistency(long test_id, long tur_id);

        StudentCorrection GetStudentCorrectionByTestAluId(long test_Id, long alu_id);

        Task InsertOrReplaceAsync(StudentCorrection entity);

        Task<StudentCorrection> FindOneAsync(StudentCorrection entity);
    }
}