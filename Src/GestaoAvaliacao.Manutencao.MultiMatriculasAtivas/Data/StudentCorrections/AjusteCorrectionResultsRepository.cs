using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.StudentCorrections
{
    internal class AjusteCorrectionResultsRepository : BaseRepository<CorrectionResults>, IAjusteCorrectionResultsRepository
    {
        public async Task DeleteCorrectionResults(long testId, long turmaId, Guid entidadeId)
        {
            var entity = new CorrectionResults(entidadeId, testId, turmaId);
            await Delete(entity);
        }
    }
}
