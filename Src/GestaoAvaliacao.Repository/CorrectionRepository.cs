using Dapper;
using GestaoAvaliacao.Entities;
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
    public class CorrectionRepository : ConnectionReadOnly, ICorrectionRepository
    {
        public IEnumerable<SelectedSection> LoadOnlySelectedSectionPaginate(ref Pager pager, StudentResponseFilter filter)
        {
            var tables = new StringBuilder("FROM SGP_TUR_Turma t WITH (NOLOCK) ");
            var where = new StringBuilder();

            tables.AppendLine("INNER JOIN SGP_ACA_TipoTurno ttn WITH (NOLOCK) ON t.ttn_id = ttn.ttn_id AND ttn.ttn_situacao = @state ");

            if (filter.ttn_id.HasValue && filter.ttn_id > 0)
                tables.AppendLine("AND ttn.ttn_id = @ttn_id ");

            tables.AppendLine("INNER JOIN SGP_ACA_CalendarioAnual cal WITH(NOLOCK) ON t.cal_id = cal.cal_id ");
            tables.AppendLine("INNER JOIN Test te WITH(NOLOCK) ON te.Id = @test_id AND YEAR(te.ApplicationStartDate) = cal.cal_ano ");

            tables.Append("INNER JOIN SGP_ESC_Escola e WITH (NOLOCK) ON e.esc_id = t.esc_id AND e.esc_situacao = @state ");
            if (filter.School_Id > 0)
                tables.AppendLine(" AND e.esc_id = @esc_id ");

            if (filter.uad_id.HasValue && !filter.uad_id.Equals(Guid.Empty))
                tables.AppendLine(" AND e.uad_idSuperiorGestao = @uad_id ");

            tables.AppendLine("INNER JOIN [SGP_SYS_UnidadeAdministrativa] uad WITH (NOLOCK) ON uad.[uad_id] = e.[uad_idSuperiorGestao] AND uad.uad_situacao = @state ");
            tables.AppendLine("LEFT JOIN [File] f WITH (NOLOCK) ON f.[OwnerId] = t.[tur_id] AND f.[ParentOwnerId] = @test_id AND f.[State] = @state AND OwnerType = @ownerType");

            if (filter.crp_ordem.HasValue && filter.crp_ordem > 0)
            {
                tables.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo cpr WITH (NOLOCK) ON cpr.tur_id = t.tur_id AND cpr.crp_ordem = @crp_ordem AND cpr.ttcr_situacao = @state  ");
            }

            if (filter.pes_id.HasValue)
            {
                tables.AppendLine("INNER JOIN ( SELECT DISTINCT (tud.tur_id) FROM SGP_TUR_TurmaDisciplina tud WITH (NOLOCK) ");
                tables.AppendLine("INNER JOIN SGP_TUR_TurmaDocente tdt WITH (NOLOCK) ON tdt.tud_id = tud.tud_id AND tdt.tdt_situacao = @state ");
                tables.AppendLine("INNER JOIN SGP_ACA_Docente d WITH (NOLOCK) ON d.doc_id = tdt.doc_id AND d.doc_situacao = @state ");
                tables.AppendLine("WHERE tud.tud_situacao = @state AND d.pes_id = @pes_id ) ");
                tables.AppendLine("AS tud ON tud.tur_id = t.tur_id ");
            }

            //Se a prova for para todas as escolas selecionadas, remover as que estão salvas como não selecionadas
            if (filter.AllAdhered)
            {
                tables.AppendLine("LEFT JOIN Adherence a WITH (NOLOCK) ON a.EntityId = t.tur_id AND a.Test_Id = @test_id AND a.TypeEntity = @typeEntity AND a.TypeSelection = @TypeSelection AND a.[State] = @state ");
                tables.AppendLine("LEFT JOIN TestSectionStatusCorrection tt WITH (NOLOCK) ON tt.Test_id = @test_id AND tt.tur_id = t.tur_id AND tt.[State] = @state ");
                tables.AppendLine("INNER JOIN SGP_TUR_TurmaCurriculo tc WITH(NOLOCK) ON tc.tur_id = t.tur_id ");
                tables.AppendLine("INNER JOIN SGP_ACA_CurriculoPeriodo crp WITH(NOLOCK) ON crp.cur_id = tc.cur_id AND crp.crr_id = tc.crr_id AND crp.crp_id = tc.crp_id ");
                where.AppendLine("WHERE a.Id IS NULL ");
                if (!string.IsNullOrEmpty(filter.StatusCorrection))
                {
                    where.AppendLine("AND ( tt.StatusCorrection IN (SELECT Items FROM dbo.SplitString('" + filter.StatusCorrection + "',',')) ");
                    if (filter.StatusCorrection.Contains("0"))
                    {
                        where.AppendLine("OR tt.StatusCorrection IS NULL ) ");
                    }
                    else
                    {
                        where.AppendLine(" )");
                    }
                }
                where.AppendLine("AND crp.tcp_id IN (SELECT TypeCurriculumGradeId FROM @table) ");
                if (DateTime.Now > filter.CorrectionEndDate)
                    where.AppendLine("AND crp.crp_situacao <> @state_excluido AND tc.tcr_situacao <> @state_excluido ");
                else
                    where.AppendLine("AND crp.crp_situacao = @state AND tc.tcr_situacao = @state ");
            }
            //Senão, trazer apenas as que estão salvas como selecionadas/parcial
            else
            {
                tables.AppendLine("INNER JOIN Adherence a WITH (NOLOCK) ON a.EntityId = t.tur_id AND a.Test_Id = @test_id AND a.[State] = @state ");
                tables.AppendLine("LEFT JOIN TestSectionStatusCorrection tt WITH (NOLOCK) ON tt.Test_id = @test_id AND tt.tur_id = t.tur_id AND tt.[State] = @state ");
                where.AppendLine("WHERE a.TypeEntity = @typeEntity ");
                if (!string.IsNullOrEmpty(filter.StatusCorrection))
                {
                    where.AppendLine("AND ( tt.StatusCorrection IN (SELECT Items FROM dbo.SplitString('" + filter.StatusCorrection + "',',')) ");
                    if (filter.StatusCorrection.Contains("0"))
                    {
                        where.AppendLine("OR tt.StatusCorrection IS NULL ) ");
                    }
                    else
                    {
                        where.AppendLine(" )");
                    }
                }
            }

            if (filter.usu_id.HasValue && !filter.usu_id.Equals(Guid.Empty) && filter.vis_id.HasValue && filter.vis_id > 0 && filter.sis_id.HasValue && filter.sis_id > 0 && ((EnumSYS_Visao)filter.vis_id != EnumSYS_Visao.Administracao))
            {
                tables.AppendLine("INNER JOIN Synonym_Core_SYS_Grupo G ON G.vis_id = @vis_id AND G.sis_id = @sis_id AND G.gru_situacao = @state ");
                tables.AppendLine("INNER JOIN Synonym_Core_SYS_UsuarioGrupo UG ON UG.gru_id = G.gru_id AND UG.usu_id = @usu_id AND UG.usg_situacao = @state ");
                tables.AppendLine("INNER JOIN Synonym_Core_SYS_UsuarioGrupoUA UGUA ON UGUA.gru_id = UG.gru_id AND UGUA.usu_id = UG.usu_id ");

                if ((EnumSYS_Visao)filter.vis_id == EnumSYS_Visao.UnidadeAdministrativa)
                {
                    tables.AppendLine("AND UGUA.uad_id = E.uad_id ");
                }
                else if ((EnumSYS_Visao)filter.vis_id == EnumSYS_Visao.Gestao)
                {
                    tables.AppendLine("AND UGUA.uad_id = E.uad_idSuperiorGestao ");
                }
            }

            if (filter.School_Id > 0)
                where.AppendLine(" AND t.esc_id = @esc_id ");

            if (filter.crp_ordem.HasValue && filter.crp_ordem > 0)
                where.AppendLine(" AND cpr.crp_ordem = @crp_ordem ");

            if (DateTime.Now > filter.CorrectionEndDate)
                where.AppendLine(" AND t.tur_situacao <> @state_excluido ");
            else
                where.AppendLine(" AND t.tur_situacao = @state ");

            var sql = new StringBuilder("DECLARE @table AS TABLE (TypeCurriculumGradeId BIGINT) ");
            sql.AppendLine("INSERT INTO @table ");
            sql.AppendLine("SELECT [TypeCurriculumGradeId] ");
            sql.AppendLine("FROM TestCurriculumGrade WITH(NOLOCK) ");
            sql.AppendLine("WHERE Test_Id = @test_id AND [State] = @state ");

            sql.AppendLine(";WITH CounteredSections AS ( ");
            sql.AppendLine("SELECT t.tur_id, t.tur_codigo, ttn.ttn_nome, ISNULL(tt.StatusCorrection, 0) AS StatusCorrection, e.uad_idSuperiorGestao AS dre_id, t.esc_id, e.esc_nome, uad.uad_nome, ");
            sql.AppendLine("ROW_NUMBER() OVER (ORDER BY t.tur_codigo, ttn.ttn_nome ASC) AS RowNumber ");
            sql.AppendLine(", f.[Id] AS FileId,f.[Name] AS FileName,f.[OriginalName] AS FileOriginalName,f.[Path] AS FilePath ");
            sql.AppendLine(tables.ToString());
            sql.AppendLine(where.ToString());
            sql.AppendLine(" ) ");
            sql.AppendLine("SELECT tur_id, tur_codigo, ttn_nome, StatusCorrection, dre_id, esc_id, esc_nome, uad_nome ");
            sql.AppendLine(", FileId, FileName, FileOriginalName, FilePath ");
            sql.AppendLine("FROM CounteredSections ");
            sql.AppendLine("WHERE RowNumber > ( @pageSize * @page ) ");
            sql.AppendLine("AND RowNumber <= ( ( @page + 1 ) * @pageSize )  ");
            sql.AppendLine("ORDER BY RowNumber ");

            sql.AppendLine("SELECT COUNT(t.tur_id) ");
            sql.AppendLine(tables.ToString());
            sql.AppendLine(where.ToString());


            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(), new
                {
                    esc_id = filter.School_Id,
                    state = EnumState.ativo,
                    state_excluido = EnumState.excluido,
                    pes_id = filter.pes_id,
                    test_id = filter.Test_Id,
                    typeEntity = EnumAdherenceEntity.Section,
                    pageSize = pager.PageSize,
                    page = pager.CurrentPage,
                    TypeSelection = EnumAdherenceSelection.NotSelected,
                    testType = filter.TestType_Id,
                    crp_ordem = filter.crp_ordem,
                    ttn_id = filter.ttn_id,
                    uad_id = filter.uad_id,
                    vis_id = filter.vis_id,
                    sis_id = filter.sis_id,
                    usu_id = filter.usu_id,
                    statusCorrection = filter.StatusCorrection,
                    ownerType = EnumFileType.AnswerSheetQRCode
                });

                var retorno = query.Read<SelectedSection>();
                int count = query.Read<int>().FirstOrDefault();

                pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));
                pager.SetTotalItens(count);

                return retorno;
            }
        }

        public IEnumerable<TestTemplate> GetTestTemplate(long test_id)
        {
            var sql = new StringBuilder("SELECT (DENSE_RANK() OVER(ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS [Order], i.Id AS Item_Id, a.Id AS Alternative_Id, a.Numeration, i.KnowledgeArea_Id, k.[Description] AS KnowledgeArea_Description ");
            sql.Append("FROM Test t WITH (NOLOCK) ");
            sql.Append("INNER JOIN Block b WITH (NOLOCK) ON b.Test_Id = t.Id ");
            sql.Append("INNER JOIN BlockItem bi WITH (NOLOCK) ON bi.Block_Id = b.Id ");
            sql.Append("INNER JOIN Item i WITH (NOLOCK) ON i.Id = bi.Item_Id ");
            sql.Append("INNER JOIN Alternative a WITH (NOLOCK) ON a.Item_Id = i.Id AND a.Correct = @correct ");
            sql.Append("LEFT JOIN KnowledgeArea K WITH(NOLOCK) ON i.KnowledgeArea_Id = K.Id AND K.State = @state ");
            sql.Append("LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = i.KnowledgeArea_Id AND B.Id = Bka.Block_Id AND Bka.State = @state ");
            sql.Append("WHERE t.Id = @test_id AND t.State = @state AND b.State = @state AND bi.State = @state AND i.State = @state AND a.State = @state ");
            sql.Append("ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order] ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var retorno = cn.Query<TestTemplate>(sql.ToString(), new
                {
                    state = (byte)EnumState.ativo,
                    test_id = test_id,
                    correct = true
                });

                return retorno;
            }
        }
    }
}