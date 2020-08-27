using Dapper;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.Abstractions;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain.Alunos;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain.Matriculas;
using GestaoAvaliacao.Repository.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.Matriculas
{
    internal class MatriculaRepository : BaseRepository, IMatriculaRepository
	{
        public async Task<IEnumerable<Matricula>> GetMatriculasAtivasDoAluno(long alunoId, int ano)
        {
            var query = @"SELECT
							mtr.alu_id AS AlunoId,
							mtr.mtu_id AS MtuId,
							esc.esc_id AS EscolaId,
							esc.esc_nome AS NomeDaEscola,
							esc.uad_idSuperiorGestao AS DreId,
							tur.tur_codigo AS Turma,
							tur.tur_id AS TurmaId,
							mtr.mtu_dataMatricula AS DataDaMatricula,
							mtr.mtu_dataCriacao AS DataDeCriacao,
							mtr.mtu_dataAlteracao AS DataDeAlteracao,
							mtr.mtu_numeroChamada AS NumeroDeChamada
						FROM
							SGP_MTR_MatriculaTurma mtr (NOLOCK)
						INNER JOIN
							SGP_TUR_Turma tur (NOLOCK)
							ON mtr.tur_id = tur.tur_id
						INNER JOIN
							SGP_ESC_Escola esc (NOLOCK)
							ON tur.esc_id = esc.esc_id
						WHERE
							mtr.alu_id = @alunoId
							AND mtr.mtu_dataSaida IS NULL
							AND YEAR(mtr.mtu_dataMatricula) >= @ano
							AND mtr.mtu_situacao = @state;";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return await cn.QueryAsync<Matricula>(query, new { state = (byte)1, alunoId, ano });
            }
        }

		public async Task UpdateNumeroDeChamadaAsync(Matricula matricula, int numeroDeChamada)
        {
			var query = @"UPDATE SGP_MTR_MatriculaTurma
						  SET 
							mtu_numeroChamada = @numeroDeChamada
					      WHERE
							alu_id = @alunoId
							AND mtu_id = @mtuId";

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				await cn.ExecuteAsync(query, new { numeroDeChamada, alunoId = matricula.AlunoId, mtuId = matricula.MtuId });
			}
		}

		public async Task DeleteMatricula(Matricula matricula)
        {
			var query = @"UPDATE SGP_MTR_MatriculaTurma
						  SET 
							mtu_situacao = @state, 
							mtu_dataSaida = GETDATE()
					      WHERE
							alu_id = @alunoId
							AND mtu_id = @mtuId";

			using (IDbConnection cn = Connection)
            {
				cn.Open();
				await cn.ExecuteAsync(query, new { state = 3, alunoId = matricula.AlunoId, mtuId = matricula.MtuId });
			}
		}
    }
}