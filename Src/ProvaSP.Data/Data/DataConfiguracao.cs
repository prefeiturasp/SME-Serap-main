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
    }
}
