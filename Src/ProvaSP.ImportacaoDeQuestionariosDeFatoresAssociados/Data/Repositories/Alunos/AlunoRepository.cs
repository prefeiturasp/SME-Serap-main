using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Data.Repositories.Alunos.Dtos;
using ImportacaoDeQuestionariosSME.Domain.Alunos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.Alunos
{
    public class AlunoRepository : BaseRepository, IAlunoRepository
    {
        public AlunoRepository()
            : base()
        {
        }

        public async Task<IEnumerable<MatriculaAnoEscolarDto>> GetAnoEscolarDoAlunoAsync(string edicao)
        {
            var param = new { edicao };

            var query = $@"SELECT 
	                        alu_matricula AS Matricula,
	                        MAX(AnoEscolar) AS AnoEscolar
                        FROM  Aluno al_provasp (NOLOCK)
                        WHERE
	                        al_provasp.Edicao = @edicao
                        GROUP BY alu_matricula";
            return await _dapperContext.QueryAsync<MatriculaAnoEscolarDto>(query, param);
        }

        public async Task<int> GetAnoescolarDoAlunoAsync(string edicao, int matricula)
        {
            var param = new { edicao, matricula };

            var query = $@"SELECT 
	                        alu_matricula AS Matricula,
	                        MAX(AnoEscolar) AS AnoEscolar
                        FROM  Aluno al_provasp (NOLOCK)
                        WHERE
	                        al_provasp.Edicao = @edicao
                        GROUP BY alu_matricula";
            return await _dapperContext.QuerySingleOrDefaultAsync<int>(query, param);
        }
    }
}