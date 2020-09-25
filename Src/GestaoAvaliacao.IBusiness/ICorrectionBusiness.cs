using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO.Tests;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreSSO = MSTech.CoreSSO.Entities;

namespace GestaoAvaliacao.IBusiness
{
    public interface ICorrectionBusiness
    {
        IEnumerable<SelectedSection> LoadOnlySelectedSectionPaginate(ref Pager pager, StudentResponseFilter filter);

        Task<IEnumerable<StudentCorrectionAnswerGrid>> GetStudentAnswer(long test_id, long tur_id, long alu_id, Guid ent_id);

        Task<Adherence> FinalizeCorrection(long team, long test_id, CoreSSO.SYS_Usuario user, EnumSYS_Visao visao);

        Task<Adherence> FinalizeCorrectionElectronicTest(long tur_id, long test_id, long alu_id, CoreSSO.SYS_Usuario user, EnumSYS_Visao visao);

        Task<Adherence> FinalizeAutomaticCorrection(long team, long test_id, Guid EntityId, MongoEntities.TestTemplate testTemplate);

        Task<CorrectionResults> GetResultCorrectionGrid(long tur_id, long test_id, long? discipline_id, Guid ent_id);

        Task<CorrectionResults> InsertCorretionResults(long tur_id, long test_id, Guid ent_id, MongoEntities.TestTemplate testTemplate,
            IEnumerable<StudentCorrectionAnswerGrid> answers, Parameter answerDuplicate, Parameter answerEmpty);

        Task<StudentTestAbsenceReason> SaveAbsenceReason(StudentTestAbsenceReason entity, Guid usuId, Guid pesId, EnumSYS_Visao visao, Guid ent_id);

        Task<StudentTestAbsenceReason> SaveAbsenceReasonApi(StudentTestAbsenceReason entity, Guid ent_id);

        Task<List<CorrectionStudentGrid>> GetGridCorrection(long tur_id, long test_id);

        Task<MongoEntities.TestTemplate> GetTestTemplate(long test_id, Guid ent_id);

        Task<StudentCorrection> SaveCorrection(long alu_id, long alternative_id, long item_id, bool n, bool r, long test_id, long tur_id, Guid ent_id, Guid usuId, Guid pesId, EnumSYS_Visao visao, bool manual, bool api = false, int ordemItem = 0, bool provaEletronica = false);

        Task<StudentCorrection> SaveCorrectionApi(long studentId, List<ItemModelDTO> itemModel, TestDTO testModel, MongoEntities.TestTemplate testTemplate);

        Task<StudentCorrection> SaveCorrectionAsync(long test_id, long alu_id, long tur_id, IEnumerable<AnswerModelDto> chosenAlternatives,
            Guid ent_id, Guid usuId, Guid pesId, EnumSYS_Visao visao, int ordemItem = 0);
    }
}