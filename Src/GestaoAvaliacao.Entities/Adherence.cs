using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Entities.Enumerator;

namespace GestaoAvaliacao.Entities
{
    public class Adherence : EntityBase
	{
		public long EntityId { get; set; }
		public long? ParentId { get; set; }
		public EnumAdherenceEntity TypeEntity { get; set; }
		public EnumAdherenceSelection TypeSelection { get; set; }
		public virtual Test Test { get; set; }
		public long Test_Id { get; set; }
	}
}
