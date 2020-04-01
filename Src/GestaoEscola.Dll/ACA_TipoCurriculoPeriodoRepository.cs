using Dapper;
using GestaoEscolar.Entities;
using GestaoEscolar.Entities.DTO;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoEscolar.Repository
{
    public class ACA_TipoCurriculoPeriodoRepository : ConnectionReadOnly, IACA_TipoCurriculoPeriodoRepository
	{
		public IEnumerable<ACA_TipoCurriculoPeriodo> GetSimple(int esc_id)
		{
			var sql = new StringBuilder("SELECT DISTINCT tcc.tcp_id, tcc.tne_id, tcc.tcp_ordem, tcc.tcp_descricao, tne.tne_nome ");
			sql.Append("FROM TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ");
			sql.Append("INNER JOIN TUR_Turma t WITH (NOLOCK) ON t.tur_id = ttcp.tur_id ");
			sql.Append("INNER JOIN ACA_TipoCurriculoPeriodo tcc WITH (NOLOCK) ON tcc.tme_id = ttcp.tme_id AND tcc.tne_id = ttcp.tne_id AND tcc.tcp_ordem = ttcp.crp_ordem ");
			sql.Append("INNER JOIN ACA_TipoNivelEnsino tne WITH (NOLOCK) ON tne.tne_id = tcc.tne_id ");
			sql.Append("WHERE t.esc_id = @esc_id AND t.tur_situacao = @state ");

			sql.Append("ORDER BY tcc.tcp_descricao, tne.tne_nome ");


			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<ACA_TipoCurriculoPeriodo, ACA_TipoNivelEnsino, ACA_TipoCurriculoPeriodo>(sql.ToString(),
					(tcp, tne) =>
					{
						tcp.ACA_TipoNivelEnsino = tne;
						return tcp;
					}, new { esc_id = esc_id, state = (byte)1 }, splitOn: "tne_nome");
			}
		}

		public ACA_TipoCurriculoPeriodo Get(int tcp_id)
		{
			var sql = new StringBuilder("SELECT [tcp_id],[tne_id],[tme_id],[tcp_descricao],[tcp_ordem],[tcp_situacao],[tcp_dataCriacao],[tcp_dataAlteracao] ");
			sql.Append("FROM [ACA_TipoCurriculoPeriodo] WITH (NOLOCK) ");
			sql.Append("WHERE [tcp_id] = @tcp_id AND [tcp_situacao] = @state ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.ExecuteScalar<ACA_TipoCurriculoPeriodo>(sql.ToString(), new { tcp_id = tcp_id, state = (byte)1 });
			}
		}

		public IEnumerable<ACA_TipoCurriculoPeriodo> Load()
		{
			var sql = new StringBuilder("SELECT [tcp_id],[tne_id],[tme_id],[tcp_descricao],[tcp_ordem],[tcp_situacao],[tcp_dataCriacao],[tcp_dataAlteracao] ");
			sql.Append("FROM [ACA_TipoCurriculoPeriodo] WITH (NOLOCK) ");
			sql.Append("WHERE [tcp_situacao] = @state ");
			sql.Append("ORDER BY [tcp_descricao] ASC ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<ACA_TipoCurriculoPeriodo>(sql.ToString(), new { state = (byte)1 });
			}
		}

		public IEnumerable<TUR_TurmaTipoCurriculoPeriodoDTO> LoadWithNivelEnsino()
		{
			var sql = new StringBuilder();
			sql.AppendLine("SELECT DISTINCT tcp.tcp_ordem, tcp.tcp_descricao, tne.tne_id, tne.tne_nome");
			sql.AppendLine("FROM ACA_TipoCurriculoPeriodo tcp WITH (NOLOCK)");
			sql.AppendLine("INNER JOIN ACA_TipoNivelEnsino tne WITH (NOLOCK) ON tcp.tne_id = tne.tne_id");

			sql.AppendLine("WHERE [tcp_situacao] = @state ");
			sql.AppendLine("ORDER BY [tcp_descricao], tne_nome ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<TUR_TurmaTipoCurriculoPeriodoDTO>(sql.ToString(), new { state = (byte)1 });
			}
		}

		public IEnumerable<TUR_TurmaTipoCurriculoPeriodoDTO> LoadWithNivelEnsino(Guid ent_id, Guid? pes_id = null, IEnumerable<string> esc_id = null, IEnumerable<string> dre_id = null)
		{
			var sql = new StringBuilder();
			sql.AppendLine("SELECT DISTINCT tcp.tcp_ordem, tcp.tcp_descricao, tne.tne_id, tne.tne_nome");
			sql.AppendLine("FROM ACA_TipoCurriculoPeriodo tcp WITH (NOLOCK)");
			sql.AppendLine("INNER JOIN ACA_TipoNivelEnsino tne WITH (NOLOCK) ON tcp.tne_id = tne.tne_id");

			if (pes_id.HasValue)
			{
				sql.AppendLine("INNER JOIN TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ON ttcp.crp_ordem = tcp.tcp_ordem AND ttcp.tne_id = tne.tne_id");
				sql.AppendLine("INNER JOIN TUR_TurmaDisciplina tud ON tud.tur_id = ttcp.tur_id");
				sql.AppendLine("INNER JOIN TUR_TurmaDocente tdt ON tdt.tud_id = tud.tud_id");
				sql.AppendLine("INNER JOIN ACA_Docente doc ON doc.doc_id = tdt.doc_id AND doc.pes_id = @pes_id AND doc.ent_id = @ent_id");
			}
			else if (esc_id != null)
			{
				sql.AppendLine("INNER JOIN TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ON ttcp.crp_ordem = tcp.tcp_ordem AND ttcp.tne_id = tne.tne_id");
				sql.Append("INNER JOIN ESC_Escola e WITH (NOLOCK) ON e.esc_id = ttcp.esc_id ");
				sql.AppendLine(string.Format("AND e.uad_id IN ({0})", string.Join(",", esc_id)));
			}

			else if (dre_id != null)
			{
				sql.AppendLine("INNER JOIN TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ON ttcp.crp_ordem = tcp.tcp_ordem AND ttcp.tne_id = tne.tne_id");
				sql.Append("INNER JOIN ESC_Escola e WITH (NOLOCK) ON e.esc_id = ttcp.esc_id ");
				sql.AppendLine(string.Format("AND e.uad_idSuperiorGestao IN ({0})", string.Join(",", dre_id)));
			}

			sql.AppendLine("WHERE [tcp_situacao] = @state ");
			sql.AppendLine("ORDER BY [tcp_descricao], tne_nome ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<TUR_TurmaTipoCurriculoPeriodoDTO>(sql.ToString(), new
				{
					state = (byte)1,
					ent_id = ent_id,
					pes_id = pes_id
				});
			}
		}

		public IEnumerable<ACA_TipoCurriculoPeriodo> GetAllTypeCurriculumGrades()
		{
			var sql = new StringBuilder("SELECT [tcp_id],[tne_id],[tme_id],[tcp_descricao],[tcp_ordem],[tcp_situacao],[tcp_dataCriacao],[tcp_dataAlteracao] ");
			sql.Append("FROM [ACA_TipoCurriculoPeriodo] WITH (NOLOCK) ");
			sql.Append("WHERE [tcp_situacao] = @state ");
			sql.Append("ORDER BY [tcp_ordem] ASC ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<ACA_TipoCurriculoPeriodo>(sql.ToString(), new { state = (byte)1 });
			}
		}

		public IEnumerable<ACA_TipoCurriculoPeriodo> LoadByLevelEducationModality(int tne_id, int tme_id)
		{
			var sql = new StringBuilder("SELECT [tcp_id],[tne_id],[tme_id],[tcp_descricao],[tcp_ordem],[tcp_situacao],[tcp_dataCriacao],[tcp_dataAlteracao] ");
			sql.Append("FROM [ACA_TipoCurriculoPeriodo] WITH (NOLOCK) ");
			sql.Append("WHERE [tne_id] = @tne_id AND [tme_id] = @tme_id AND [tcp_situacao] = @state ");
			sql.Append("ORDER BY [tcp_ordem] ASC ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<ACA_TipoCurriculoPeriodo>(sql.ToString(), new { tne_id = tne_id, tme_id = tme_id, state = (byte)1 });
			}
		}

		public string GetDescription(int tcp_id, int tne_id, int tme_id, int tcp_ordem)
		{
			var sql = new StringBuilder("SELECT [tcp_descricao] ");
			sql.Append("FROM [ACA_TipoCurriculoPeriodo] WITH (NOLOCK) ");
			sql.Append("WHERE [tcp_situacao] = @state ");
			if (tcp_id > 0)
				sql.Append("AND [tcp_id] = @tcp_id ");
			if (tne_id > 0)
				sql.Append("AND [tne_id] = @tne_id ");
			if (tme_id > 0)
				sql.Append("AND [tme_id] = @tme_id ");
			if (tcp_ordem > 0)
				sql.Append("AND [tcp_ordem] = @tcp_ordem ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				ACA_TipoCurriculoPeriodo entity = cn.Query<ACA_TipoCurriculoPeriodo>(sql.ToString(), new { tcp_id = tcp_id, tne_id = tne_id, tme_id = tme_id, tcp_ordem = tcp_ordem, state = (byte)1 }).FirstOrDefault();

				return entity != null ? entity.tcp_descricao : " - ";
			}
		}

		public int GetId(int tne_id, int tme_id, int tcp_ordem)
		{
			var sql = new StringBuilder("SELECT [tcp_id] ");
			sql.Append("FROM [ACA_TipoCurriculoPeriodo] WITH (NOLOCK) ");
			sql.Append("WHERE [tcp_situacao] = @state ");
			if (tne_id > 0)
				sql.Append("AND [tne_id] = @tne_id ");
			if (tme_id > 0)
				sql.Append("AND [tme_id] = @tme_id ");
			if (tcp_ordem > 0)
				sql.Append("AND [tcp_ordem] = @tcp_ordem ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				ACA_TipoCurriculoPeriodo entity = cn.Query<ACA_TipoCurriculoPeriodo>(sql.ToString(), new { tne_id = tne_id, tme_id = tme_id, tcp_ordem = tcp_ordem, state = (byte)1 }).FirstOrDefault();

				return entity != null ? entity.tcp_id : 0;
			}
		}
	}
}
