using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IExportAnalysisRepository
	{
		IEnumerable<ExportAnalysisDTO> Search(ref Pager pager, ExportAnalysisFilter filter);
		ExportAnalysis GetByTestId(long TestId);
		IEnumerable<ExportAnalysis> GetByExecutionState(EnumServiceState state);
		ExportAnalysis Save(ExportAnalysis entity);
		ExportAnalysis Update(ExportAnalysis entity);
	}
}
