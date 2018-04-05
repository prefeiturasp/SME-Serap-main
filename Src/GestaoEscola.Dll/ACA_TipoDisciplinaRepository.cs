using Dapper;
using GestaoEscolar.Entities;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository.Context;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GestaoEscolar.Repository
{
    public class ACA_TipoDisciplinaRepository : ConnectionReadOnly, IACA_TipoDisciplinaRepository
    {
        #region Read

        public IEnumerable<ACA_TipoDisciplina> Load(int typeLevelEducation)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT tds_id, tds_nome " +
                           "FROM ACA_TipoDisciplina WITH (NOLOCK) " +
                           "WHERE tne_id = @tneid";

                var result = cn.Query<ACA_TipoDisciplina>(sql, new { tneid = typeLevelEducation });

                return result;
            }
        }

        public ACA_TipoDisciplina Get(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT tds_id, tds_nome " +
                           "FROM ACA_TipoDisciplina WITH (NOLOCK) " +
                           "WHERE tds_id = @id";

                var result = cn.Query<ACA_TipoDisciplina>(sql, new { id = id }).FirstOrDefault();

                return result;
            }
        }

        #endregion
    }
}
