using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class StudentCorrectionBusiness : IStudentCorrectionBusiness
    {
        private readonly IStudentCorrectionRepository _studentCorrectionRepository;
        private readonly IStudentTestAbsenceReasonRepository _studentTestAbsenceReasonRepository;

        public StudentCorrectionBusiness(IStudentCorrectionRepository studentCorrectionRepository, IStudentTestAbsenceReasonRepository studentTestAbsenceReasonRepository)
        {
            _studentCorrectionRepository = studentCorrectionRepository;
            _studentTestAbsenceReasonRepository = studentTestAbsenceReasonRepository;
        }

        public async Task<StudentCorrection> SaveAPI(List<Answer> answerList, long alu_id, TestDTO testModel)
        {
            var escola = _studentTestAbsenceReasonRepository.GetEscIdDreIdByTeam(testModel.tur_Id);

            var studentCorrection = new StudentCorrection(testModel.test_Id, testModel.tur_Id, alu_id, testModel.ent_Id, escola.dre_id, escola.esc_id);

            studentCorrection.Answers = answerList;
            studentCorrection.Automatic = true;
            await _studentCorrectionRepository.InsertOrReplaceAsync(studentCorrection);
            return studentCorrection;
        }

        public async Task<StudentCorrection> Save(Answer answer, long alu_id, long test_id, long tur_id, Guid ent_id, bool api, int ordemItem, bool provaEntregue)
        {
            var escola = _studentTestAbsenceReasonRepository.GetEscIdDreIdByTeam(tur_id);

            var studentCorrection = await _studentCorrectionRepository.FindOneAsync(new StudentCorrection(test_id, tur_id, alu_id, ent_id, escola.dre_id, escola.esc_id));
            if (studentCorrection is null)
            {
                studentCorrection = new StudentCorrection(test_id, tur_id, alu_id, ent_id, escola.dre_id, escola.esc_id);
                studentCorrection.CreateDate = DateTime.Now;
            }

            studentCorrection.Automatic = api;
            studentCorrection.OrdemUltimaResposta = ordemItem;
            studentCorrection.provaFinalizada = provaEntregue;

            studentCorrection.Answers.Add(answer);
            await _studentCorrectionRepository.InsertOrReplaceAsync(studentCorrection);
            return studentCorrection;
        }

        public async Task<List<StudentCorrection>> Save(List<StudentCorrection> corrections)
        {
            foreach (var item in corrections)
                await _studentCorrectionRepository.Replace(item);

            return corrections;
        }

        public Task<bool> Delete(StudentCorrection entity)
        {
            return _studentCorrectionRepository.Delete(entity);
        }

        public async Task<StudentCorrection> Get(long alu_id, long test_id, long tur_id, Guid ent_id)
        {
            var escola = _studentTestAbsenceReasonRepository.GetEscIdDreIdByTeam(tur_id);
            var result = await _studentCorrectionRepository.FindOneAsync(new StudentCorrection(test_id, tur_id, alu_id, ent_id, escola.dre_id, escola.esc_id));
            return result;
            //if (count == 0)
            //{
            //    return null;
            //}
            //else
            //    return await _studentCorrectionRepository.GetEntity(new StudentCorrection(test_id, tur_id, alu_id, ent_id, escola.dre_id, escola.esc_id));

        }

        public async Task<List<StudentCorrection>> GetByTest(long test_id, long tur_id)
        {
            return await _studentCorrectionRepository.GetByTest(test_id, tur_id);
        }

        public async Task<List<StudentCorrection>> GetByTest(List<long> testId)
        {
            return await _studentCorrectionRepository.GetByTest(testId);
        }

        public async Task<long> CountInconsistency(long test_id, long tur_id)
        {
            return await _studentCorrectionRepository.CountInconsistency(test_id, tur_id);
        }

        public StudentCorrection GetStudentCorrectionByTestAluId(long test_Id, long alu_id)
        {
            return _studentCorrectionRepository.GetStudentCorrectionByTestAluId(test_Id, alu_id);
        }
    }
}
