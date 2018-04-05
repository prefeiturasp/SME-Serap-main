using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class CognitiveCompetenceMap: EntityBaseMap<CognitiveCompetence>
	{
		public CognitiveCompetenceMap()
		{
			ToTable("CognitiveCompetence");

			Property(p => p.Description)
						.IsRequired()
						.HasMaxLength(500)
						.HasColumnType("varchar");
		}
	}
}
