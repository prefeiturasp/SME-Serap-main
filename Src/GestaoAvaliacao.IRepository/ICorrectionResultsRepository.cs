using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.MongoEntities.Projections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
	public interface ICorrectionResultsRepository
	{
		/// <summary>
		/// Busca as informações do aluno na prova informada, como nome, número de matricula e desempenho na mesma
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="alu_id">Id do aluno</param>
		/// <returns>Projection com as informações do aluno em determinada prova</returns>
		Task<StudentTestInformationProjection> GetStudentTestInformationByTestAndStudent(long test_Id, long alu_id);

        /// <summary>
        /// Busca as médias(acertos e porcentagem) da SME, DRE, escola e turma da prova passada(test_id)
        /// </summary>
        /// <param name="test_Id">Id da prova</param>
        /// <param name="esc_Id">Id da escola</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="team_id">Id da turma</param>
        /// <returns>Projection com médias(acertos e porcentagem) da SME, DRE, escola e turma em relação a prova passada</returns>
        List<TestAverageTeamResult> GetTestAveragesHitsAndPercentagesByTest(List<long> test_Id, long? discipline_id);
        /// <summary>
        /// Busca as médias(porcentagem) de acertos da SME e DRE em relação a cada item da prova
        /// </summary>
        /// <param name="test_Id">Id da prova</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <returns>Lista de projection com as médias(porcentagem) da SME e DRE em relação a cada item da prova</returns>
        Task<List<ItemAvgPercentageSMEAndDREProjection>> GetAvgPercentageSmeAndDrePerItemByTest(long test_Id, Guid dre_id);

		/// <summary>
		/// Busca as médias de escolha por alternativa de cada item em uma prova
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <param name="esc_id">Id da escola</param>
		/// <returns>Lista de projection com as médias(porcentagem) de escolha da DRE por alternativa de cada item da prova</returns>
		Task<List<ItemPercentageChoiceByAlternativeProjection>> GetItemPercentageChoiceByAlternative(long test_Id, long? discipline_id, Guid? dre_id, int? esc_id);

		Task<CorrectionResults> GetEntity(CorrectionResults entity);
		Task<CorrectionResults> Insert(CorrectionResults entity);
		Task<long> Count(CorrectionResults entity);
		Task<CorrectionResults> Replace(CorrectionResults entity);
		Task<bool> Delete(CorrectionResults entity);

		List<TestAverageDreDTO> GetTestAverageByTestGroupByDre(long testId, long? discipline_id);
		List<TestAverageSchoolDTO> GetTestAverageByTestDreGroupBySchool(long testId, long? discipline_id, Guid dre_id);
		List<TestAveragePerformanceDTO> GetTestAveragePerformanceGeneral(long testId, long? discipline_id);
		List<TestAverageItemPerformanceDTO> GetTestAverageItemPerformanceGeneral(long testId);
        List<TestAverageItemPerformanceDTO> GetTestAverageItemPerformanceByDre(List<long> testId);
        List<TestAverageItemPerformanceDTO> GetTestAverageItemPerformanceBySchool(long test_Id, Guid dre_id);

        Task<List<CorrectionResults>> GetByTest(List<long> testId);

    }
}
