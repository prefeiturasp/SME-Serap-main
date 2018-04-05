using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class AdherenceMap : EntityBaseMap<Adherence>
	{
		public AdherenceMap()
		{
			ToTable("Adherence");

			HasRequired(p => p.Test)
				.WithMany()
				.HasForeignKey(p => p.Test_Id);
		}
	}
}
