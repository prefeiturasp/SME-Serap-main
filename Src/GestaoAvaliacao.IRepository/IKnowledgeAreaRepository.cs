using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IKnowledgeAreaRepository
    {
        KnowledgeArea Save(KnowledgeArea entity);
        KnowledgeArea Update(KnowledgeArea entity);
        KnowledgeArea Get(long id);
        IEnumerable<KnowledgeArea> Load(Guid EntityId, ref Pager pager);
        IEnumerable<KnowledgeArea> Search(Guid EntityId, String search, ref Pager pager);
        void Delete(KnowledgeArea entity);
        bool ExistsModelDescription(long id, string description, Guid ent_id);
        bool ExistsSubject(long Id);
        List<AJX_Select2> LoadAllKnowledgeAreaActive(string description, Guid EntityId);
    }
}
