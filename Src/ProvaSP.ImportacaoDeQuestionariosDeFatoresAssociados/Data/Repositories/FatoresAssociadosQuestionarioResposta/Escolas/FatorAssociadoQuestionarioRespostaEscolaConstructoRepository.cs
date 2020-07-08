using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.Escolas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionarioResposta.Escolas
{
    public class FatorAssociadoQuestionarioRespostaEscolaConstructoRepository : BaseInsertRepository<FatorAssociadoQuestionarioRespostaEscolaConstructo>, IFatorAssociadoQuestionarioRespostaEscolaConstructoRepository
    {
        public FatorAssociadoQuestionarioRespostaEscolaConstructoRepository()
            : base()
        {
        }

        public async Task InsertAsync(IEnumerable<FatorAssociadoQuestionarioRespostaEscolaConstructo> entities)
        {
            if (entities is null || !entities.Any()) return;

            var sqls = GetSqlsInPages(entities);
            foreach (var sql in sqls)
            {
                await _dapperContext.ExecuteAsync(sql);
            }
        }

        protected override string GetQueryInsert()
            => @"INSERT INTO FatorAssociadoQuestionarioRespostaEscolaConstructo
               (FatorAssociadoQuestionarioRespostaEscolaId, ConstructoId)
               VALUES ";

        protected override string GetValuesQueryForEntity(FatorAssociadoQuestionarioRespostaEscolaConstructo entity)
            => $"({entity.FatorAssociadoQuestionarioRespostaEscolaId}, {entity.ConstructoId})";
    }
}
