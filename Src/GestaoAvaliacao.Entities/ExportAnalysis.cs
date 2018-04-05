using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Util;

namespace GestaoAvaliacao.Entities
{
    public class ExportAnalysis : EntityBase
	{
		public long Test_Id { get; set; }
		public virtual Test Test { get; set; }
		public EnumServiceState StateExecution { get; set; }
	}
}
