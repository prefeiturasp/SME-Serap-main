using Dapper;
using ProvaSP.Data.Funcionalidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Data
{
    public static class DataProficiencia
    {
        public static bool AlunoPossuiProficiencia(string alu_matricula)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.ExecuteScalar<Boolean>(
                                        sql: @"
                                            IF EXISTS(SELECT * FROM ResultadoAluno WHERE alu_matricula=@alu_matricula)
                                                SELECT 1
                                            ELSE
                                                SELECT 0
                                            ",
                                        param: new
                                        {
                                            alu_matricula = new DbString() { Value = alu_matricula, IsAnsi = true, Length = 50 }
                                        });
            }
            
        }

        public static float CalcularProficienciaAtualTurma(int tur_id, int AreaConhecimentoID)
        {
            DateTime tur_dataAlteracao_ProvaSP;
            DateTime tur_dataAlteracao_SERAp;

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                tur_dataAlteracao_ProvaSP = conn.ExecuteScalar<DateTime>(
                                        sql: @"
                                            IF EXISTS(SELECT * FROM ResultadoEnturmacaoAtual WHERE tur_id=@tur_id AND AreaConhecimentoID=@AreaConhecimentoID)
                                                SELECT tur_dataAlteracao FROM ResultadoEnturmacaoAtual WHERE tur_id=@tur_id AND AreaConhecimentoID=@AreaConhecimentoID
                                            ELSE
                                                SELECT tur_dataAlteracao='1900-01-01'
                                            ",
                                        param: new
                                        {
                                            tur_id = tur_id,
                                            AreaConhecimentoID = AreaConhecimentoID
                                        });
            }

            using (var conn = new SqlConnection(StringsConexao.GestaoEscolar))
            {
                tur_dataAlteracao_SERAp = conn.ExecuteScalar<DateTime>(
                                        sql: @"
                                            SELECT tur_dataAlteracao FROM TUR_Turma WHERE tur_id=@tur_id
                                            ",
                                        param: new
                                        {
                                            tur_id = tur_id
                                        });
            }

            float proficiencia = 0;
            if (tur_dataAlteracao_SERAp == tur_dataAlteracao_ProvaSP)
            {
                using (var conn = new SqlConnection(StringsConexao.ProvaSP))
                {
                    proficiencia = conn.ExecuteScalar<float>(
                                            sql: @"
                                                SELECT Valor FROM ResultadoEnturmacaoAtual WHERE tur_id=@tur_id AND AreaConhecimentoID=@AreaConhecimentoID
                                            ",
                                            param: new
                                            {
                                                tur_id = tur_id,
                                                AreaConhecimentoID = AreaConhecimentoID
                                            });
                }
            }
            else
            {
                var lista_alu_matricula = new List<string>();
                using (var conn = new SqlConnection(StringsConexao.GestaoEscolar))
                {
                    lista_alu_matricula = conn.Query<string>(
                                        sql: @"
                                            SELECT a.alu_matricula
                                            FROM MTR_MatriculaTurma mt
                                            JOIN ACA_Aluno a ON a.alu_id=mt.alu_id
                                            WHERE mt.tur_id=@tur_id
                                            ",
                                        param: new
                                        {
                                            tur_id = tur_id,
                                            AreaConhecimentoID = AreaConhecimentoID
                                        }).ToList<string>();
                }

                int totalEncontrados = 0;
                float soma = 0;
                using (var conn = new SqlConnection(StringsConexao.ProvaSP))
                {
                    foreach (string alu_matricula in lista_alu_matricula)
                    {
                        var valor = conn.ExecuteScalar<string>(
                                            sql: @"
                                            IF EXISTS(SELECT TOP 1 Valor FROM ResultadoAluno WHERE AreaConhecimentoID=@AreaConhecimentoID AND alu_matricula=@alu_matricula)
                                                SELECT TOP 1 Valor
                                                FROM ResultadoAluno
                                                WHERE AreaConhecimentoID=@AreaConhecimentoID AND alu_matricula=@alu_matricula
                                                ORDER BY Edicao DESC
                                            ELSE
                                                SELECT ''
                                            ",
                                            param: new
                                            {
                                                alu_matricula = alu_matricula,
                                                AreaConhecimentoID = AreaConhecimentoID
                                            });
                        if (!string.IsNullOrEmpty(valor))
                        {
                            totalEncontrados++;
                            soma += float.Parse(valor);
                        }
                    }

                    if (totalEncontrados > 0)
                    {
                        proficiencia = soma / totalEncontrados;
                    }

                    conn.Execute(
                                    sql: @"
                                    UPDATE ResultadoEnturmacaoAtual SET Valor=@Valor WHERE AreaConhecimentoID=@AreaConhecimentoID AND tur_id=@tur_id
                                    ",
                                    param: new
                                    {
                                        tur_id = tur_id,
                                        AreaConhecimentoID = AreaConhecimentoID,
                                        Valor = proficiencia
                                    });
                }


            }

            
            return proficiencia;
        }
    }
}

