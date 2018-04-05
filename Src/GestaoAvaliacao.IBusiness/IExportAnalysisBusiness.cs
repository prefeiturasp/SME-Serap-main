using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IExportAnalysisBusiness
	{
		IEnumerable<ExportAnalysis> GetByExecutionState(EnumServiceState state);
		IEnumerable<ExportAnalysisDTO> Search(ref Pager pager,ExportAnalysisFilter filter);
		ExportAnalysis Save(ExportAnalysis entity);
		ExportAnalysis Update(ExportAnalysis entity);
	}
}
