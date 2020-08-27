using Dapper;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.Abstractions;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain.Alunos;
using GestaoAvaliacao.Repository.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.Alunos
{
    internal class AlunoRepository : BaseRepository, IAlunoRepository
    {
        public async Task<IEnumerable<Aluno>> GetAlunosComMaisDeUmaMatriculaAtiva(int ano)
        {
            var query = @"SELECT
							alu.alu_id AS AlunoId,
							alu.alu_nome AS Nome,
							alu.alu_matricula AS Matricula,
							alu.ent_id AS EntidadeId
						FROM
							SGP_MTR_MatriculaTurma mtr (NOLOCK)
						INNER JOIN
							SGP_ACA_Aluno alu (NOLOCK)
							ON mtr.alu_id = alu.alu_id
						WHERE
							mtr.mtu_situacao = @state
							AND mtr.mtu_dataSaida IS NULL
							AND YEAR(mtr.mtu_dataMatricula) >= @ano
						GROUP BY
							alu.alu_id,
							alu.alu_nome,
							alu.alu_matricula,
							alu.ent_id
						HAVING COUNT(*) > 1;";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return await cn.QueryAsync<Aluno>(query, new { state = (byte)1, ano });
            }
        }
    }
}