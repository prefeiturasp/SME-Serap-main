using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.Questionarios.Itens;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.Questionarios.Itens
{
    public class QuestionarioItemRepository : BaseRepository, IQuestionarioItemRepository
    {
        public QuestionarioItemRepository()
            : base()
        {
        }

        public async Task<IEnumerable<QuestionarioItem>> GetAsync(int questionarioId)
        {
            var query = @"SELECT Numero, Titulo FROM QuestionarioItem (NOLOCK)
                        WHERE QuestionarioId = @questionarioId";

            var param = new { questionarioId };
            var results = await _dapperContext.QueryAsync<QuestionarioItem>(query, param);

            foreach (var questionarioItem in results) questionarioItem.MontarQuestoes();
            return results;
        }
    }
}