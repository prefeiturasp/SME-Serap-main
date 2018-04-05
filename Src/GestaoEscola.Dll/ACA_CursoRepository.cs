using Dapper;
using GestaoEscolar.Entities;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GestaoEscolar.Repository
{
    public class ACA_CursoRepository : ConnectionReadOnly, IACA_CursoRepository
	{
		#region Read

		public IEnumerable<ACA_Curso> Load(Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();

                var sql = @"SELECT cur_id, cur_nome " +
                           "FROM ACA_Curso WITH (NOLOCK) " +
                           "WHERE cur_situacao = @state " +
                           "AND ent_id = @entityid";

                var result = cn.Query<ACA_Curso>(sql, new { state = 1, entityid = EntityId });

                return result;
			}
		}

        public IEnumerable<ACA_Curso> LoadByTipoNivelEnsino(Guid EntityId, int tne_id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT cur_id, cur_nome " +
                           "FROM ACA_Curso WITH (NOLOCK) " +
                           "WHERE cur_situacao = @state " +
                           "AND ent_id = @entityid " +
                           "AND tne_id = @tneid";

                var result = cn.Query<ACA_Curso>(sql, new { state = 1, entityid = EntityId, tneid = tne_id });

                return result;
            }
        }

        public IEnumerable<ACA_Curso> LoadByNivelEnsinoModality(Guid EntityId, int tne_id, int tme_id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT cur_id, cur_nome, tme_id " +
                           "FROM ACA_Curso WITH (NOLOCK) " +
                           "WHERE cur_situacao = @state " +
                           "AND ent_id = @entityid " +
                           "AND tne_id = @tneid " +
                           "AND tme_id = @tmeid";

                var result = cn.Query<ACA_Curso>(sql, new { state = 1, entityid = EntityId, tneid = tne_id, tmeid = tme_id });

                return result;
            }
        }

        public ACA_Curso Get(long id, Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
                var sql = @"SELECT cur_id, cur_nome " +
                           "FROM ACA_Curso WITH (NOLOCK) " +
                           "WHERE cur_id = @id " +
                           "AND ent_id = @entityid";

                var result = cn.Query<ACA_Curso>(sql, new { id = id, entityid = EntityId }).FirstOrDefault();

                return result;
			}
		}

		#endregion
	}
}
