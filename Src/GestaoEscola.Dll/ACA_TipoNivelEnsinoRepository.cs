using Dapper;
using GestaoEscolar.Entities;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository.Context;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GestaoEscolar.Repository
{
    public class ACA_TipoNivelEnsinoRepository : ConnectionReadOnly, IACA_TipoNivelEnsinoRepository
    {
        #region Read

        public IEnumerable<ACA_TipoNivelEnsino> Load()
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT tne_id, tne_nome " +
                           "FROM ACA_TipoNivelEnsino WITH (NOLOCK)";

                var result = cn.Query<ACA_TipoNivelEnsino>(sql);

                return result;
            }
        }

        public ACA_TipoNivelEnsino Get(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT tne_id, tne_nome " +
                           "FROM ACA_TipoNivelEnsino WITH (NOLOCK) " +
                           "WHERE tne_id = @id";

                var result = cn.Query<ACA_TipoNivelEnsino>(sql, new { id = id }).FirstOrDefault();

                return result;
            }
        }

        #endregion
    }
}
