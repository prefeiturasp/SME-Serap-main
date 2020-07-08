using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.ImagensAlunos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.ImagensAlunos
{
    public class ImagemAlunoRepository : BaseInsertRepository<ImagemAluno>, IImagemAlunoRepository
    {
        public async Task InsertAsync(IEnumerable<ImagemAluno> imagensAlunos)
        {
            if (imagensAlunos is null || !imagensAlunos.Any()) return;

            var sqls = GetSqlsInPages(imagensAlunos);
            foreach (var sql in sqls)
            {
                await _dapperContext.ExecuteAsync(sql);
            }
        }

        protected override string GetQueryInsert()
            => @"INSERT INTO ImagemAluno
               (Edicao, AreaConhecimentoID, esc_codigo, alu_matricula, questao, pagina, alu_nome, caminho)
               VALUES ";

        protected override string GetValuesQueryForEntity(ImagemAluno entity)
            => $@"('{entity.Edicao}', {entity.AreaConhecimentoId}, '{entity.EscCodigo}', '{entity.AluMatricula}', '{entity.Questao}', {entity.Pagina}, '{entity.AluNome}', '{entity.Caminho}')";
    }
}