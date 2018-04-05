using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface ISectionTestStatsRepository
	{
		Task<SectionTestStats> GetEntity(SectionTestStats entity);
		Task<SectionTestStats> Insert(SectionTestStats entity);
		Task<long> Count(SectionTestStats entity);
		Task<SectionTestStats> Replace(SectionTestStats entity);
		Task<SectionTestStats> GetByTest(long test_id, long tur_id);      
        Task<List<SectionTestStats>> GetByTest(long test_id);
		Task<List<SectionTestStatsGroupDTO>> GetGrouped(long test_id, IEnumerable<long> turmas);
    }
}
