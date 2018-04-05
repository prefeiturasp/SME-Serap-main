using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class ExportAnalysisBusiness : IExportAnalysisBusiness
	{
		#region Dependences

		readonly IExportAnalysisRepository exportAnalysisRepository;

		#endregion

		public ExportAnalysisBusiness(IExportAnalysisRepository exportAnalysisRepository)
		{
			this.exportAnalysisRepository = exportAnalysisRepository;
		}

		public ExportAnalysis Save(ExportAnalysis entity)
		{
			entity.Validate = this.Validate(entity);

			if (entity.Validate.IsValid)
			{
				var cadastred = exportAnalysisRepository.GetByTestId(entity.Test_Id);

				if (cadastred != null)
				{
                    cadastred.StateExecution = entity.StateExecution;
                    entity = exportAnalysisRepository.Update(cadastred);
				}
				else
				{
					entity = exportAnalysisRepository.Save(entity);
				}
			}

			return entity;
		}

		public ExportAnalysis Update(ExportAnalysis entity)
		{
			entity.Validate = this.Validate(entity);

			if (entity.Validate.IsValid)
			{
				entity = exportAnalysisRepository.Update(entity);
			}

			return entity;
		}

		public IEnumerable<ExportAnalysisDTO> Search(ref Pager pager, ExportAnalysisFilter filter)
		{
			return exportAnalysisRepository.Search(ref pager, filter);
		}

		public IEnumerable<ExportAnalysis> GetByExecutionState(EnumServiceState state)
		{
			return exportAnalysisRepository.GetByExecutionState(state);
		}


		#region Private methods
		


		private Validate Validate(ExportAnalysis entity)
		{
			Validate valid = new Util.Validate();

			if (entity.Test_Id == 0)
				valid.Message = "Prova é obrigatório.";			

			if (!string.IsNullOrEmpty(valid.Message))
			{
				string br = "<br/>";
				valid.Message = valid.Message.TrimStart(br.ToCharArray());

				valid.IsValid = false;

				if (valid.Code <= 0)
					valid.Code = 400;

				valid.Type = ValidateType.alert.ToString();
			}
			else
				valid.IsValid = true;

			return valid;
		}
		#endregion
	}
}
