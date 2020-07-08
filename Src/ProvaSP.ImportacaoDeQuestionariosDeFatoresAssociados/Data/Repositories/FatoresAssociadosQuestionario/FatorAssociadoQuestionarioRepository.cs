using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionario;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionario
{
    public class FatorAssociadoQuestionarioRepository : BaseInsertRepository<FatorAssociadoQuestionario>, IFatorAssociadoQuestionarioRepository
    {
        public FatorAssociadoQuestionarioRepository()
            : base()
        {
        }

        public async Task InsertAsync(IEnumerable<FatorAssociadoQuestionario> fatoresAssociadosQuestionarios)
        {
            if (fatoresAssociadosQuestionarios is null || !fatoresAssociadosQuestionarios.Any()) return;

            var sqls = GetSqlsInPages(fatoresAssociadosQuestionarios);
            foreach (var sql in sqls)
            {
                await _dapperContext.ExecuteAsync(sql);
            }
        }

        protected override string GetQueryInsert()
            => @"INSERT INTO FatorAssociadoQuestionario
               (FatorAssociadoQuestionarioId, Edicao, Nome)
               VALUES ";

        protected override string GetValuesQueryForEntity(FatorAssociadoQuestionario fatorAssociadoQuestinarioResposta)
            => $@"({fatorAssociadoQuestinarioResposta.FatorAssociadoQuestionarioId}, '{fatorAssociadoQuestinarioResposta.Edicao}', '{fatorAssociadoQuestinarioResposta.Nome}')";
    }
}