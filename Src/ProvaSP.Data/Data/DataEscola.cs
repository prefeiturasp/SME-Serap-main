using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
namespace ProvaSP.Data
{
    public static class DataEscola
    {
        public static Escola RecuperarEscola(string esc_codigo)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                try
                {
                    var escola=conn.Query<Escola>(
                                        sql: @"
                                            SELECT esc_codigo, uad_codigo, esc_nome
                                            FROM Escola WITH (NOLOCK)
                                            WHERE esc_codigo = @esc_codigo",
                                        param: new
                                        {
                                            esc_codigo = new DbString() { Value = esc_codigo, IsAnsi = true, Length = 20 }
                                        }).First();
                    return escola;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            
        }

        public static string RecuperarCodigoEscolaComBaseNaTurma(int tur_id, SqlTransaction dbContextTransaction, SqlConnection conn)
        {
            return conn.ExecuteScalar<string>(
                                        sql: @"
                                            SELECT e.esc_codigo
                                            FROM Escola e WITH (NOLOCK)
                                            JOIN Turma t WITH (NOLOCK) ON e.esc_codigo = t.esc_codigo
                                            WHERE t.tur_id = @tur_id",
                                        param: new
                                        {
                                            tur_id = tur_id
                                        },
                                        transaction: dbContextTransaction);
        }

        public static string RecuperarPrimeiroCodigoEscola(string Edicao, string usu_id)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.ExecuteScalar<string>(
                                        sql: @"
                                            SELECT TOP 1 esc_codigo
                                            FROM PessoaPerfil WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND usu_id=@usu_id",
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            usu_id = new DbString() { Value = usu_id, IsAnsi = true, Length = 40 }
                                        });
            }

            
        }

        public static string RecuperarCodigoEscolaComBaseNoPerfilDaPessoa(string Edicao, string usu_id, int PerfilID, SqlTransaction dbContextTransaction, SqlConnection conn)
        {
            return conn.ExecuteScalar<string>(
                                        sql: @"
                                            SELECT esc_codigo
                                            FROM PessoaPerfil WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND usu_id=@usu_id AND PerfilID=@PerfilID",
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            usu_id = new DbString() { Value = usu_id, IsAnsi = true, Length = 40 },
                                            PerfilID = PerfilID
                                        },
                                        transaction: dbContextTransaction);
        }

        public static string RecuperarCodigoEscola(string gru_id, string usu_id)
        {
            using (var conn = new SqlConnection(StringsConexao.CoreSSO))
            {
                return conn.ExecuteScalar<string>(
                                        sql: @"
                                                SELECT TOP 1 uad_codigo
                                                FROM SYS_UnidadeAdministrativa
                                                WHERE uad_id IN(
                                                        SELECT TOP 1 uad_id
                                                        FROM SYS_UsuarioGrupoUA
                                                        WHERE gru_id=@gru_id AND usu_id=@usu_id
                                                    )
                                            ",
                                        param: new
                                        {
                                            gru_id = new DbString() { Value = gru_id, IsAnsi = true, Length = 40 },
                                            usu_id = new DbString() { Value = usu_id, IsAnsi = true, Length = 40 }
                                        });
            }   
        }

    }
}
