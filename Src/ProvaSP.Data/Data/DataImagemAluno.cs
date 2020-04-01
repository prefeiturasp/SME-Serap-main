using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ProvaSP.Data
{
    public static class DataImagemAluno
    {
        public static List<ImagemAluno> SelecionarPorEdicaoAreaConhecimentoAluno(string Edicao, byte AreaConhecimentoId, string alu_matricula)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.Query<ImagemAluno>(
                    sql: @"
                            SELECT 
	                            I.Edicao, I.AreaConhecimentoID, I.esc_codigo, I.alu_matricula, I.alu_nome, I.questao, I.pagina, I.caminho, R.REDQ1, R.REDQ2, R.REDQ3, R.REDQ4, R.REDQ5
                            FROM 
	                            ImagemAluno I WITH (NOLOCK)
	                            LEFT JOIN ResultadoAluno R WITH (NOLOCK)
		                            ON R.Edicao = I.Edicao
		                            AND R.AreaConhecimentoID = I.AreaConhecimentoID
		                            AND I.esc_codigo = R.esc_codigo
		                            AND R.alu_matricula = I.alu_matricula
                            WHERE
	                            I.Edicao = @Edicao
                                AND I.AreaConhecimentoId = @AreaConhecimentoId
                                AND I.alu_matricula = @alu_matricula"
                    , param: new
                    {
                        Edicao,
                        AreaConhecimentoId,
                        alu_matricula
                    }).ToList<ImagemAluno>();
            }

        }
    }
}
