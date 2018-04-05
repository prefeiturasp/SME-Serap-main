using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MongoEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Services
{
    public class GenerateNewCorrectionResult
	{		
		private readonly ITestSectionStatusCorrectionBusiness _testSectionStatusCorrectionBusiness;
		private readonly ICorrectionBusiness _correctionBusiness;
		private readonly IParameterBusiness _parameterBusiness;		
		private readonly IBlockBusiness _blockBusiness;

		public GenerateNewCorrectionResult(ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness,
			ICorrectionBusiness correctionBusiness, IParameterBusiness parameterBusiness, IBlockBusiness blockBusiness)

		{			
			_testSectionStatusCorrectionBusiness = testSectionStatusCorrectionBusiness;
			_correctionBusiness = correctionBusiness;
			_parameterBusiness = parameterBusiness;			
			_blockBusiness = blockBusiness;
		}

		public async Task Execute()
		{
			try
			{
				Parameter entidadePadrao = _parameterBusiness.GetByKey("ENTIDADE"); 

				if (entidadePadrao == null || string.IsNullOrEmpty(entidadePadrao.Value))
				{
					throw new ValidationException("Parâmetro 'Entidade padrão do sistema' é obrigatório.");
				}

				Parameter answerDuplicate = _parameterBusiness.GetParamByKey("CODE_ALTERNATIVE_DUPLICATE", new Guid(entidadePadrao.Value));
				Parameter answerEmpty = _parameterBusiness.GetParamByKey("CODE_ALTERNATIVE_EMPTY", new Guid(entidadePadrao.Value));

				List<TempCorrectionResult> lstTestSection = await _testSectionStatusCorrectionBusiness.TempCorrectionResults(new Guid(entidadePadrao.Value));

				var lstTestGroupByTest = lstTestSection.GroupBy(u => u.Test_id).Select((n) => new { Test_id = n.Key, Turmas = n.ToList() }).OrderByDescending(o => o.Test_id);

				foreach (var testSection in lstTestGroupByTest)
				{
					MongoEntities.TestTemplate testTemplate = await _correctionBusiness.GetTestTemplate(testSection.Test_id, new Guid(entidadePadrao.Value));
					IEnumerable<StudentCorrectionAnswerGrid> answers = _blockBusiness.GetTestQuestions(testSection.Test_id);

					foreach (var turma in testSection.Turmas)
					{
						await _correctionBusiness.InsertCorretionResults(turma.Tur_id, turma.Test_id, new Guid(entidadePadrao.Value), testTemplate, answers, answerDuplicate, answerEmpty);
						turma.Processed = true;
						turma.UpdateDate = DateTime.Now;
						await _testSectionStatusCorrectionBusiness.UpdateTempCorrection(turma);
					}
				}
			}
			catch (ValidationException ex)
			{
				LogFacade.LogFacade.SaveError(ex, ex.Message);
			}
			catch (Exception e)
			{
				LogFacade.LogFacade.SaveError(e);
			}
		}
	}
}
