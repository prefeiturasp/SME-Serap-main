using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestSectionStatusCorrectionBusiness
    {
        TestSectionStatusCorrection Save(TestSectionStatusCorrection entity);
        TestSectionStatusCorrection Update(TestSectionStatusCorrection entity);
        TestSectionStatusCorrection SetStatusCorrection(long test_id, long tur_id, EnumStatusCorrection status);
        Task SetCorrectionStatusAsync(long test_id, long tur_id, EnumStatusCorrection status);
        TestSectionStatusCorrection Get(long test_id, long tur_id);
        Task<TestSectionStatusCorrection> GetAsync(long test_Id, long tur_id);
        IEnumerable<TestSectionStatusCorrection> GetByTest(long test_id);
        IEnumerable<TestSectionStatusCorrection> GetBySchool(long Test_Id, int esc_id);
        IEnumerable<TestStatsEntitiesDTO> GetFinalizedEntities(long? Test_Id = null, string Year = null, Guid? uad_id = null, int? esc_id = null, long? tur_id = null,
            DateTime? FinalizationDate = null);
        IEnumerable<FinalizedTestYearDTO> GetSectionsToCalculate(DateTime? PartialDate);
        Task<List<TempCorrectionResult>> TempCorrectionResults(Guid ent_id);
        Task<TempCorrectionResult> UpdateTempCorrection(TempCorrectionResult entity);
        Task<TempCorrectionResult> GetTempCorrection(Guid ent_id, long test_id, long tur_id);
        //Task<TempCorrectionResult> GenerateTempCorrectionResult(Guid ent_id, long test_id, long tur_id);
    }
}
