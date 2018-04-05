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
    public class ACA_CurriculoPeriodoRepository : ConnectionReadOnly, IACA_CurriculoPeriodoRepository
	{
		#region Read

		public IEnumerable<ACA_CurriculoPeriodo> Load(int cur_id, Guid ent_id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();

                var sql = @"SELECT CP.crp_id, CP.crp_ordem, CP.crp_descricao, CP.tcp_id " +
						   "FROM ACA_CurriculoPeriodo CP WITH (NOLOCK) " +
						   "INNER JOIN ACA_Curso C WITH (NOLOCK) ON C.cur_id = CP.cur_id " +
						   "WHERE CP.cur_id = @curid " +
						   "AND C.ent_id = @entityid";

				var result = cn.Query<ACA_CurriculoPeriodo>(sql, new { curid = cur_id, entityid = ent_id });

				return result;
			}
		}

		public IEnumerable<ACA_CurriculoPeriodo> GetCurriculumGradeByesc_id(int esc_id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var sql = @"SELECT cpr.crp_ordem, cpr.crp_descricao " +
							"FROM ESC_Escola esc " +
							"INNER JOIN TUR_Turma tur ON esc.esc_id = tur.esc_id " +
							"INNER JOIN TUR_TurmaCurriculo tcr ON tur.tur_id = tcr.tur_id " +
							"INNER JOIN ACA_CurriculoPeriodo cpr ON cpr.cur_id = tcr.cur_id " +
							"and cpr.crr_id = tcr.crr_id " +
							"and cpr.crp_id = tcr.crp_id " +
							"WHERE esc.esc_id = @esc_id " +
							"AND tur.tur_tipo = @tipo " +
							"AND tcr.tcr_situacao = @state " +
							"AND esc.esc_situacao = @state " +
							"AND tur.tur_situacao = @state " +
							"AND cpr.crp_situacao = @state " +
							"GROUP BY cpr.crp_ordem, cpr.crp_descricao ";

				var result = cn.Query<ACA_CurriculoPeriodo>(sql, new { state = 1, tipo = 1, esc_id = esc_id });

				return result;
			}
		}

		public ACA_CurriculoPeriodo Get(long id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT crp_id, crp_ordem, crp_descricao " +
						   "FROM ACA_CurriculoPeriodo WITH (NOLOCK) " +
						   "WHERE crp_id = @id";

				var result = cn.Query<ACA_CurriculoPeriodo>(sql, new { id = id }).FirstOrDefault();

				return result;
			}
		}

		#endregion
	}
}
