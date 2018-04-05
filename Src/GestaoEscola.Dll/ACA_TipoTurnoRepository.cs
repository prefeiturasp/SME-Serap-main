using Dapper;
using GestaoEscolar.Entities;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository.Context;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoEscolar.Repository
{
    public class ACA_TipoTurnoRepository : ConnectionReadOnly, IACA_TipoTurnoRepository
	{
		public IEnumerable<ACA_TipoTurno> Load(int esc_id)
		{
			var sql = new StringBuilder("SELECT DISTINCT ttn.ttn_id, ttn.ttn_nome ");
			sql.Append("FROM ACA_TipoTurno ttn ");
			sql.Append("INNER JOIN TUR_Turma t ON t.ttn_id = ttn.ttn_id ");
			sql.Append("WHERE ttn_situacao = @state AND t.tur_situacao = @state AND t.esc_id = @esc_id ");
			sql.Append("ORDER BY ttn.ttn_nome ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<ACA_TipoTurno>(sql.ToString(), new { state = (byte)1, esc_id = esc_id });
			}
		}

        public ACA_TipoTurno Get(int id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT [ttn_id]
                              ,[ttn_nome]
                              ,[ttn_situacao]
                              ,[ttn_dataCriacao]
                              ,[ttn_dataAlteracao] " +
                           "FROM [ACA_TipoTurno] " +
                           "WHERE [ttn_id] = @id AND [ttn_situacao] = @state";

                var result = cn.Query<ACA_TipoTurno>(sql, new { state = (byte)1, id = id }).FirstOrDefault();

                return result;
            }
        }
	}
}
