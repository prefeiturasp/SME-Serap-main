using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface ISectionTestStatsBusiness
	{
		Task<SectionTestStats> Save(SectionTestStats entity, Guid ent_id);
		Task<SectionTestStats> GetByTest(long test_id, long tur_id);
        Task<List<SectionTestStats>> GetByTest(long test_id);
		Task<List<SectionTestStatsGroupDTO>> GetGrouped(long test_id, IEnumerable<long> turmas);
	}
}
