using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IStudentCorrectionBusiness
    {
        Task<StudentCorrection> Save(Answer entity, long alu_id, long test_id, long tur_id, Guid ent_id, bool api, int ordemItem, bool provaEntregue);
        Task<StudentCorrection> Get(long alu_id, long test_id, long tur_id, Guid ent_id);
        Task<List<StudentCorrection>> GetByTest(long test_id, long tur_id);
        Task<List<StudentCorrection>> GetByTest(List<long> testId);

        Task<List<StudentCorrection>> Save(List<StudentCorrection> corrections);
        Task<bool> Delete(StudentCorrection entity);
        Task<long> CountInconsistency(long test_id, long tur_id);

        Task<StudentCorrection> SaveAPI(List<Answer> answerList, long alu_id, TestDTO testModel);
        StudentCorrection GetStudentCorrectionByTestAluId(long test_Id, long alu_id);
    }
}
