using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace GestaoAvaliacao.MongoRepository
{
    public class TestTemplateRepository : BaseRepository<TestTemplate>, ITestTemplateRepository
	{
		public async Task<TestTemplate> GetByTest(long id)
		{
			var filter = Builders<TestTemplate>.Filter.Eq("test_id", id);

			return await base.FindOne(filter);
		}
	}
}
