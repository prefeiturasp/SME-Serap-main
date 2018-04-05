using GestaoAvaliacao.Entities;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.Projections;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.IBusiness
{
	public interface ICorrectionResultsBusiness
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
		TestAveragesHitsAndPercentagesProjection GetTestAveragesHitsAndPercentagesByTest(long test_id, int? esc_Id, Guid? dre_id, long? team_id, long? discipline_id);
		/// <summary>
		/// Busca as médias(acertos e porcentagem) da SME, DRE, escola e turma da prova passada(test_id)
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="esc_Id">Id da escola</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <param name="team_id">Id da turma</param>
		/// <returns>Projection com médias(acertos e porcentagem) da SME, DRE, escola e turma em relação a prova passada</returns>
		TestAveragesHitsAndPercentagesProjection GetAveragesByTest(long test_id, int? esc_id, Guid? dre_id, long? tcp_id, long? discipline_id, SYS_Usuario user, SYS_Grupo group);
		/// <summary>
		/// Busca as médias(acertos e porcentagem) da SME, DRE, escola e turma das provas do TestSubGroup_Id 
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="esc_Id">Id da escola</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <param name="team_id">Id da turma</param>
		/// <returns>Projection com médias(acertos e porcentagem) da SME, DRE, escola e turma em relação a prova passada</returns>
		TestAveragesHitsAndPercentagesProjection GetAveragesByTestSubGroup_Id(long subgroup_id, int? esc_id, Guid? dre_id, long? tcp_id, long? discipline_id, SYS_Usuario user, SYS_Grupo group);

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

		Task<CorrectionResults> Save(CorrectionResults entity);
		Task<CorrectionResults> GetEntity(CorrectionResults entity);
		CorrectionResults GetResultFilterByDiscipline(CorrectionResults results, long? discipline_id);
		Task<bool> Delete(CorrectionResults entity);
		Task<EntityFile> GetResultFile(Guid ent_id, long test_id, long tur_id, long? discipline_id, string separator, string virtualDirectory, string physicalDirectory);
		Task<CorrectionResults> UnblockCorrection(long team_id, long test_id, SYS_Usuario user, EnumSYS_Visao visao);
		Task<CorrectionResults> GenerateCorrectionResults(Guid ent_id, long test_id, long tur_id);
		Task<CorrectionResults> GenerateCorrectionResultsInsert(Guid ent_id, long test_id, long tur_id, MongoEntities.TestTemplate testTemplate,
			IEnumerable<StudentCorrectionAnswerGrid> answers, Parameter answerDuplicate, Parameter answerEmpty);
	}
}
