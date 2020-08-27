using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionarioResposta;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.DRE;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionario.DRE
{
    public class FatorAssociadoQuestionarioRespostaDRERepository : BaseInsertRepository<FatorAssociadoQuestionarioRespostaDRE>, IFatorAssociadoQuestionarioRespostaDRERepository
    {
        public FatorAssociadoQuestionarioRespostaDRERepository()
            : base()
        {
        }

        public async Task InsertAsync(IEnumerable<FatorAssociadoQuestionarioRespostaDRE> fatoresAssociadoQuestionarioRespostaDRE)
        {
            if (fatoresAssociadoQuestionarioRespostaDRE is null || !fatoresAssociadoQuestionarioRespostaDRE.Any()) return;

            var sqls = GetSqlsInPages(fatoresAssociadoQuestionarioRespostaDRE);
            foreach (var sql in sqls)
            {
                await _dapperContext.ExecuteAsync(sql);
            }
        }

        public async Task<int> GetNextPk()
        {
            var query = "SELECT MAX(FatorAssociadoQuestionarioRespostaDREId) FROM FatorAssociadoQuestionarioRespostaDRE (NOLOCK)";
            var result = await _dapperContext.QuerySingleOrDefaultAsync<int>(query);
            return result + 1;
        }

        protected override string GetQueryInsert()
            => @"INSERT INTO FatorAssociadoQuestionarioRespostaDRE
               (FatorAssociadoQuestionarioRespostaDREId, Edicao, CicloId, AnoEscolar, FatorAssociadoQuestionarioId, VariavelId, ItemId, uad_sigla, VariavelDescricao, ItemDescricao, Valor)
               VALUES ";

        protected override string GetValuesQueryForEntity(FatorAssociadoQuestionarioRespostaDRE entity)
            => $@"({entity.Id}, '{entity.Edicao}', {entity.CicloId}, {entity.AnoEscolar}, {entity.FatorAssociadoQuestionarioId}, '{entity.VariavelId}', {entity.ItemId}, '{entity.UadSigla}', '{entity.VariavelDescricao}', '{entity.ItemDescricao}', {entity.Valor.ToString().Replace(",", ".")})";
    }
}