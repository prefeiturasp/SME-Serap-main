using GestaoAvaliacao.MongoEntities;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface IPerformanceItemRepository
	{
		Task<PerformanceItem> Save(PerformanceItem entity);
		Task<PerformanceItem> Get(PerformanceItem entity);
	}
}
