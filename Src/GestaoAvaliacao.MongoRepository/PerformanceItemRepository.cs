using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace GestaoAvaliacao.MongoRepository
{
    public class PerformanceItemRepository : BaseRepository<PerformanceItem>, IPerformanceItemRepository
	{
		public async Task<PerformanceItem> Get(PerformanceItem entity)
		{
			var filter = Builders<PerformanceItem>.Filter.Eq("_id", entity._id);
			return await base.FindOne(filter);
		}

		public async Task<PerformanceItem> Save(PerformanceItem entity)
		{
			if (await base.Count(entity) > 0)
			{
				return await base.Replace(entity);
			}
			else
				return await base.Insert(entity);
		}
	}
}
