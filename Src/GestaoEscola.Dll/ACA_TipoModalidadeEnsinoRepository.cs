using Dapper;
using GestaoEscolar.Entities;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository.Context;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GestaoEscolar.Repository
{
    public class ACA_TipoModalidadeEnsinoRepository : ConnectionReadOnly, IACA_TipoModalidadeEnsinoRepository
    {
        #region Read

        public IEnumerable<ACA_TipoModalidadeEnsino> Load()
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT tme_id, tme_nome " +
                           "FROM ACA_TipoModalidadeEnsino WITH (NOLOCK)";

                var result = cn.Query<ACA_TipoModalidadeEnsino>(sql);

                return result;
            }
        }

        public ACA_TipoModalidadeEnsino Get(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT tme_id, tme_nome " +
                           "FROM ACA_TipoModalidadeEnsino WITH (NOLOCK) " +
                           "WHERE tme_id = @id";

                var result = cn.Query<ACA_TipoModalidadeEnsino>(sql, new { id = id }).FirstOrDefault();

                return result;
            }
        }

        #endregion
    }
}
