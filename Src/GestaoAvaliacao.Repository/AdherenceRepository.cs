using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
	public class AdherenceRepository : ConnectionReadOnly, IAdherenceRepository
	{
		#region Read
		public IEnumerable<AdherenceGrid> LoadSchoolGrid(Guid ent_id, ref Util.Pager pager, Guid uad_id, int esc_id, bool AllAdhered, long test_id, long testType_id, int ttn_id = 0, int crp_ordem = 0,
			IEnumerable<string> uadGestor = null, Guid? pes_id = null, IEnumerable<string> uadCoordenador = null)
		{
			#region Query

			#region Campos
			var camposSelect = new StringBuilder(string.Format("SELECT e.esc_id, esc_nome, uad.uad_nome, ISNULL(a.TypeSelection, {0}) AS TypeSelection, ", AllAdhered ? (byte)EnumAdherenceSelection.Selected : (byte)EnumAdherenceSelection.NotSelected));
			camposSelect.AppendLine("ROW_NUMBER() OVER (ORDER BY uad.uad_nome, esc_nome ASC) AS RowNumber ");
			#endregion

			#region Tables
			var tabelas = new StringBuilder("FROM SGP_ESC_Escola e WITH (NOLOCK) ");            
            tabelas.AppendLine("INNER JOIN AdministrativeUnitType AS AUT WITH(NOLOCK) ON AUT.AdministrativeUnitTypeId = e.tua_id AND AUT.State = @state ");
            tabelas.AppendLine("INNER JOIN SGP_SYS_UnidadeAdministrativa uad WITH (NOLOCK) ON e.uad_idSuperiorGestao = uad.uad_id  AND uad.uad_situacao = @state ");
			tabelas.AppendLine("INNER JOIN (SELECT DISTINCT tur.esc_id ");
			tabelas.AppendLine("FROM SGP_TUR_Turma tur WITH (NOLOCK) ");

			if (pes_id.HasValue)
			{
				tabelas.AppendLine("INNER JOIN ( SELECT DISTINCT (tud.tur_id) FROM SGP_TUR_TurmaDisciplina tud WITH (NOLOCK) ");
				tabelas.AppendLine("INNER JOIN SGP_TUR_TurmaDocente tdt WITH (NOLOCK) ON tdt.tud_id = tud.tud_id ");
				tabelas.AppendLine("INNER JOIN SGP_ACA_Docente d WITH (NOLOCK) ON d.doc_id = tdt.doc_id  ");
				tabelas.Append("WHERE tud.tud_situacao = @state AND tdt.tdt_situacao = @state AND d.doc_situacao = @state AND d.pes_id = @pes_id AND d.ent_id = @ent_id )  ");
				tabelas.Append("AS tud ON tud.tur_id = tur.tur_id ");
			}

			tabelas.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ON tur.tur_id = ttcp.tur_id AND ttcp.tur_id = tur.tur_id AND ttcp.ttcr_situacao = @state ");
			tabelas.AppendLine("INNER JOIN SGP_ACA_TipoCurriculoPeriodo tcg WITH (NOLOCK) ON tcg.tcp_ordem = ttcp.crp_ordem AND tcg.tme_id = ttcp.tme_id AND tcg.tne_id = ttcp.tne_id  ");
			tabelas.AppendLine("INNER JOIN TestTypeCourse ttc WITH (NOLOCK) ON ttc.CourseId = ttcp.cur_id AND TestType_Id = @testType AND ttc.State = @state ");
			tabelas.AppendLine("INNER JOIN TestCurriculumGrade tcc WITH (NOLOCK) ON tcc.TypeCurriculumGradeId = tcg.tcp_id AND tcc.State = @state AND tcc.Test_Id = @test_id ");




			tabelas.AppendLine("WHERE tur.tur_situacao = @state ");

			if (crp_ordem > 0)
				tabelas.AppendLine("AND ttcp.crp_ordem = @crp_ordem ");

			if (esc_id > 0)
				tabelas.AppendLine("AND tur.esc_id = @esc_id ");

			if (ttn_id > 0)
				tabelas.AppendLine("AND tur.ttn_id = @ttn_id ");

			tabelas.AppendLine(") t ON e.esc_id = t.esc_id ");

			tabelas.AppendLine("LEFT JOIN Adherence a WITH (NOLOCK) ON a.EntityId = e.esc_id AND a.TypeEntity = @typeEntity AND a.Test_Id = @test_id ");
			tabelas.AppendLine("WHERE e.ent_id = @ent_id AND e.esc_situacao = @state ");

			if (uad_id != Guid.Empty)
				tabelas.AppendLine("AND e.uad_idSuperiorGestao = @uad_idSuperiorGestao ");

			if (uadGestor != null)
				tabelas.AppendLine(string.Format("AND e.uad_idSuperiorGestao IN ({0}) ", string.Join(",", uadGestor)));

			if (uadCoordenador != null)
				tabelas.AppendLine(string.Format("AND e.uad_id IN ({0}) ", string.Join(",", uadCoordenador)));

			if (esc_id > 0)
				tabelas.AppendLine("AND e.esc_id = @esc_id ");

			#endregion


			var sql = new StringBuilder("WITH CounteredSchools AS ( ");
			sql.Append(camposSelect.ToString());
			sql.Append(tabelas.ToString());
			sql.AppendLine(")");
			sql.AppendLine("SELECT esc_id, esc_nome, uad_nome, TypeSelection ");
			sql.AppendLine("FROM CounteredSchools ");
			sql.AppendLine("WHERE RowNumber > ( @pageSize * @page ) ");
			sql.AppendLine("AND RowNumber <= ( ( @page + 1 ) * @pageSize ) ");
			sql.AppendLine("ORDER BY RowNumber ");

			sql.AppendLine("SELECT count(e.esc_id) ");
			sql.AppendLine(tabelas.ToString());
			#endregion

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var query = cn.QueryMultiple(sql.ToString(),
					new
					{
						ent_id = ent_id,
						pagesize = pager.PageSize,
						page = pager.CurrentPage,
						uad_idSuperiorGestao = uad_id,
						esc_id = esc_id,
						ttn_id = ttn_id,
						state = (byte)1,
						crp_ordem = crp_ordem,
						typeEntity = (byte)EnumAdherenceEntity.School,
						test_id = test_id,
						testType = testType_id,
						pes_id = pes_id
					});

				var retorno = query.Read<AdherenceGrid>();

				int count = query.Read<int>().FirstOrDefault();

				pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return retorno;

			}
		}

		public IEnumerable<Adherence> GetByTest(long test_id, EnumAdherenceEntity typeEntity, IEnumerable<int> idsEntity = null, long ParentId = 0)
		{
			var sql = new StringBuilder("SELECT Id, EntityId, TypeSelection ");
			sql.Append("FROM Adherence WITH (NOLOCK) ");
			sql.Append("WHERE Test_Id = @test_id AND State = @state ");
			sql.Append("AND TypeEntity = @typeEntity ");
			if (idsEntity != null)
				sql.AppendFormat("AND EntityId IN ({0}) ", string.Join(",", idsEntity));
			if (ParentId > 0)
				sql.Append("AND ParentId = @ParentId ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var retorno = cn.Query<Adherence>(sql.ToString(), new { test_id = test_id, @state = (byte)1, typeEntity = (byte)typeEntity, ParentId = ParentId });

				return retorno;
			}
		}

		public Adherence GetByTest(long test_id, EnumAdherenceEntity typeEntity, long EntityId)
		{
			var sql = new StringBuilder("SELECT Id, EntityId, TypeSelection, StatusCorrection ");
			sql.Append("FROM Adherence WITH (NOLOCK) ");
			sql.Append("WHERE Test_Id = @test_id AND State = @state ");
			sql.Append("AND TypeEntity = @typeEntity ");
			sql.AppendFormat("AND EntityId = {0} ", EntityId);


			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var retorno = cn.Query<Adherence>(sql.ToString(), new { test_id = test_id, @state = (byte)1, typeEntity = (byte)typeEntity }).FirstOrDefault();

				return retorno;
			}
		}

		public IEnumerable<AdheredEntityDTO> GetAdheredDreSimple(long TestId, EnumSYS_Visao visao, Guid gru_id, Guid usu_id, Guid pes_id, Guid ent_id, int tne_id, int crp_ordem, bool AllAdhered)
		{
			var sql = new StringBuilder();
			sql.AppendLine("SELECT DISTINCT CAST(uad.uad_id AS varchar(36)) AS EntityId, (uad.uad_sigla + ' - ' +  uad.uad_nome) AS Description");
			sql.AppendLine("FROM SGP_SYS_UnidadeAdministrativa uad WITH(NOLOCK)");
			sql.AppendLine("INNER JOIN SGP_ESC_Escola esc WITH(NOLOCK) ON uad.uad_id = esc.uad_idSuperiorGestao");
			sql.AppendLine("INNER JOIN SGP_TUR_Turma tur WITH(NOLOCK) ON tur.esc_id = esc.esc_id");

			if (visao != EnumSYS_Visao.Administracao)
				sql.AppendLine("INNER JOIN GetUserSection(@gru_id, @usu_id, @pes_id, @ent_id, @vis_id, @state, @esc_id, @uad_id, @ttn_id, @tne_id, @crp_ordem) section ON section.tur_id = tur.tur_id");

			if (!AllAdhered)
				sql.AppendLine("INNER JOIN Adherence a WITH(NOLOCK) ON a.EntityId = esc.esc_id AND a.TypeEntity = @TypeEntity AND a.Test_Id = @TestId");
			else
			{
				sql.AppendLine("LEFT JOIN Adherence a WITH(NOLOCK) ON a.EntityId = esc.esc_id AND a.TypeEntity = @TypeEntity AND a.Test_Id = @TestId AND a.TypeSelection = @TypeSelection");
				sql.AppendLine("WHERE a.Id IS NULL");
			}

			sql.AppendLine("ORDER BY (uad.uad_sigla + ' - ' +  uad.uad_nome)");

			var tneid = tne_id == 0 ? null : tne_id.ToString();
			var crporderm = crp_ordem == 0 ? null : crp_ordem.ToString();

			#region Parameters
			var p = new DynamicParameters();
			p.Add("@gru_id", gru_id);
			p.Add("@usu_id", usu_id);
			p.Add("@pes_id", pes_id);
			p.Add("@ent_id", ent_id);
			p.Add("@vis_id", (byte)visao);
			p.Add("@state", (byte)EnumState.ativo);
			p.Add("@esc_id");
			p.Add("@uad_id");
			p.Add("@ttn_id");
			p.Add("@tne_id", tneid);
			p.Add("@crp_ordem", crporderm);
			p.Add("@TypeEntity", (byte)EnumAdherenceEntity.School);
			p.Add("@TestId", TestId);
			p.Add("@TypeSelection", (byte)EnumAdherenceSelection.NotSelected);
			#endregion

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var retorno = cn.Query<AdheredEntityDTO>(sql.ToString(), p);

				return retorno;
			}
		}

        public IEnumerable<AdheredEntityDTO> GetAdheredDreSimpleReportItem(IEnumerable<long> lstTest, EnumSYS_Visao visao, Guid gru_id, Guid usu_id, Guid pes_id, Guid ent_id, int tne_id, int crp_ordem)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT CAST(uad.uad_id AS varchar(36)) AS EntityId, (uad.uad_sigla + ' - ' +  uad.uad_nome) AS Description");
            sql.AppendLine("FROM SGP_SYS_UnidadeAdministrativa uad WITH(NOLOCK)");
            sql.AppendLine("INNER JOIN SGP_ESC_Escola esc WITH(NOLOCK) ON uad.uad_id = esc.uad_idSuperiorGestao");
            sql.AppendLine("INNER JOIN SGP_TUR_Turma tur WITH(NOLOCK) ON tur.esc_id = esc.esc_id");

            if (visao != EnumSYS_Visao.Administracao)
                sql.AppendLine("INNER JOIN GetUserSection(@gru_id, @usu_id, @pes_id, @ent_id, @vis_id, @state, @esc_id, @uad_id, @ttn_id, @tne_id, @crp_ordem) section ON section.tur_id = tur.tur_id");

            sql.AppendFormat("INNER JOIN Test T WITH(NOLOCK) ON T.Id IN ({0}) ", string.Join(",", lstTest));
            sql.AppendLine("LEFT JOIN Adherence A WITH(NOLOCK) ON A.Test_Id = T.Id AND A.EntityId = esc.esc_id AND A.TypeEntity = @TypeEntity AND A.[State] = 1 ");
            sql.AppendLine("WHERE ((T.AllAdhered = 1 AND ISNULL(A.TypeSelection, 0) NOT IN (@TypeSelectionNaoSelecionado, @TypeSelectionBloqueado)) OR (T.AllAdhered = 0 AND TypeSelection = @TypeSelectionSelecionado)) ");
            sql.AppendLine("GROUP BY uad.uad_id, (uad.uad_sigla + ' - ' +  uad.uad_nome)");
            sql.AppendLine("ORDER BY (uad.uad_sigla + ' - ' +  uad.uad_nome)");

            var tneid = tne_id == 0 ? null : tne_id.ToString();
            var crporderm = crp_ordem == 0 ? null : crp_ordem.ToString();

            #region Parameters
            var p = new DynamicParameters();
            p.Add("@gru_id", gru_id);
            p.Add("@usu_id", usu_id);
            p.Add("@pes_id", pes_id);
            p.Add("@ent_id", ent_id);
            p.Add("@vis_id", (byte)visao);
            p.Add("@state", (byte)EnumState.ativo);
            p.Add("@esc_id");
            p.Add("@uad_id");
            p.Add("@ttn_id");
            p.Add("@tne_id", tneid);
            p.Add("@crp_ordem", crporderm);
            p.Add("@TypeEntity", (byte)EnumAdherenceEntity.School);
            p.Add("@TypeSelectionNaoSelecionado", (byte)EnumAdherenceSelection.NotSelected);
            p.Add("@TypeSelectionBloqueado", (byte)EnumAdherenceSelection.Blocked);
            p.Add("@TypeSelectionSelecionado", (byte)EnumAdherenceSelection.Selected);
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var retorno = cn.Query<AdheredEntityDTO>(sql.ToString(), p);

                return retorno;
            }
        }

        public IEnumerable<AdheredEntityDTO> GetAdheredSchoolSimpleReportItem(List<long> lstTest, EnumSYS_Visao visao, Guid gru_id, Guid usu_id, Guid pes_id, Guid ent_id, int tne_id, int crp_ordem, Guid uad_id)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT CAST(esc.esc_id AS varchar) AS EntityId, esc.esc_nome AS Description");
            sql.AppendLine("FROM SGP_ESC_Escola esc WITH(NOLOCK) ");
            sql.AppendLine("INNER JOIN SGP_TUR_Turma tur WITH(NOLOCK) ON tur.esc_id = esc.esc_id");

            if (visao != EnumSYS_Visao.Administracao)
                sql.AppendLine("INNER JOIN GetUserSection(@gru_id, @usu_id, @pes_id, @ent_id, @vis_id, @state, @esc_id, @uad_id, @ttn_id, @tne_id, @crp_ordem) section ON section.tur_id = tur.tur_id");

            sql.AppendFormat("INNER JOIN Test T WITH(NOLOCK) ON T.Id IN ({0}) ", string.Join(",", lstTest));
            sql.AppendLine("LEFT JOIN Adherence A WITH(NOLOCK) ON A.Test_Id = T.Id AND A.EntityId = esc.esc_id AND A.TypeEntity = @TypeEntity AND A.[State] = 1 ");
            sql.AppendLine("WHERE ((T.AllAdhered = 1 AND ISNULL(A.TypeSelection, 0) NOT IN (@TypeSelectionNaoSelecionado, @TypeSelectionBloqueado)) OR (T.AllAdhered = 0 AND TypeSelection = @TypeSelectionSelecionado)) ");
            sql.AppendLine("AND esc.uad_idSuperiorGestao = @uad_id");
            sql.AppendLine("ORDER BY esc.esc_nome");

            var tneid = tne_id == 0 ? null : tne_id.ToString();
            var crporderm = crp_ordem == 0 ? null : crp_ordem.ToString();

            #region Parameters
            var p = new DynamicParameters();
            p.Add("@gru_id", gru_id);
            p.Add("@usu_id", usu_id);
            p.Add("@pes_id", pes_id);
            p.Add("@ent_id", ent_id);
            p.Add("@vis_id", (byte)visao);
            p.Add("@state", (byte)EnumState.ativo);
            p.Add("@esc_id");
            p.Add("@uad_id", uad_id);
            p.Add("@ttn_id");
            p.Add("@tne_id", tneid);
            p.Add("@crp_ordem", crporderm);
            p.Add("@TypeEntity", (byte)EnumAdherenceEntity.School);
            p.Add("@TypeSelectionNaoSelecionado", (byte)EnumAdherenceSelection.NotSelected);
            p.Add("@TypeSelectionBloqueado", (byte)EnumAdherenceSelection.Blocked);
            p.Add("@TypeSelectionSelecionado", (byte)EnumAdherenceSelection.Selected);
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var retorno = cn.Query<AdheredEntityDTO>(sql.ToString(), p);

                return retorno;
            }
        }

        public IEnumerable<AdheredEntityDTO> GetAdheredSchoolSimple(long TestId, EnumSYS_Visao visao, Guid gru_id, Guid usu_id, Guid pes_id, Guid ent_id, int tne_id, int crp_ordem, bool AllAdhered, Guid uad_id)
		{
			var sql = new StringBuilder();
			sql.AppendLine("SELECT DISTINCT CAST(esc.esc_id AS varchar) AS EntityId, esc.esc_nome AS Description");
			sql.AppendLine("FROM SGP_ESC_Escola esc WITH(NOLOCK) ");
			sql.AppendLine("INNER JOIN SGP_TUR_Turma tur WITH(NOLOCK) ON tur.esc_id = esc.esc_id");

			if (visao != EnumSYS_Visao.Administracao)
				sql.AppendLine("INNER JOIN GetUserSection(@gru_id, @usu_id, @pes_id, @ent_id, @vis_id, @state, @esc_id, @uad_id, @ttn_id, @tne_id, @crp_ordem) section ON section.tur_id = tur.tur_id");

			if (!AllAdhered)
			{
				sql.AppendLine("INNER JOIN Adherence a WITH(NOLOCK) ON a.EntityId = esc.esc_id AND a.TypeEntity = @TypeEntity AND a.Test_Id = @TestId");

				if (uad_id != Guid.Empty)
					sql.AppendLine("WHERE esc.uad_idSuperiorGestao = @uad_id");
			}
			else
			{
				sql.AppendLine("LEFT JOIN Adherence a WITH(NOLOCK) ON a.EntityId = esc.esc_id AND a.TypeEntity = @TypeEntity AND a.Test_Id = @TestId AND a.TypeSelection = @TypeSelection");
				sql.AppendLine("WHERE a.Id IS NULL");
				if (uad_id != Guid.Empty)
					sql.AppendLine("AND esc.uad_idSuperiorGestao = @uad_id");
			}

			sql.AppendLine("ORDER BY esc.esc_nome");

			var tneid = tne_id == 0 ? null : tne_id.ToString();
			var crporderm = crp_ordem == 0 ? null : crp_ordem.ToString();

			#region Parameters
			var p = new DynamicParameters();
			p.Add("@gru_id", gru_id);
			p.Add("@usu_id", usu_id);
			p.Add("@pes_id", pes_id);
			p.Add("@ent_id", ent_id);
			p.Add("@vis_id", (byte)visao);
			p.Add("@state", (byte)EnumState.ativo);
			p.Add("@esc_id");
			p.Add("@uad_id", uad_id);
			p.Add("@ttn_id");
			p.Add("@tne_id", tneid);
			p.Add("@crp_ordem", crporderm);
			p.Add("@TypeEntity", (byte)EnumAdherenceEntity.School);
			p.Add("@TestId", TestId);
			p.Add("@TypeSelection", (byte)EnumAdherenceSelection.NotSelected);
			#endregion

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var retorno = cn.Query<AdheredEntityDTO>(sql.ToString(), p);

				return retorno;
			}
		}

        public IEnumerable<AdheredEntityDTO> GetAdheredSectionSimple(long TestId, EnumSYS_Visao visao, Guid gru_id, Guid usu_id, Guid pes_id, Guid ent_id, int tne_id, int crp_ordem, bool AllAdhered, int esc_id)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT CAST(tur.tur_id AS varchar) AS EntityId, tur.tur_codigo + ' ' + ttn.ttn_nome AS Description");
            sql.AppendLine("FROM SGP_TUR_Turma tur WITH(NOLOCK)");
            sql.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp WITH(NOLOCK) ON ttcp.tur_id = tur.tur_id AND ((ttcp.tne_id = @tne_id AND ttcp.crp_ordem = @crp_ordem) OR (@tne_id = 0 AND 0 = @crp_ordem)) ");
            sql.AppendLine("INNER JOIN SGP_ACA_TipoTurno ttn WITH(NOLOCK) ON ttn.ttn_id = tur.ttn_id");

			if (visao != EnumSYS_Visao.Administracao)
				sql.AppendLine("INNER JOIN GetUserSection(@gru_id, @usu_id, @pes_id, @ent_id, @vis_id, @state, @esc_id, @uad_id, @ttn_id, @tne_id, @crp_ordem) section ON section.tur_id = tur.tur_id");

			if (!AllAdhered)
				sql.AppendLine("INNER JOIN Adherence a WITH(NOLOCK) ON a.EntityId = tur.tur_id AND a.TypeEntity = @TypeEntity AND a.Test_Id = @TestId");
			else
				sql.AppendLine("LEFT JOIN Adherence a WITH(NOLOCK) ON a.EntityId = tur.tur_id AND a.TypeEntity = @TypeEntity AND a.Test_Id = @TestId AND a.TypeSelection = @TypeSelection");

			sql.AppendLine("WHERE tur.tur_situacao = @state");

			if (AllAdhered)
				sql.AppendLine("AND a.Id IS NULL");

			sql.AppendLine("ORDER BY tur.tur_codigo + ' ' + ttn.ttn_nome");

			#region Parameters
			var p = new DynamicParameters();
			p.Add("@gru_id", gru_id);
			p.Add("@usu_id", usu_id);
			p.Add("@pes_id", pes_id);
			p.Add("@ent_id", ent_id);
			p.Add("@vis_id", (byte)visao);
			p.Add("@state", (byte)EnumState.ativo);
			p.Add("@esc_id", esc_id);
			p.Add("@uad_id");
			p.Add("@ttn_id");
			p.Add("@tne_id", tne_id);
			p.Add("@crp_ordem", crp_ordem);
			p.Add("@TypeEntity", (byte)EnumAdherenceEntity.Section);
			p.Add("@TestId", TestId);
			p.Add("@TypeSelection", (byte)EnumAdherenceSelection.NotSelected);
			#endregion

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var retorno = cn.Query<AdheredEntityDTO>(sql.ToString(), p);

				return retorno;
			}
		}


        public IEnumerable<AdheredEntityDTO> GetAdheredSectionBySchool(long TestId, EnumSYS_Visao visao, Guid gru_id, Guid usu_id, Guid pes_id, Guid ent_id, int tne_id, int crp_ordem, bool AllAdhered, int esc_id)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT CAST(tur.tur_id AS varchar) AS EntityId, tur.tur_codigo + ' ' + ttn.ttn_nome AS Description");
            sql.AppendLine("FROM SGP_TUR_Turma tur WITH(NOLOCK)");
            sql.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp WITH(NOLOCK) ON ttcp.tur_id = tur.tur_id AND ((ttcp.tne_id = @tne_id AND ttcp.crp_ordem = @crp_ordem) OR (@tne_id = 0 AND 0 = @crp_ordem)) ");
            sql.AppendLine("INNER JOIN SGP_ACA_TipoTurno ttn WITH(NOLOCK) ON ttn.ttn_id = tur.ttn_id");

            if (visao != EnumSYS_Visao.Administracao)
                sql.AppendLine("INNER JOIN GetUserSection(@gru_id, @usu_id, @pes_id, @ent_id, @vis_id, @state, @esc_id, @uad_id, @ttn_id, @tne_id, @crp_ordem) section ON section.tur_id = tur.tur_id");

            if (!AllAdhered)
                sql.AppendLine("INNER JOIN Adherence a WITH(NOLOCK) ON a.EntityId = tur.tur_id AND a.TypeEntity = @TypeEntity AND a.Test_Id = @TestId");
            else
                sql.AppendLine("LEFT JOIN Adherence a WITH(NOLOCK) ON a.EntityId = tur.tur_id AND a.TypeEntity = @TypeEntity AND a.Test_Id = @TestId AND a.TypeSelection = @TypeSelection");

            sql.AppendLine("WHERE tur.tur_situacao = @state AND tur.esc_id = @esc_id");

            if (AllAdhered)
                sql.AppendLine("AND a.Id IS NULL");

            sql.AppendLine("ORDER BY tur.tur_codigo + ' ' + ttn.ttn_nome");

            #region Parameters
            var p = new DynamicParameters();
            p.Add("@gru_id", gru_id);
            p.Add("@usu_id", usu_id);
            p.Add("@pes_id", pes_id);
            p.Add("@ent_id", ent_id);
            p.Add("@vis_id", (byte)visao);
            p.Add("@state", (byte)EnumState.ativo);
            p.Add("@esc_id", esc_id);
            p.Add("@uad_id");
            p.Add("@ttn_id");
            p.Add("@tne_id", tne_id);
            p.Add("@crp_ordem", crp_ordem);
            p.Add("@TypeEntity", (byte)EnumAdherenceEntity.Section);
            p.Add("@TestId", TestId);
            p.Add("@TypeSelection", (byte)EnumAdherenceSelection.NotSelected);
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var retorno = cn.Query<AdheredEntityDTO>(sql.ToString(), p);

                return retorno;
            }
        }
        public IEnumerable<AdherenceGrid> LoadSectionGrid(int esc_id, long test_id, long TestType_Id, bool AllAdhered, int ttn_id = 0, int crp_ordem = 0, Guid? pes_id = null, Guid? ent_id = null)
        {
            #region Query
            var sql = new StringBuilder(string.Format("SELECT tur.tur_id AS esc_id, tur.tur_codigo AS esc_nome, ' (' + tcg.tcp_descricao + ' - ' + ttn.ttn_nome + ' - ' + cur.cur_nome +') '  AS uad_nome, ISNULL(a.TypeSelection, {0}) AS TypeSelection ",
                AllAdhered ? (byte)EnumAdherenceSelection.Selected : (byte)EnumAdherenceSelection.NotSelected));

			sql.Append(this.GetSectionsDisponibleTestQuery(ttn_id, crp_ordem, pes_id));
			sql.Append("ORDER BY tur.tur_codigo ");
			#endregion

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var retorno = cn.Query<AdherenceGrid>(sql.ToString(), new
				{
					esc_id = esc_id,
					ttn_id = ttn_id,
					state = (byte)1,
					pes_id = pes_id,
					crp_ordem = crp_ordem,
					testType = TestType_Id,
					test_id = test_id,
					typeEntity = (byte)EnumAdherenceEntity.Section,
					ent_id = ent_id
				});

				return retorno;
			}
		}

		public IEnumerable<AdherenceGrid> LoadStudent(long tur_id, long test_id, bool AllAdhered, DateTime dataAplicacao)
		{
			#region Query
			var sql = string.Format(@"
                        SELECT 
	                        Alu.alu_id, 
	                        (CASE WHEN (Mtu.mtu_numeroChamada > 0) THEN CAST(Mtu.mtu_numeroChamada AS VARCHAR(MAX)) + ' - ' ELSE '' END + Alu.alu_nome) AS alu_nome, 
	                        ISNULL(a.TypeSelection, {0}) AS TypeSelection,
                            CAST(CASE WHEN (a.Id IS NULL) THEN 0 ELSE 1 END AS BIT) AS existAdherence
                        FROM 
	                        SGP_MTR_MatriculaTurma AS Mtu WITH(NOLOCK)
	                        INNER JOIN SGP_ACA_Aluno AS Alu WITH(NOLOCK)
		                        ON Mtu.alu_id = Alu.alu_id
		                        AND Alu.alu_situacao <> @stateExcluido
	                        LEFT JOIN Adherence A WITH(NOLOCK) 
		                        ON a.EntityId = alu.alu_id
		                        AND a.TypeEntity = @typeEntity 
		                        AND a.Test_Id = @Test_Id
                        WHERE
	                        Mtu.tur_id = @tur_id
                            AND Mtu.mtu_situacao <> @stateExcluido
	                        AND Mtu.mtu_dataMatricula <= @dataAplicacao
	                        AND (Mtu.mtu_dataSaida IS NULL OR Mtu.mtu_dataSaida > @dataAplicacao)
                        ORDER BY 
	                        Mtu.mtu_numeroChamada, alu.alu_nome"
					, AllAdhered ? (byte)EnumAdherenceSelection.Selected : (byte)EnumAdherenceSelection.NotSelected);

			#endregion

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var retorno = cn.Query<AdherenceGrid>(sql.ToString(), new
				{
					tur_id = tur_id,
					test_id = test_id,
					stateExcluido = (byte)3,
					typeEntity = (byte)EnumAdherenceEntity.Student,
					dataAplicacao = dataAplicacao
				});

				return retorno;
			}
		}

		public int GetDisponibleSectionTest(int esc_id, long test_id, long TestType_Id, bool AllAdhered, int ttn_id = 0, int crp_ordem = 0, Guid? pes_id = null, Guid? ent_id = null)
		{
			var sql = new StringBuilder("SELECT COUNT(tur.tur_id) ");
			sql.Append(this.GetSectionsDisponibleTestQuery(ttn_id, crp_ordem, pes_id));

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var retorno = cn.Query<int>(sql.ToString(), new
				{
					esc_id = esc_id,
					ttn_id = ttn_id,
					state = (byte)1,
					pes_id = pes_id,
					crp_ordem = crp_ordem,
					testType = TestType_Id,
					test_id = test_id,
					typeEntity = (byte)EnumAdherenceEntity.Section,
					ent_id = ent_id
				});

				return retorno.FirstOrDefault();
			}
		}

		private string GetSectionsDisponibleTestQuery(int ttn_id, int crp_ordem, Guid? pes_id)
		{
			var sql = new StringBuilder("FROM SGP_TUR_Turma tur WITH (NOLOCK) ");
			sql.AppendLine("INNER JOIN SGP_ACA_TipoTurno ttn WITH (NOLOCK) ON ttn.ttn_id = tur.ttn_id ");
			sql.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ON tur.tur_id = ttcp.tur_id AND ttcp.tur_id = tur.tur_id AND ttcp.ttcr_situacao = @state ");
			sql.AppendLine("INNER JOIN SGP_ACA_TipoCurriculoPeriodo tcg WITH (NOLOCK) ON tcg.tcp_ordem = ttcp.crp_ordem AND tcg.tme_id = ttcp.tme_id AND tcg.tne_id = ttcp.tne_id ");
			sql.AppendLine("INNER JOIN TestTypeCourse ttc WITH (NOLOCK) ON ttc.CourseId = ttcp.cur_id AND TestType_Id = @testType AND ttc.State = @state ");
			sql.AppendLine("INNER JOIN SGP_ACA_Curso cur WITH (NOLOCK) ON cur.cur_id = ttc.CourseId ");
			sql.AppendLine("INNER JOIN SGP_ACA_CurriculoPeriodo crp WITH(NOLOCK) ON crp.cur_id = cur.cur_id AND crp.crp_ordem = ttcp.crp_ordem AND crp.tcp_id = tcg.tcp_id ");
			sql.AppendLine("INNER JOIN TestCurriculumGrade tcc WITH (NOLOCK) ON tcc.TypeCurriculumGradeId = tcg.tcp_id AND tcc.State = @state AND tcc.Test_Id = @test_id ");

			if (pes_id.HasValue)
			{
				sql.AppendLine("INNER JOIN ( SELECT DISTINCT (tud.tur_id) FROM SGP_TUR_TurmaDisciplina tud WITH (NOLOCK) ");
				sql.AppendLine("INNER JOIN SGP_TUR_TurmaDocente tdt WITH (NOLOCK) ON tdt.tud_id = tud.tud_id ");
				sql.AppendLine("INNER JOIN SGP_ACA_Docente d WITH (NOLOCK) ON d.doc_id = tdt.doc_id  ");
				sql.Append("WHERE tud.tud_situacao = @state AND tdt.tdt_situacao = @state AND d.doc_situacao = @state AND d.pes_id = @pes_id AND d.ent_id = @ent_id )  ");
				sql.Append("AS tud ON tud.tur_id = tur.tur_id ");
			}

			sql.AppendLine("LEFT JOIN Adherence a WITH (NOLOCK) ON a.EntityId = tur.tur_id AND a.TypeEntity = @typeEntity AND a.Test_Id = tcc.Test_Id ");

			sql.Append("WHERE tur.esc_id = @esc_id AND tur.tur_situacao = @state ");

			if (ttn_id > 0)
				sql.AppendLine("AND tur.ttn_id = @ttn_id ");

			if (crp_ordem > 0)
				sql.AppendLine("AND ttcp.crp_ordem = @crp_ordem ");

			return sql.ToString();
		}

		public void GetTotalByTest(long test_id, bool AllAdhered, out int totalSchool, out int totalSection, EnumSYS_Visao visao, Guid? pes_id = null,
			Guid? ent_id = null, IEnumerable<string> uad_ids = null)
		{
			#region Escolas
			var sql = new StringBuilder("SELECT ISNULL(COUNT(EntityId), 0) ");
			sql.Append("FROM Adherence a WITH (NOLOCK) ");

			switch (visao)
			{
				case EnumSYS_Visao.Gestao:
					sql.AppendLine(string.Format("INNER JOIN SGP_ESC_Escola e WITH (NOLOCK) ON e.esc_id = a.EntityId AND e.esc_situacao = @state AND uad_idSuperiorGestao IN ({0})",
						string.Join(",", uad_ids)));
					break;
				case EnumSYS_Visao.UnidadeAdministrativa:
					sql.AppendLine(string.Format("INNER JOIN SGP_ESC_Escola e WITH (NOLOCK) ON e.esc_id = a.EntityId AND e.esc_situacao = @state AND uad_id IN ({0})",
						string.Join(",", uad_ids)));
					break;
				case EnumSYS_Visao.Individual:
					sql.AppendLine("INNER JOIN (");
					sql.AppendLine("SELECT DISTINCT tur.esc_id");
					sql.AppendLine("FROM SGP_TUR_Turma tur WITH(NOLOCK)");
					sql.AppendLine("INNER JOIN SGP_TUR_TurmaDisciplina tud WITH(NOLOCK) ON tur.tur_id = tud.tur_id AND tud.tud_situacao = @state");
					sql.AppendLine("INNER JOIN SGP_TUR_TurmaDocente tdt WITH(NOLOCK) ON tdt.tud_id = tud.tud_id AND tdt_situacao = @state");
					sql.AppendLine("INNER JOIN SGP_ACA_Docente doc WITH(NOLOCK) ON doc.doc_id = tdt.doc_id AND doc.doc_situacao = @state AND doc.ent_id = @ent_id AND doc.pes_id = @pes_id");
					sql.AppendLine("WHERE tur.tur_situacao = @state) AS esc ON esc.id = a.EntityId");
					break;
				default:
					break;
			}

			sql.Append("WHERE TypeEntity = @typeSchool AND Test_Id = @test_id AND State = @state ");

			if (AllAdhered)
				sql.Append("AND TypeSelection = @typeSelection ");
			#endregion

			#region Turmas
			sql.Append("SELECT ISNULL(COUNT(EntityId), 0) ");
			sql.Append("FROM Adherence a WITH (NOLOCK) ");

			switch (visao)
			{
				case EnumSYS_Visao.Gestao:
					sql.AppendLine("INNER JOIN SGP_Tur_Turma tur WITH (NOLOCK) ON tur.tur_id = a.EntityId");
					sql.AppendLine(string.Format("INNER JOIN SGP_ESC_Escola e WITH (NOLOCK) ON e.esc_id = a.ParentId AND e.esc_situacao = @state AND uad_idSuperiorGestao IN ({0})",
						string.Join(",", uad_ids)));
					break;
				case EnumSYS_Visao.UnidadeAdministrativa:
					sql.AppendLine("INNER JOIN SGP_Tur_Turma tur WITH (NOLOCK) ON tur.tur_id = a.EntityId");
					sql.AppendLine(string.Format("INNER JOIN SGP_ESC_Escola e WITH (NOLOCK) ON e.esc_id = a.ParentId AND e.esc_situacao = @state AND uad_id IN ({0})",
						string.Join(",", uad_ids)));
					break;
				case EnumSYS_Visao.Individual:
					sql.AppendLine("INNER JOIN (");
					sql.AppendLine("SELECT tur.tur_id");
					sql.AppendLine("FROM SGP_TUR_Turma tur WITH(NOLOCK)");
					sql.AppendLine("INNER JOIN SGP_TUR_TurmaDisciplina tud WITH(NOLOCK) ON tur.tur_id = tud.tur_id AND tud.tud_situacao = @state");
					sql.AppendLine("INNER JOIN SGP_TUR_TurmaDocente tdt WITH(NOLOCK) ON tdt.tud_id = tud.tud_id AND tdt_situacao = @state");
					sql.AppendLine("INNER JOIN SGP_ACA_Docente doc WITH(NOLOCK) ON doc.doc_id = tdt.doc_id AND doc.doc_situacao = @state AND doc.ent_id = @ent_id AND doc.pes_id = @pes_id");
					sql.AppendLine("WHERE tur.tur_situacao = @state) AS esc ON esc.id = a.EntityId");
					break;
				default:
					break;
			}
			sql.Append("WHERE TypeEntity = @typeSection AND Test_Id = @test_id AND State = @state ");

			if (AllAdhered)
				sql.Append("AND TypeSelection = @typeSelection ");
			#endregion

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var query = cn.QueryMultiple(sql.ToString(), new
				{
					typeSchool = (byte)EnumAdherenceEntity.School,
					state = (byte)EnumState.ativo,
					test_id = test_id,
					typeSection = (byte)EnumAdherenceEntity.Section,
					typeSelection = (byte)EnumAdherenceSelection.NotSelected
				});

				totalSchool = query.Read<int>().FirstOrDefault();
				totalSection = query.Read<int>().FirstOrDefault();
			}
		}

		public IEnumerable<AdherenceGrid> LoadOnlySelectedSchool(long test_id, ref Util.Pager pager, bool AllSelected, Guid uad_id, int esc_id, int ttn_id = 0, int crp_ordem = 0,
			Guid? pes_id = null, Guid? ent_id = null, IEnumerable<string> uadGestor = null, IEnumerable<string> uadCoordenador = null)
		{
			var tables = new StringBuilder("FROM SGP_ESC_Escola e WITH (NOLOCK) ");
            tables.AppendLine("INNER JOIN AdministrativeUnitType AS AUT WITH(NOLOCK) ON AUT.AdministrativeUnitTypeId = e.tua_id AND AUT.State = @state ");
            tables.Append("INNER JOIN SGP_SYS_UnidadeAdministrativa uad WITH (NOLOCK) ON uad.uad_id = e.uad_idSuperiorGestao ");

			if (pes_id.HasValue && ent_id.HasValue)
			{
				tables.AppendLine("INNER JOIN (SELECT DISTINCT t.esc_id FROM SGP_TUR_Turma t WITH (NOLOCK) ");
				tables.AppendLine("INNER JOIN SGP_TUR_TurmaDisciplina tud WITH (NOLOCK) ON tud.tur_id = t.tur_id AND tud.tud_situacao = @state ");
				tables.AppendLine("INNER JOIN SGP_TUR_TurmaDocente tdt WITH (NOLOCK) ON tdt.tud_id = tud.tud_id AND tdt.tdt_situacao = @state ");
				tables.AppendLine("INNER JOIN SGP_ACA_Docente d WITH (NOLOCK) ON d.doc_id = tdt.doc_id AND d.doc_situacao = @state ");
				tables.AppendLine("WHERE t.tur_situacao = @state AND d.pes_id = @pes_id AND d.ent_id = @ent_id) t ON t.esc_id = e.esc_id ");
			}
			if ((ttn_id > 0) && (crp_ordem > 0))
			{
				tables.AppendLine("INNER JOIN (SELECT DISTINCT esc_id FROM SGP_TUR_Turma tur WITH (NOLOCK) ");
				tables.AppendLine("INNER JOIN SGP_TUR_TurmaCurriculo tcr WITH (NOLOCK) ON tur.tur_id = tcr.tur_id ");
				tables.AppendLine("INNER JOIN SGP_ACA_CurriculoPeriodo cpr WITH (NOLOCK) ON cpr.cur_id = tcr.cur_id AND cpr.crr_id = tcr.crr_id AND cpr.crp_id = tcr.crp_id ");
				tables.AppendLine("WHERE tur.tur_situacao = @state AND tur.ttn_id = @ttn_id AND tur.esc_id = @esc_id AND cpr.crp_ordem = @crp_ordem) t ON t.esc_id = e.esc_id ");
			}
			else if (ttn_id > 0)
			{
				tables.AppendLine("INNER JOIN (SELECT DISTINCT esc_id FROM SGP_TUR_Turma WITH (NOLOCK) WHERE tur_situacao = @state AND ttn_id = @ttn_id AND esc_id = @esc_id) t ON t.esc_id = e.esc_id ");
			}
			else if (crp_ordem > 0)
			{
				tables.AppendLine("INNER JOIN (SELECT DISTINCT esc_id FROM SGP_TUR_Turma tur WITH (NOLOCK) ");
				tables.AppendLine("INNER JOIN SGP_TUR_TurmaCurriculo tcr WITH (NOLOCK) ON tur.tur_id = tcr.tur_id ");
				tables.AppendLine("INNER JOIN SGP_ACA_CurriculoPeriodo cpr WITH (NOLOCK) ON cpr.cur_id = tcr.cur_id AND cpr.crr_id = tcr.crr_id AND cpr.crp_id = tcr.crp_id ");
				tables.AppendLine("WHERE tur.tur_situacao = @state AND tur.esc_id = @esc_id AND cpr.crp_ordem = @crp_ordem) t ON t.esc_id = e.esc_id ");
			}

			//Se a prova for para todas as escolas selecionadas, remover as que estão salvas como não selecionadas
			if (AllSelected)
			{
				tables.Append("LEFT JOIN Adherence a WITH (NOLOCK) ON a.EntityId = e.esc_id AND a.Test_Id = @test_id AND a.TypeEntity = @typeEntity ");
				tables.Append(string.Format("WHERE (a.Id IS NULL OR a.TypeSelection = {0}) ", (byte)EnumAdherenceSelection.Partial));

				if (uad_id != Guid.Empty)
					tables.Append("AND e.uad_idSuperiorGestao = @uad_idSuperiorGestao ");

				if (uadGestor != null)
					tables.AppendFormat("AND e.uad_idSuperiorGestao IN ({0}) ", string.Join(",", uadGestor));

				if (uadCoordenador != null)
					tables.AppendFormat("AND e.uad_id IN ({0}) ", string.Join(",", uadCoordenador));

				if (esc_id > 0)
					tables.Append("AND e.esc_id = @esc_id ");
			}
			//Senão, trazer apenas as que estão salvas como selecionadas/parcial
			else
			{
				tables.Append("INNER JOIN Adherence a WITH (NOLOCK) ON a.EntityId = e.esc_id AND a.Test_Id = @test_id AND a.TypeEntity = @typeEntity ");

				if (uad_id != Guid.Empty || esc_id > 0)
				{
					tables.Append("WHERE ");

					if (uad_id != Guid.Empty)
						tables.Append("e.uad_idSuperiorGestao = @uad_idSuperiorGestao ");

					if (uadGestor != null)
						tables.AppendFormat("AND e.uad_idSuperiorGestao IN ({0}) ", string.Join(",", uadGestor));

					if (esc_id > 0)
						tables.Append("AND e.esc_id = @esc_id ");
				}
			}

			var sql = new StringBuilder("WITH CounteredSchools AS ( ");
			sql.Append("SELECT e.esc_id, e.esc_nome, uad.uad_nome, a.TypeSelection, ");
			sql.Append("ROW_NUMBER() OVER (ORDER BY uad.uad_nome, esc_nome ASC) AS RowNumber ");
			sql.Append(tables.ToString());
			sql.Append(" ) ");
			sql.AppendFormat("SELECT esc_id, esc_nome, uad_nome, ISNULL(TypeSelection, {0}) AS TypeSelection ", (byte)EnumAdherenceSelection.Selected);
			sql.Append("FROM CounteredSchools ");
			sql.Append("WHERE RowNumber > ( @pageSize * @page ) ");
			sql.Append("AND RowNumber <= ( ( @page + 1 ) * @pageSize )  ");
			sql.Append("ORDER BY RowNumber ");

			sql.Append("SELECT COUNT(e.esc_id) ");
			sql.Append(tables.ToString());

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var query = cn.QueryMultiple(sql.ToString(), new
				{
					test_id = test_id,
					uad_idSuperiorGestao = uad_id,
					esc_id = esc_id,
					pagesize = pager.PageSize,
					page = pager.CurrentPage,
					typeEntity = EnumAdherenceEntity.School,
					pes_id = pes_id,
					ent_id = ent_id,
					state = (byte)EnumState.ativo,
					ttn_id = ttn_id,
					crp_ordem = crp_ordem
				});

				var retorno = query.Read<AdherenceGrid>();

				int count = query.Read<int>().FirstOrDefault();

				pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return retorno;
			}
		}
		public IEnumerable<TeamsDTO> GetSectionByTestAndTcpId(List<long> test_id, Guid? uad_id, int? esc_id, long? tcp_id,
					Guid? pes_id = null, Guid? ent_id = null, IEnumerable<string> uadGestor = null, IEnumerable<string> uadCoordenador = null)
		{
			var tables = new StringBuilder("FROM SGP_TUR_Turma t WITH (NOLOCK) ");

			tables.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ON t.tur_id = ttcp.tur_id AND ttcp.tur_id = t.tur_id AND ttcp.ttcr_situacao = @state ");
			tables.AppendLine("INNER JOIN SGP_ACA_TipoCurriculoPeriodo tcg WITH (NOLOCK) ON tcg.tcp_ordem = ttcp.crp_ordem AND tcg.tme_id = ttcp.tme_id AND tcg.tne_id = ttcp.tne_id AND tcg.tcp_id = ISNULL(@tcp_id, tcg.tcp_id)");
			tables.AppendLine("INNER JOIN SGP_ACA_Curso cur WITH (NOLOCK) ON cur.cur_id = ttcp.cur_id ");
			tables.AppendLine("INNER JOIN SGP_ACA_CurriculoPeriodo crp WITH(NOLOCK) ON crp.cur_id = cur.cur_id AND crp.crp_ordem = ttcp.crp_ordem AND crp.tcp_id = tcg.tcp_id ");
			tables.AppendFormat("INNER JOIN TestCurriculumGrade tcc WITH (NOLOCK) ON tcc.TypeCurriculumGradeId = tcg.tcp_id AND tcc.State = @state AND tcc.Test_Id IN ({0}) ", string.Join(",", test_id));
            tables.AppendLine("INNER JOIN SGP_ESC_Escola e WITH(NOLOCK) ON t.esc_id = e.esc_id AND e.esc_situacao <> @excluido ");

            if (pes_id.HasValue)
			{
				tables.AppendLine("INNER JOIN ( SELECT DISTINCT (tud.tur_id) FROM SGP_TUR_TurmaDisciplina tud WITH (NOLOCK) ");
				tables.AppendLine("INNER JOIN SGP_TUR_TurmaDocente tdt WITH (NOLOCK) ON tdt.tud_id = tud.tud_id ");
				tables.AppendLine("INNER JOIN SGP_ACA_Docente d WITH (NOLOCK) ON d.doc_id = tdt.doc_id ");
				tables.Append("WHERE tud.tud_situacao = @state AND tdt.tdt_situacao = @state AND d.doc_situacao = @state AND d.pes_id = @pes_id ) ");
				tables.Append("AS tud ON tud.tur_id = t.tur_id ");
			}

            tables.AppendLine("INNER JOIN Test Te WITH(NOLOCK) ON tcc.Test_Id = Te.Id ");
            tables.AppendLine("LEFT JOIN Adherence A WITH(NOLOCK) ON A.Test_Id = Te.Id AND A.EntityId = e.esc_id AND A.TypeEntity = @typeEntity AND A.[State] = 1 ");
            tables.AppendLine("WHERE (Te.AllAdhered = 1 AND ISNULL(A.TypeSelection, 0) NOT IN (@TypeSelectionNaoSelecionado, @TypeSelectionBloqueado) OR (Te.AllAdhered = 0 AND TypeSelection = @TypeSelectionSelecionado)) ");
            tables.Append("AND t.tur_situacao <> @excluido ");
            
            if (uad_id != null && uad_id != Guid.Empty)
				tables.Append("AND e.uad_idSuperiorGestao = @uad_idSuperiorGestao ");
			if (uadGestor != null)
				tables.AppendFormat("AND e.uad_idSuperiorGestao IN ({0}) ", string.Join(",", uadGestor));
			if (uadCoordenador != null)
				tables.AppendFormat("AND e.uad_id IN ({0}) ", string.Join(",", uadCoordenador));
			if (esc_id > 0)
				tables.Append("AND e.esc_id = @esc_id ");


			var sql = new StringBuilder("SELECT Te.Id AS test_id, t.tur_id, t.tur_codigo, t.esc_id ");
			sql.Append(tables.ToString());


			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var query = cn.QueryMultiple(sql.ToString(), new
				{					
					uad_idSuperiorGestao = uad_id,
					esc_id = esc_id,
					typeEntity = EnumAdherenceEntity.School,
					pes_id = pes_id,
					ent_id = ent_id,
					state = (byte)EnumState.ativo,
					excluido = (byte)EnumState.excluido,
					tcp_id = tcp_id,
                    TypeSelectionNaoSelecionado = EnumAdherenceSelection.NotSelected,
                    TypeSelectionBloqueado = EnumAdherenceSelection.Blocked,
                    TypeSelectionSelecionado = EnumAdherenceSelection.Selected
				});

				var retorno = query.Read<TeamsDTO>();

				return retorno;
			}
		}

		public IEnumerable<AdherenceGrid> LoadOnlySelectedSection(long test_id, int esc_id, bool AllSelected, int ttn_id = 0, int crp_ordem = 0, Guid? pes_id = null)
		{
			var sql = new StringBuilder("SELECT t.tur_id AS esc_id, t.tur_codigo AS esc_nome, ' (' + tcg.tcp_descricao + ' - ' + ttn.ttn_nome + ' - ' + cur.cur_nome +') '  AS uad_nome, ");
			sql.AppendFormat("{0} AS TypeSelection ", AllSelected ? (byte)EnumAdherenceSelection.Selected : (byte)EnumAdherenceSelection.NotSelected);
			sql.Append("FROM SGP_TUR_Turma t WITH (NOLOCK) ");
			sql.Append("INNER JOIN SGP_ACA_TipoTurno ttn WITH (NOLOCK) ON t.ttn_id = ttn.ttn_id ");

			sql.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ON t.tur_id = ttcp.tur_id AND ttcp.tur_id = t.tur_id AND ttcp.ttcr_situacao = @state ");
			sql.AppendLine("INNER JOIN SGP_ACA_TipoCurriculoPeriodo tcg WITH (NOLOCK) ON tcg.tcp_ordem = ttcp.crp_ordem AND tcg.tme_id = ttcp.tme_id AND tcg.tne_id = ttcp.tne_id ");
			sql.AppendLine("INNER JOIN SGP_ACA_Curso cur WITH (NOLOCK) ON cur.cur_id = ttcp.cur_id ");
			sql.AppendLine("INNER JOIN SGP_ACA_CurriculoPeriodo crp WITH(NOLOCK) ON crp.cur_id = cur.cur_id AND crp.crp_ordem = ttcp.crp_ordem AND crp.tcp_id = tcg.tcp_id ");
			sql.AppendLine("INNER JOIN TestCurriculumGrade tcc WITH (NOLOCK) ON tcc.TypeCurriculumGradeId = tcg.tcp_id AND tcc.State = @state AND tcc.Test_Id = @test_id ");

			if (pes_id.HasValue)
			{
				sql.AppendLine("INNER JOIN ( SELECT DISTINCT (tud.tur_id) FROM SGP_TUR_TurmaDisciplina tud WITH (NOLOCK) ");
				sql.AppendLine("INNER JOIN SGP_TUR_TurmaDocente tdt WITH (NOLOCK) ON tdt.tud_id = tud.tud_id ");
				sql.AppendLine("INNER JOIN SGP_ACA_Docente d WITH (NOLOCK) ON d.doc_id = tdt.doc_id ");
				sql.Append("WHERE tud.tud_situacao = @state AND tdt.tdt_situacao = @state AND d.doc_situacao = @state AND d.pes_id = @pes_id ) ");
				sql.Append("AS tud ON tud.tur_id = t.tur_id ");
			}

			//Se a prova for para todas as escolas selecionadas, remover as que estão salvas como não selecionadas
			if (AllSelected)
			{
				sql.Append("LEFT JOIN Adherence a WITH (NOLOCK) ON a.EntityId = t.tur_id AND a.Test_Id = @test_id AND a.TypeEntity = @typeEntity AND a.TypeSelection != @TypeSelection ");
				sql.Append(string.Format("WHERE (a.Id IS NULL OR a.TypeSelection = {0}) AND ", (byte)EnumAdherenceSelection.Partial));

				if (ttn_id > 0)
					sql.Append("t.ttn_id = @ttn_id AND ");
			}
			//Senão, trazer apenas as que estão salvas como selecionadas/parcial
			else
			{
				sql.Append(string.Format("INNER JOIN Adherence a WITH (NOLOCK) ON a.EntityId = t.tur_id AND a.Test_Id = @test_id AND a.TypeEntity = @typeEntity AND (a.TypeSelection = {0} OR a.TypeSelection = {1}) ", (byte)EnumAdherenceSelection.Selected, (byte)EnumAdherenceSelection.Partial));
				sql.Append("WHERE ");
				if (ttn_id > 0)
					sql.Append("t.ttn_id = @ttn_id AND ");
			}

			sql.Append("t.esc_id = @esc_id AND t.tur_situacao = @state ");

			if (crp_ordem > 0)
				sql.AppendLine("AND crp.crp_ordem = @crp_ordem ");

			sql.Append("ORDER BY t.tur_codigo ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var retorno = cn.Query<AdherenceGrid>(sql.ToString(), new
				{
					esc_id = esc_id,
					ttn_id = ttn_id,
					crp_ordem = crp_ordem,
					state = (byte)1,
					pes_id = pes_id,
					test_id = test_id,
					typeEntity = EnumAdherenceEntity.Section,
					TypeSelection = EnumAdherenceSelection.Selected
				});

				return retorno;
			}
		}

		public IEnumerable<AdherenceGrid> LoadSelectedStudent(long tur_id, long test_id, bool AllAdhered, DateTime dataAplicacao)
		{
			#region Query
			StringBuilder sql = new StringBuilder();
			sql.AppendFormat(@" SELECT 
	                                Alu.alu_id, 
	                                (CASE WHEN (Mtu.mtu_numeroChamada > 0) THEN CAST(Mtu.mtu_numeroChamada AS VARCHAR(MAX)) + ' - ' ELSE '' END + Alu.alu_nome) AS alu_nome, 
	                                ISNULL(a.TypeSelection, {0}) AS TypeSelection
                                FROM 
	                                SGP_MTR_MatriculaTurma AS Mtu WITH(NOLOCK)
	                                INNER JOIN SGP_ACA_Aluno AS Alu WITH(NOLOCK)
		                                ON Mtu.alu_id = Alu.alu_id
		                                AND Alu.alu_situacao <> @stateExcluido "
							, AllAdhered ? (byte)EnumAdherenceSelection.Selected : (byte)EnumAdherenceSelection.NotSelected);

			//Se a prova for para todas as escolas selecionadas, remover as que estão salvas como não selecionadas
			if (AllAdhered)
			{
				sql.Append("LEFT JOIN Adherence A WITH (NOLOCK) ON A.EntityId = alu.alu_id AND A.Test_Id = @test_id AND a.TypeEntity = @typeEntity AND a.TypeSelection != @TypeSelection ");
				sql.Append(string.Format("WHERE (a.Id IS NULL OR a.TypeSelection = {0}) AND ", (byte)EnumAdherenceSelection.Blocked));
			}
			//Senão, trazer apenas as que estão salvas como selecionadas/parcial
			else
			{
				sql.Append("INNER JOIN Adherence A WITH (NOLOCK) ON A.EntityId = alu.alu_id AND A.Test_Id = @test_id AND a.TypeEntity = @typeEntity ");
				sql.Append("WHERE ");
			}

			sql.Append(@" Mtu.tur_id = @tur_id
                            AND Mtu.mtu_situacao <> @stateExcluido
	                        AND Mtu.mtu_dataMatricula <= @dataAplicacao
	                        AND (Mtu.mtu_dataSaida IS NULL OR Mtu.mtu_dataSaida > @dataAplicacao)
                        ORDER BY  Mtu.mtu_numeroChamada, alu.alu_nome");

			#endregion

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var retorno = cn.Query<AdherenceGrid>(sql.ToString(), new
				{
					tur_id = tur_id,
					test_id = test_id,
					stateExcluido = (byte)3,
					typeEntity = (byte)EnumAdherenceEntity.Student,
					TypeSelection = (byte)EnumAdherenceSelection.Selected,
					dataAplicacao = dataAplicacao
				});

				return retorno;
			}
		}

		public IEnumerable<AdherenceDTO> GetSectionsToAnswerSheetLot(long test_id, long TestType_Id, bool AllAdhered, int? esc_id, Guid? uad_id)
		{
			var sql = new StringBuilder("SELECT tur.tur_id, ua.uad_nome AS dre_nome, CASE WHEN ua.uad_sigla IS NULL THEN ua.uad_nome ELSE ua.uad_sigla END AS dre_sigla, esc.esc_nome, tur.tur_codigo, ttn.ttn_nome, esc.esc_id ");
			sql.AppendLine("FROM SGP_TUR_Turma tur WITH (NOLOCK) ");
			sql.AppendLine("INNER JOIN SGP_ESC_Escola esc WITH(NOLOCK) ON esc.esc_id = tur.esc_id ");

			if (esc_id > 0)
				sql.AppendLine("AND esc.esc_id = @esc_id");

			if (uad_id != null)
				sql.AppendLine("AND esc.uad_idSuperiorGestao = @uad_id");

			sql.AppendLine("INNER JOIN SGP_SYS_UnidadeAdministrativa ua WITH(NOLOCK) ON ua.uad_id = esc.uad_idSuperiorGestao ");
			sql.AppendLine("INNER JOIN SGP_ACA_TipoTurno ttn WITH (NOLOCK) ON ttn.ttn_id = tur.ttn_id ");
			sql.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ON tur.tur_id = ttcp.tur_id AND ttcp.tur_id = tur.tur_id AND ttcp.ttcr_situacao = @state ");
			sql.AppendLine("INNER JOIN SGP_ACA_TipoCurriculoPeriodo tcg WITH (NOLOCK) ON tcg.tcp_ordem = ttcp.crp_ordem AND tcg.tme_id = ttcp.tme_id AND tcg.tne_id = ttcp.tne_id ");
			sql.AppendLine("INNER JOIN TestTypeCourse ttc WITH (NOLOCK) ON ttc.CourseId = ttcp.cur_id AND TestType_Id = @testType AND ttc.State = @state ");
			sql.AppendLine("INNER JOIN SGP_ACA_Curso cur WITH (NOLOCK) ON cur.cur_id = ttc.CourseId ");
			sql.AppendLine("INNER JOIN TestCurriculumGrade tcc WITH (NOLOCK) ON tcc.TypeCurriculumGradeId = tcg.tcp_id AND tcc.State = @state AND tcc.Test_Id = @test_id ");

			if (AllAdhered)
			{
				sql.AppendLine("LEFT JOIN Adherence a ON a.EntityId = tur.tur_id AND a.TypeEntity = @typeEntity AND a.Test_Id = @test_id AND a.State = @state AND a.TypeSelection = @typeNotSelected");
				sql.Append("WHERE a.Id IS NULL AND ");
			}
			else
			{
				sql.AppendLine("INNER JOIN Adherence a ON a.EntityId = tur.tur_id AND a.TypeEntity = @typeEntity AND a.Test_Id = @test_id AND a.State = @state");
				sql.Append("WHERE ");
			}

			sql.Append("tur.tur_situacao = @state ");
			sql.AppendLine("ORDER BY ua.uad_nome, esc.esc_nome, tur.tur_codigo, ttn.ttn_nome");


			using (IDbConnection cn = Connection)
			{
				cn.Open();
				return cn.Query<AdherenceDTO>(sql.ToString(), new
				{
					esc_id = esc_id,
					uad_id = uad_id,
					state = (byte)1,
					test_id = test_id,
					testType = TestType_Id,
					typeEntity = EnumAdherenceEntity.Section,
					typeNotSelected = EnumAdherenceSelection.NotSelected
				});
			}
		}

		public IEnumerable<AdheredEntityDTO> LoadDreSimpleAdherence(long test_id, Guid ent_id, bool allAdhered, IEnumerable<string> uad_id = null)
		{
			var sql = new StringBuilder("SELECT CAST(Uad.uad_id AS varchar(36)) AS EntityId, Uad.uad_sigla + ' - ' +  Uad.uad_nome AS Description ");
			sql.Append("FROM SGP_SYS_UnidadeAdministrativa Uad WITH(NOLOCK) ");
			sql.Append("INNER JOIN SGP_ESC_Escola Esc WITH(NOLOCK) ON Uad.uad_id = Esc.uad_idSuperiorGestao ");

			if (!allAdhered)
			{
				sql.AppendLine("INNER JOIN Adherence Adh WITH(NOLOCK) ON Adh.EntityId = Esc.esc_id AND Adh.TypeEntity = @typeEntity AND Adh.Test_Id = @test_id ");
				sql.AppendLine("WHERE Uad.ent_id = @ent_id AND Uad.uad_situacao = @situacao AND Adh.State = @situacao ");
				if (uad_id != null)
					sql.AppendFormat("AND Uad.uad_id IN ({0}) ", string.Join(",", uad_id.ToArray()));
			}
			else
			{
				sql.AppendLine("LEFT JOIN Adherence Adh WITH(NOLOCK) ON Adh.EntityId = Esc.esc_id AND Adh.TypeEntity = @typeEntity AND Adh.Test_Id = @test_id AND Adh.TypeSelection = @typeSelection ");
				sql.AppendLine("WHERE Adh.Id IS NULL AND Uad.ent_id = @ent_id AND Uad.uad_situacao = @situacao ");
				if (uad_id != null)
					sql.AppendFormat("AND Uad.uad_id IN ({0}) ", string.Join(",", uad_id.ToArray()));
			}

			sql.Append("GROUP BY Uad.uad_id, Uad.uad_sigla, Uad.uad_nome ");
			sql.Append("ORDER BY Uad.uad_nome");

			var p = new DynamicParameters();
			p.Add("@ent_id", ent_id);
			p.Add("@situacao", (byte)1);
			p.Add("@typeEntity", (byte)EnumAdherenceEntity.School);
			p.Add("@test_id", test_id);
			p.Add("@typeSelection", (byte)EnumAdherenceSelection.NotSelected);

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<AdheredEntityDTO>(sql.ToString(), p);
			}
		}

		public IEnumerable<AdheredEntityDTO> LoadDRESimpleCoordinatorAdherence(long test_id, Guid ent_id, IEnumerable<string> uad_id, bool allAdhered)
		{
			var sql = new StringBuilder();
			sql.AppendLine("SELECT CAST(Uad.uad_id AS varchar(36)) AS EntityId, Uad.uad_nome AS Description ");
			sql.AppendLine("FROM SGP_SYS_UnidadeAdministrativa Uad WITH(NOLOCK) ");
			sql.AppendLine("INNER JOIN SGP_ESC_Escola Esc WITH(NOLOCK) ON Uad.uad_id = Esc.uad_idSuperiorGestao ");
			if (!allAdhered)
			{
				sql.AppendLine("INNER JOIN Adherence Adh WITH(NOLOCK) ON Adh.EntityId = Esc.esc_id AND Adh.TypeEntity = @typeEntity AND Adh.Test_Id = @test_id ");
				sql.AppendLine("WHERE Uad.ent_id = @ent_id AND Uad.uad_situacao = @situacao AND Adh.State = @situacao ");
				if (uad_id != null)
					sql.AppendFormat("AND Uad.uad_id IN ({0}) ", string.Join(",", uad_id.ToArray()));
			}
			else
			{
				sql.AppendLine("LEFT JOIN Adherence Adh WITH(NOLOCK) ON Adh.EntityId = Esc.esc_id AND Adh.TypeEntity = @typeEntity AND Adh.Test_Id = @test_id AND Adh.TypeSelection = @typeSelection ");
				sql.AppendLine("WHERE Adh.Id IS NULL AND Uad.ent_id = @ent_id AND Uad.uad_situacao = @situacao ");
				if (uad_id != null)
					sql.AppendFormat("AND Esc.uad_id IN ({0}) ", string.Join(",", uad_id.ToArray()));
			}
			sql.AppendLine("GROUP BY Uad.uad_id, Uad.uad_nome ");
			sql.AppendLine("ORDER BY Uad.uad_nome");

			var p = new DynamicParameters();
			p.Add("@ent_id", ent_id);
			p.Add("@situacao", (byte)1);
			p.Add("@typeEntity", (byte)EnumAdherenceEntity.School);
			p.Add("@test_id", test_id);
			p.Add("@typeSelection", (byte)EnumAdherenceSelection.NotSelected);

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				return cn.Query<AdheredEntityDTO>(sql.ToString(), p);
			}
		}

		public IEnumerable<AdheredEntityDTO> LoadDRESimpleTeacherAdherence(long test_id, Guid ent_id, Guid pes_id, bool allAdhered)
		{
			var sql = new StringBuilder("SELECT CAST(Uad.uad_id AS varchar(36)) AS EntityId, Uad.uad_nome AS Description ");
			sql.Append("FROM SGP_ACA_Docente a WITH(NOLOCK) ");
			sql.Append("INNER JOIN SGP_TUR_TurmaDocente td WITH(NOLOCK) ON a.doc_id = td.doc_id ");
			sql.Append("INNER JOIN SGP_TUR_TurmaDisciplina tdis WITH(NOLOCK) ON tdis.tud_id = td.tud_id ");
			sql.Append("INNER JOIN SGP_TUR_Turma t WITH(NOLOCK) ON t.tur_id = tdis.tur_id ");
			sql.Append("INNER JOIN SGP_ESC_Escola e WITH(NOLOCK) ON e.esc_id = t.esc_id ");
			sql.Append("INNER JOIN SGP_SYS_UnidadeAdministrativa uad WITH(NOLOCK) ON uad.uad_id = e.uad_idSuperiorGestao ");
			if (!allAdhered)
			{
				sql.AppendLine("INNER JOIN Adherence Adh WITH(NOLOCK) ON Adh.EntityId = e.esc_id AND Adh.TypeEntity = @typeEntity AND Adh.Test_Id = @test_id AND Adh.State = @situacao  ");
			}
			sql.AppendLine("WHERE a.ent_id = @ent_id AND Uad.uad_situacao = @situacao AND a.pes_id = @pes_id ");
			sql.Append("AND a.doc_situacao = @situacao AND td.tdt_situacao = @situacao AND t.tur_situacao = @situacao AND e.esc_situacao = @situacao AND uad.uad_situacao = @situacao ");

			sql.Append("GROUP BY Uad.uad_id, Uad.uad_nome ");
			sql.Append("ORDER BY uad.uad_nome");

			var p = new DynamicParameters();
			p.Add("@ent_id", ent_id);
			p.Add("@pes_id", pes_id);
			p.Add("@situacao", (byte)1);
			p.Add("@typeEntity", (byte)EnumAdherenceEntity.School);
			p.Add("@test_id", test_id);
			p.Add("@typeSelection", (byte)EnumAdherenceSelection.NotSelected);

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<AdheredEntityDTO>(sql.ToString(), p);
			}
		}

		public IEnumerable<AdheredEntityDTO> LoadSchoolSimpleAdherence(long test_id, Guid ent_id, Guid uad_id, bool allAdhered, IEnumerable<string> esc_id = null)
		{
			var sql = new StringBuilder("SELECT CAST(Esc.esc_id AS varchar(15)) AS EntityId, Esc.esc_nome AS Description ");
			sql.Append("FROM SGP_ESC_Escola Esc WITH(NOLOCK) ");

			if (!allAdhered)
			{
				sql.AppendLine("INNER JOIN Adherence Adh WITH(NOLOCK) ON Adh.EntityId = Esc.esc_id AND Adh.TypeEntity = @typeEntity AND Adh.Test_Id = @test_id ");
				sql.AppendLine("WHERE ent_id = @ent_id AND esc_situacao = @state AND Adh.State = @state ");
				if (esc_id != null)
					sql.AppendFormat("AND uad_id IN ({0}) ", string.Join(",", esc_id));
			}
			else
			{
				sql.AppendLine("LEFT JOIN Adherence Adh WITH(NOLOCK) ON Adh.EntityId = Esc.esc_id AND Adh.TypeEntity = @typeEntity AND Adh.Test_Id = @test_id AND Adh.TypeSelection = @typeSelection ");
				sql.AppendLine("WHERE Adh.Id IS NULL AND ent_id = @ent_id AND esc_situacao = @state ");
				if (esc_id != null)
					sql.AppendFormat("AND uad_id IN ({0}) ", string.Join(",", esc_id));
			}

			sql.AppendFormat("AND Esc.uad_idSuperiorGestao = @uad_idSuperiorGestao ");
			sql.Append("GROUP BY Esc.esc_id, Esc.esc_nome ");
			sql.Append("ORDER BY Esc.esc_nome");

			var p = new DynamicParameters();
			p.Add("@ent_id", ent_id);
			p.Add("@state", (byte)1);
			p.Add("@typeEntity", (byte)EnumAdherenceEntity.School);
			p.Add("@test_id", test_id);
			p.Add("@uad_idSuperiorGestao", uad_id);
			p.Add("@typeSelection", (byte)EnumAdherenceSelection.NotSelected);

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<AdheredEntityDTO>(sql.ToString(), p);
			}
		}

		public IEnumerable<AdheredEntityDTO> LoadSchoolSimpleTeacherAdherence(long test_id, Guid ent_id, Guid pes_id, Guid uad_id, bool allAdhered)
		{
			var sql = new StringBuilder("SELECT CAST(e.esc_id AS varchar(15)) AS EntityId, e.esc_nome AS Description ");
			sql.AppendLine("FROM SGP_ACA_Docente (NOLOCK) d ");
			sql.AppendLine("INNER JOIN SGP_TUR_TurmaDocente (NOLOCK) tdoc ON d.doc_id = tdoc.doc_id AND tdoc.tdt_situacao = @state ");
			sql.AppendLine("INNER JOIN SGP_TUR_TurmaDisciplina (NOLOCK) td ON td.tud_id = tdoc.tud_id AND td.tud_situacao = @state ");
			sql.AppendLine("INNER JOIN SGP_TUR_Turma t (NOLOCK) ON t.tur_id = td.tur_id AND t.tur_situacao = @state ");
			sql.AppendLine("INNER JOIN SGP_ESC_Escola e (NOLOCK) ON e.esc_id = t.esc_id AND e.esc_situacao = @state ");

			if (!allAdhered)
			{
				sql.AppendLine("INNER JOIN Adherence Adh WITH(NOLOCK) ON Adh.EntityId = e.esc_id AND Adh.TypeEntity = @typeEntity AND Adh.Test_Id = @test_id AND Adh.State = @state ");
			}
			sql.AppendLine("AND d.doc_situacao = @state ");
			sql.AppendLine("AND e.uad_idSuperiorGestao = @uad_id  AND d.ent_id = @ent_id AND pes_id = @pes_id ");
			sql.AppendLine("GROUP BY e.esc_id, e.esc_nome ");
			sql.AppendLine("ORDER BY e.esc_nome ");

			var p = new DynamicParameters();
			p.Add("@ent_id", ent_id);
			p.Add("@pes_id", pes_id);
			p.Add("@state", (byte)1);
			p.Add("@typeEntity", (byte)EnumAdherenceEntity.School);
			p.Add("@test_id", test_id);
			p.Add("@uad_id", uad_id);
			p.Add("@typeSelection", (byte)EnumAdherenceSelection.NotSelected);

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<AdheredEntityDTO>(sql.ToString(), p);
			}
		}

		public List<TeamsDTO> GetInfoTeams(List<long?> turmas)
		{
			var sql = new StringBuilder("SELECT tur_id, tur_codigo ");
			sql.Append("FROM dbo.SGP_TUR_Turma WITH(NOLOCK) ");
			sql.AppendFormat("WHERE tur_id IN ({0}) ", string.Join(",", turmas.ToArray()));

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<TeamsDTO>(sql.ToString()).ToList();
			}
		}

		#endregion

		#region Persistence
		public void RemoveByTest(long test_id)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				var adherences = GestaoAvaliacaoContext.Adherence.Where(a => a.Test.Id == test_id).ToList();

				foreach (var item in adherences)
				{
					GestaoAvaliacaoContext.Entry(item).State = System.Data.Entity.EntityState.Deleted;

					GestaoAvaliacaoContext.SaveChanges();
				}
			}
		}

		public void RemoveByIds(long test_id, IEnumerable<long> ids, EnumAdherenceEntity entityType)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				foreach (var id in ids)
				{
					var adherence = GestaoAvaliacaoContext.Adherence.FirstOrDefault(a => a.Test.Id == test_id && a.TypeEntity == entityType && a.EntityId == id);

					if (adherence != null)
					{
						GestaoAvaliacaoContext.Entry(adherence).State = System.Data.Entity.EntityState.Deleted;

						GestaoAvaliacaoContext.SaveChanges();
					}
				}
			}
		}

		public void RemoveById(long test_id, long id, EnumAdherenceEntity entityType)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				var adherence = GestaoAvaliacaoContext.Adherence.FirstOrDefault(a => a.Test.Id == test_id && a.TypeEntity == entityType && a.EntityId == id);

				if (adherence != null)
				{
					GestaoAvaliacaoContext.Entry(adherence).State = System.Data.Entity.EntityState.Deleted;

					GestaoAvaliacaoContext.SaveChanges();
				}
			}
		}

		public void Save(List<Adherence> adherences)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				foreach (var adherence in adherences)
					GestaoAvaliacaoContext.Adherence.Add(adherence);

				GestaoAvaliacaoContext.SaveChanges();
			}
		}

		public void Insert(Adherence adherence)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				GestaoAvaliacaoContext.Adherence.Add(adherence);

				GestaoAvaliacaoContext.SaveChanges();
			}

		}

		public void Save(long test_id, IEnumerable<long> ids, EnumAdherenceEntity entityType, EnumAdherenceSelection typeSelection, long parentId = 0)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				foreach (var id in ids)
				{
					var adherence = GestaoAvaliacaoContext.Adherence.FirstOrDefault(a => a.Test_Id == test_id && a.TypeEntity == entityType
						&& a.EntityId == id);

					if (adherence == null)
					{
						adherence = new Adherence()
						{
							CreateDate = DateTime.Now,
							EntityId = id,
							State = (byte)EnumState.ativo,
							Test_Id = test_id,
							TypeEntity = entityType,
							TypeSelection = typeSelection,
							UpdateDate = DateTime.Now
						};
						if (parentId > 0)
							adherence.ParentId = parentId;
						GestaoAvaliacaoContext.Adherence.Add(adherence);
					}
					else
					{
						adherence.State = (byte)EnumState.ativo;
						adherence.TypeSelection = typeSelection;
						adherence.UpdateDate = DateTime.Now;
						adherence.Test_Id = test_id;

						if (parentId > 0)
							adherence.ParentId = parentId;

						GestaoAvaliacaoContext.Entry(adherence).State = System.Data.Entity.EntityState.Modified;
					}
					GestaoAvaliacaoContext.SaveChanges();
				}
			}
		}

		public void Save(long test_id, long id, EnumAdherenceEntity entityType, EnumAdherenceSelection typeSelection, long parentId = 0)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				var adherence = GestaoAvaliacaoContext.Adherence.FirstOrDefault(a => a.Test_Id == test_id && a.TypeEntity == entityType
					&& a.EntityId == id);

				if (adherence == null)
				{
					adherence = new Adherence()
					{
						CreateDate = DateTime.Now,
						EntityId = id,
						State = (byte)EnumState.ativo,
						Test_Id = test_id,
						TypeEntity = entityType,
						TypeSelection = typeSelection,
						UpdateDate = DateTime.Now
					};
					if (parentId > 0)
						adherence.ParentId = parentId;
					GestaoAvaliacaoContext.Adherence.Add(adherence);
				}
				else
				{
					adherence.State = (byte)EnumState.ativo;
					adherence.TypeSelection = typeSelection;
					adherence.UpdateDate = DateTime.Now;
					adherence.Test_Id = test_id;

					if (parentId > 0)
						adherence.ParentId = parentId;

					GestaoAvaliacaoContext.Entry(adherence).State = System.Data.Entity.EntityState.Modified;
				}
				GestaoAvaliacaoContext.SaveChanges();
			}
		}

		#endregion
	}
}
