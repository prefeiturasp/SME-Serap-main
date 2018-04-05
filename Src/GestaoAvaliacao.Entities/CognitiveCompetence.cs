using GestaoAvaliacao.Entities.Base;
using System;

namespace GestaoAvaliacao.Entities
{
    public class CognitiveCompetence : EntityBase
	{
		public string Description { get; set; }
		public Guid EntityId { get; set; }
	}
}
