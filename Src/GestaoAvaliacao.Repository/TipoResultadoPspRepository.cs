using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GestaoAvaliacao.Repository
{
    public class TipoResultadoPspRepository : ConnectionReadOnly, ITipoResultadoPspRepository
    {
        public TipoResultadoPspRepository()
        {

        }

        public IEnumerable<TipoResultadoPsp> ObterTodosAtivos()
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"select 
								[Id]
								,[Codigo]
								,[Nome]
								,[NomeTabelaProvaSp]
								,[CreateDate]
								,[UpdateDate]
								,[State]
								from TipoResultadoPsp
								where [State] = 1";

                var result = cn.Query<TipoResultadoPsp>(sql);
                return result;
            }
        }

        public TipoResultadoPsp ObterPorCodigo(int codigoTipoResultado)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"select 
								[Id]
								,[Codigo]
								,[Nome]
								,[NomeTabelaProvaSp]
								,[CreateDate]
								,[UpdateDate]
								,[State]
								from TipoResultadoPsp
								where [State] = 1
                                and Codigo = @codigoTipoResultado";

                var result = cn.Query<TipoResultadoPsp>(sql, new { codigoTipoResultado }).FirstOrDefault();
                return result;
            }
        }

    }
}
