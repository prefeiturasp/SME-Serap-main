using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface ICognitiveCompetenceBusiness
	{
        CognitiveCompetence Save(CognitiveCompetence entity, Guid entityid);
        CognitiveCompetence Update(long id, CognitiveCompetence entity, Guid entityid);
        CognitiveCompetence Delete(long id);
		CognitiveCompetence Get(long id);
		IEnumerable<CognitiveCompetence> Load(ref Pager pager, Guid entityid);
		IEnumerable<CognitiveCompetence> Search(string search, ref Pager pager, Guid entityid);
        IEnumerable<CognitiveCompetence> FindAll(Guid EntityId);
	}
}
