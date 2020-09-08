using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestSectionStatusCorrectionRepository
	{
		TestSectionStatusCorrection Save(TestSectionStatusCorrection entity);
		TestSectionStatusCorrection Update(TestSectionStatusCorrection entity);
		Task<TestSectionStatusCorrection> UpdateAsync(TestSectionStatusCorrection entity);
		TestSectionStatusCorrection SetStatusCorrectionUpdate(TestSectionStatusCorrection entity);

		TestSectionStatusCorrection Get(long Test_Id, long tur_id);
		Task<TestSectionStatusCorrection> GetAsync(long Test_Id, long tur_id);
		IEnumerable<TestSectionStatusCorrection> GetByTest(long Test_Id);
		IEnumerable<TestSectionStatusCorrection> GetBySchool(long Test_Id, int esc_id);
		IEnumerable<TestStatsEntitiesDTO> GetFinalizedEntities(long? Test_Id, string Year, Guid? uad_id, int? esc_id, long? tur_id, DateTime? FinalizationDate);
        IEnumerable<FinalizedTestYearDTO> GetSectionsToCalculate(DateTime? PartialDate);
        List<TestSectionStatusCorrection> GetAll();

    }
}
