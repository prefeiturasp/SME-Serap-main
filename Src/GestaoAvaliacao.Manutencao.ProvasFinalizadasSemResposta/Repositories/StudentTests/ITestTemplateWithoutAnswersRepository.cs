using GestaoAvaliacao.MongoEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.StudentTests
{
    public interface ITestTemplateWithoutAnswersRepository
    {
        Task<List<TestTemplate>> GetAsync(IEnumerable<long> testIds);
    }
}