using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ProvaSP.Data
{
    public static class DataConfiguracao
    {
        public static List<Configuracao> RetornarConfiguracao()
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.Query<Configuracao>(
                    sql: @"
                        SELECT Chave, Valor
                        FROM Configuracao
                    "
                    ).ToList<Configuracao>();
            }
            
        }

        public static bool AtualizarConfiguracao(Configuracao configuracao)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                int ret = conn.Execute("UPDATE Configuracao SET Valor = @Valor WHERE Chave = @Chave",
                            param: new
                            {
                                configuracao.Valor,
                                configuracao.Chave
                            });
                return ret > 0;
            }
        }
    }
}
