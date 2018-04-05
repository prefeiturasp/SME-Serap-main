using Dapper;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository.Context;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GestaoEscolar.Repository
{
    public class TUR_TurmaTipoCurriculoPeriodoRepository : ConnectionReadOnly, ITUR_TurmaTipoCurriculoPeriodoRepository
	{
		public IEnumerable<int> GetYearsBySchool(int esc_id)
		{
			var sql = new StringBuilder();
			sql.AppendLine("SELECT distinct crp_ordem");
			sql.AppendLine("FROM TUR_TurmaTipoCurriculoPeriodo");
			sql.AppendLine("WHERE esc_id = @esc_id");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<int>(sql.ToString(), new { esc_id = esc_id });
			}

		}
	}
}
