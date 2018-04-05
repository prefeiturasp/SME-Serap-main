using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IPerformanceLevelBusiness
    {
        PerformanceLevel Save(PerformanceLevel entity, Guid ent_id);
        PerformanceLevel Update(long id, PerformanceLevel entity, Guid ent_id);
        PerformanceLevel Get(long id);
        PerformanceLevel Delete(long id);
        IEnumerable<PerformanceLevel> Load(ref Pager pager, Guid EntityId);
        IEnumerable<PerformanceLevel> LoadLevels(Guid EntityId);
        IEnumerable<PerformanceLevel> Search(string search, ref Pager pager, Guid EntityId);
        IEnumerable<PerformanceLevel> GetAll(Guid EntityId);
    }
}
