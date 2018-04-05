using GestaoAvaliacao.MongoEntities;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IPerformanceItemBusiness
	{
		Task<PerformanceItem> Save(PerformanceItem entity);
		Task<PerformanceItem> Get(PerformanceItem entity);
	}
}
