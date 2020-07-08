using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.Escolas;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionarioResposta.Escolas
{
    public class FatorAssociadoQuestionarioRespostaEscolaRepository : BaseInsertRepository<FatorAssociadoQuestionarioRespostaEscola>, IFatorAssociadoQuestionarioRespostaEscolaRepository
    {
        public FatorAssociadoQuestionarioRespostaEscolaRepository()
            : base()
        {
        }

        public async Task InsertAsync(IEnumerable<FatorAssociadoQuestionarioRespostaEscola> fatoresAssociadoQuestionarioRespostaEscola)
        {
            if (fatoresAssociadoQuestionarioRespostaEscola is null || !fatoresAssociadoQuestionarioRespostaEscola.Any()) return;

            var sqls = GetSqlsInPages(fatoresAssociadoQuestionarioRespostaEscola);
            foreach (var sql in sqls)
            {
                await _dapperContext.ExecuteAsync(sql);
            }
        }

        public async Task<int> GetNextPk()
        {
            var query = "SELECT MAX(FatorAssociadoQuestionarioRespostaEscolaId) FROM FatorAssociadoQuestionarioRespostaEscola (NOLOCK)";
            var result = await _dapperContext.QuerySingleOrDefaultAsync<int>(query);
            return result + 1;
        }

        protected override string GetQueryInsert()
            => @"INSERT INTO FatorAssociadoQuestionarioRespostaEscola
               (FatorAssociadoQuestionarioRespostaEscolaId, Edicao, CicloId, AnoEscolar, FatorAssociadoQuestionarioId, VariavelId, ItemId, uad_sigla, esc_codigo, VariavelDescricao, ItemDescricao, Valor)
               VALUES ";

        protected override string GetValuesQueryForEntity(FatorAssociadoQuestionarioRespostaEscola entity)
            => $@"({entity.Id}, '{entity.Edicao}', {entity.CicloId}, {entity.AnoEscolar}, {entity.FatorAssociadoQuestionarioId}, '{entity.VariavelId}', {entity.ItemId}, '{entity.UadSigla}', '{entity.EscCodigo}', '{entity.VariavelDescricao}', '{entity.ItemDescricao}', {entity.Valor.ToString().Replace(",", ".")})";
    }
}