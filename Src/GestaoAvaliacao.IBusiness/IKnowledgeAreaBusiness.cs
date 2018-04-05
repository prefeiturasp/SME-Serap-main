using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IKnowledgeAreaBusiness
    {
        KnowledgeArea Get(long id);
        IEnumerable<KnowledgeArea> Load(ref Pager pager, Guid EntityId);
        IEnumerable<KnowledgeArea> Search(string search, ref Pager pager, Guid EntityId);
        KnowledgeArea Save(KnowledgeArea entity, Guid entityid);
        KnowledgeArea Update(KnowledgeArea entity, Guid ent_id);
        KnowledgeArea Delete(long id);
        bool ExistsModelDescription(long id, string description, Guid ent_id);
        List<AJX_Select2> LoadAllKnowledgeAreaActive(string description, Guid EntityId);
    }
}
