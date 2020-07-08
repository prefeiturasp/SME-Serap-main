using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.SME;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionarioResposta.SME
{
    public class FatorAssociadoQuestionarioRespostaSMEConstructoRepository : BaseInsertRepository<FatorAssociadoQuestionarioRespostaSMEConstructo>, IFatorAssociadoQuestionarioRespostaSMEConstructoRepository
    {
        public FatorAssociadoQuestionarioRespostaSMEConstructoRepository()
            : base()
        {
        }

        public async Task InsertAsync(IEnumerable<FatorAssociadoQuestionarioRespostaSMEConstructo> entities)
        {
            if (entities is null || !entities.Any()) return;

            var sqls = GetSqlsInPages(entities);
            foreach (var sql in sqls)
            {
                await _dapperContext.ExecuteAsync(sql);
            }
        }

        protected override string GetQueryInsert()
            => @"INSERT INTO FatorAssociadoQuestionarioRespostaSMEConstructo
               (FatorAssociadoQuestionarioRespostaSMEId, ConstructoId)
               VALUES ";

        protected override string GetValuesQueryForEntity(FatorAssociadoQuestionarioRespostaSMEConstructo entity)
            => $"({entity.FatorAssociadoQuestionarioRespostaSMEId}, {entity.ConstructoId})";
    }
}