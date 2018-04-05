using GestaoAvaliacao.MongoEntities;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestTemplateRepository
	{
		Task<TestTemplate> GetEntity(TestTemplate entity);
		Task<TestTemplate> Insert(TestTemplate entity);
		Task<long> Count(TestTemplate entity);
		Task<TestTemplate> Replace(TestTemplate entity);
		Task<bool> Delete(TestTemplate entity);
	}
}
