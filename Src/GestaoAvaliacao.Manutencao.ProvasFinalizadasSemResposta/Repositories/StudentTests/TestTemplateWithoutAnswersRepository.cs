using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoRepository;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.StudentTests
{
    public class TestTemplateWithoutAnswersRepository : BaseRepository<TestTemplate>, ITestTemplateWithoutAnswersRepository
    {
        public Task<List<TestTemplate>> GetAsync(IEnumerable<long> testIds)
        {
            var filter = Builders<TestTemplate>.Filter.In(x => x.Test_Id, testIds);
            return base.Find(filter);
        }
    }
}