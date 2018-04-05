using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IPerformanceLevelRepository
    {
        PerformanceLevel Save(PerformanceLevel entity);
        void Update(PerformanceLevel entity);
        PerformanceLevel Get(long id);
        IEnumerable<PerformanceLevel> Load(ref Pager pager, Guid EntityId);
        void Delete(PerformanceLevel entity);
        IEnumerable<PerformanceLevel> Search(String search, ref Pager pager, Guid EntityId);
        IEnumerable<PerformanceLevel> LoadLevels(Guid EntityId);
        bool ExistsCode(string code, Guid ent_id);
        bool ExistsDescription(string description, Guid ent_id);
        bool ExistsCodeAlter(string code, long id, Guid ent_id);
        bool ExistsDescriptionAlter(string description, long id, Guid ent_id);
        IEnumerable<PerformanceLevel> GetAll(Guid EntityId);
    }
}
