using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ICognitiveCompetenceRepository
	{
		CognitiveCompetence Save(CognitiveCompetence entity);
		void Update(CognitiveCompetence entity);
		CognitiveCompetence Get(long id);
		IEnumerable<CognitiveCompetence> Load(ref Pager pager, Guid EntityId);
		void Delete(long id);
		IEnumerable<CognitiveCompetence> Search(string search, ref Pager pager, Guid EntityId);
		bool ExistsDescriptionNamed(string description, Guid ent_id);
		bool ExistsDescriptionNamedAlter(String Description, int id, Guid ent_id);
		IEnumerable<CognitiveCompetence> FindAll(Guid EntityId);
	}
}
