using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociados;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociados
{
    public class FatorAssociadoRepository : BaseInsertRepository<FatorAssociado>, IFatorAssociadoRepository
    {
        public async Task InsertAsync(IEnumerable<FatorAssociado> fatoresAssociados)
        {
            if (fatoresAssociados is null || !fatoresAssociados.Any()) return;

            var sqls = GetSqlsInPages(fatoresAssociados);
            foreach (var sql in sqls)
            {
                await _dapperContext.ExecuteAsync(sql);
            }
        }

        protected override string GetQueryInsert()
            => @"INSERT INTO FatorAssociado
               (ConstructoId, AreaConhecimentoId, Pontos)
               VALUES ";

        protected override string GetValuesQueryForEntity(FatorAssociado entity)
            => $"({entity.ConstructoId}, {entity.AreaConhecimentoId}, '{entity.Pontos}')";
    }
}