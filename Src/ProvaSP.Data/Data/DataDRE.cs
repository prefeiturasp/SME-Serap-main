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
    public static class DataDRE
    {
        public static string RecuperarNome(string uad_sigla)
        {
            if (uad_sigla == "CS")
                return "CAPELA DO SOCORRO";
            else if (uad_sigla == "CL")
                return "CAMPO LIMPO";
            else if (uad_sigla == "IQ")
                return "ITAQUERA";
            else if (uad_sigla == "FO")
                return "FREGUESIA/BRASILANDIA";
            else if (uad_sigla == "SM")
                return "SAO MATEUS";
            else if (uad_sigla == "G")
                return "GUAIANASES";
            else if (uad_sigla == "PE")
                return "PENHA";
            else if (uad_sigla == "SA")
                return "SANTO AMARO";
            else if (uad_sigla == "PJ")
                return "PIRITUBA/JARAGUA";
            else if (uad_sigla == "MP")
                return "SAO MIGUEL";
            else if (uad_sigla == "BT")
                return "BUTANTA";
            else if (uad_sigla == "IP")
                return "IPIRANGA";
            else if (uad_sigla == "JT")
                return "JACANA/TREMEMBE";
            else
                return "";
        }

        public static string RecuperarCodigoDREComBaseNoPerfilDaPessoa(string Edicao, string usu_id)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.ExecuteScalar<string>(
                                        sql: @"
                                            SELECT uad_sigla
                                            FROM PessoaPerfil WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND usu_id=@usu_id AND PerfilID=@PerfilID",
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            usu_id = new DbString() { Value = usu_id, IsAnsi = true, Length = 40 },
                                            PerfilID = 1 
                                        });
            }
            
        }

        public static string RecuperarCodigoDRE(string gru_id, string usu_id)
        {
            using (var conn = new SqlConnection(StringsConexao.CoreSSO))
            {
                return conn.ExecuteScalar<string>(
                                        sql: @"
                                                SELECT TOP 1 uad_sigla
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

        public static string RecuperarCodigoDRE(string esc_codigo)
        {
            using (var conn = new SqlConnection(StringsConexao.GestaoEscolar))
            {
                return conn.ExecuteScalar<string>(
                                        sql: @"
                                                SELECT TOP 1 uad_sigla
                                                FROM SYS_UnidadeAdministrativa
                                                WHERE uad_id IN (SELECT uad_idSuperiorGestao FROM ESC_Escola WHERE esc_codigo=@esc_codigo)
                                            ",
                                        param: new
                                        {
                                            esc_codigo = new DbString() { Value = esc_codigo, IsAnsi = true, Length = 50 }
                                        });
            }
        }
    }
}

