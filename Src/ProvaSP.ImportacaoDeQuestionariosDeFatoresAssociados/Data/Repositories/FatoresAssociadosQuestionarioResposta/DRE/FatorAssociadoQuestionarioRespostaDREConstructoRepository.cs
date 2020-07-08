using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.DRE;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionarioResposta.DRE
{
    public class FatorAssociadoQuestionarioRespostaDREConstructoRepository : BaseInsertRepository<FatorAssociadoQuestionarioRespostaDREConstructo>, IFatorAssociadoQuestionarioRespostaDREConstructoRepository
    {
        public FatorAssociadoQuestionarioRespostaDREConstructoRepository()
            : base()
        {
        }

        public async Task InsertAsync(IEnumerable<FatorAssociadoQuestionarioRespostaDREConstructo> entities)
        {
            if (entities is null || !entities.Any()) return;

            var sqls = GetSqlsInPages(entities);
            foreach (var sql in sqls)
            {
                await _dapperContext.ExecuteAsync(sql);
            }
        }

        protected override string GetQueryInsert()
            => @"INSERT INTO FatorAssociadoQuestionarioRespostaDREConstructo
               (FatorAssociadoQuestionarioRespostaDREId, ConstructoId)
               VALUES ";

        protected override string GetValuesQueryForEntity(FatorAssociadoQuestionarioRespostaDREConstructo entity)
            => $"({entity.FatorAssociadoQuestionarioRespostaDREId}, {entity.ConstructoId})";
    }
}