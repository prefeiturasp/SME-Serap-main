using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ExportAnalysisMap : EntityBaseMap<ExportAnalysis>
	{
		public ExportAnalysisMap()
		{
			ToTable("ExportAnalysis");

			HasRequired(p => p.Test)
				.WithMany()
				.HasForeignKey(p => p.Test_Id);
		}
	}
}
