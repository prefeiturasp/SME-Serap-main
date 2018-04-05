using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class PerformanceItemBusiness : IPerformanceItemBusiness
	{
		#region Dependences
		readonly IPerformanceItemRepository performanceItemRepository;
		#endregion

		public PerformanceItemBusiness(IPerformanceItemRepository performanceItemRepository)
		{
			this.performanceItemRepository = performanceItemRepository;
		}

		public async Task<PerformanceItem> Save(PerformanceItem entity)
		{
			return await performanceItemRepository.Save(entity);
		}

		public async Task<PerformanceItem> Get(PerformanceItem entity)
		{
			return await performanceItemRepository.Get(entity);
		}
	}
}
