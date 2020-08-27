using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.SME;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionarioResposta.SME
{
    public class FatorAssociadoQuestionarioRespostaSMERepository : BaseInsertRepository<FatorAssociadoQuestionarioRespostaSME>, IFatorAssociadoQuestionarioRespostaSMERepository
    {
        public FatorAssociadoQuestionarioRespostaSMERepository()
            : base()
        {
        }

        public async Task InsertAsync(IEnumerable<FatorAssociadoQuestionarioRespostaSME> fatoresAssociadoQuestionarioRespostaSME)
        {
            if (fatoresAssociadoQuestionarioRespostaSME is null || !fatoresAssociadoQuestionarioRespostaSME.Any()) return;

            var sqls = GetSqlsInPages(fatoresAssociadoQuestionarioRespostaSME);
            foreach (var sql in sqls)
            {
                await _dapperContext.ExecuteAsync(sql);
            }
        }

        public async Task<int> GetNextPk()
        {
            var query = "SELECT MAX(FatorAssociadoQuestionarioRespostaSMEId) FROM FatorAssociadoQuestionarioRespostaSME (NOLOCK)";
            var result = await _dapperContext.QuerySingleOrDefaultAsync<int>(query);
            return result + 1;
        }

        protected override string GetQueryInsert()
            => @"INSERT INTO FatorAssociadoQuestionarioRespostaSME
               (FatorAssociadoQuestionarioRespostaSMEId, Edicao, CicloId, AnoEscolar, FatorAssociadoQuestionarioId, VariavelId, ItemId, VariavelDescricao, ItemDescricao, Valor)
               VALUES ";

        protected override string GetValuesQueryForEntity(FatorAssociadoQuestionarioRespostaSME entity)
            => $@"({entity.Id}, '{entity.Edicao}', {entity.CicloId}, {entity.AnoEscolar}, {entity.FatorAssociadoQuestionarioId}, '{entity.VariavelId}', {entity.ItemId}, '{entity.VariavelDescricao}', '{entity.ItemDescricao}', {entity.Valor.ToString().Replace(",", ".")})";
    }
}