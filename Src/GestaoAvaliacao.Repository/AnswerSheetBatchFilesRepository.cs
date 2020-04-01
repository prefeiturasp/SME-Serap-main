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
    public class AnswerSheetBatchFilesRepository : ConnectionReadOnly, IAnswerSheetBatchFilesRepository
	{
		#region Read

		public IEnumerable<AnswerSheetBatchResult> SearchBatchFiles(ref Pager pager, AnswerSheetBatchFilter filter, string nomeAluno, string turma)
		{
			if (filter.CoreVisionId == (int)EnumSYS_Visao.Gestao || filter.CoreVisionId == (int)EnumSYS_Visao.UnidadeAdministrativa)
			{
				filter.UadList = GetUserPermission(filter);
				if (filter.UadList == null || (filter.UadList != null && filter.UadList.Count() <= 0))
					return null;
			}

			StringBuilder sql = new StringBuilder();

            sql.AppendLine("DECLARE @TBDados AS TABLE ( ");
            sql.AppendLine("Id BIGINT, ");
            sql.AppendLine("CreateDate DATETIME, ");
            sql.AppendLine("UpdateDate DATETIME, ");
            sql.AppendLine("[Sent] BIT, ");
            sql.AppendLine("Processing TINYINT, ");
            sql.AppendLine("SectionId_BatchFile BIGINT, ");
            sql.AppendLine("SchoolId_BatchFile BIGINT, ");
            sql.AppendLine("[Description] VARCHAR(500), ");
            sql.AppendLine("[Test_Id] BIGINT, ");
            sql.AppendLine("SupAdmUnitId UNIQUEIDENTIFIER, ");
            sql.AppendLine("SchoolId INT, ");
            sql.AppendLine("SectionId BIGINT, ");
            sql.AppendLine("[OwnerEntity] TINYINT, ");
            sql.AppendLine("[Situation] TINYINT, ");
            sql.AppendLine("[FileName] VARCHAR(500), ");
            sql.AppendLine("FileId BIGINT, ");
            sql.AppendLine("FilePath VARCHAR(500), ");
            sql.AppendLine("[Student_Id] BIGINT, ");
            sql.AppendLine("StudentName VARCHAR(500), ");
            sql.AppendLine("SchoolName VARCHAR(500), ");
            sql.AppendLine("UadSigla VARCHAR(100), ");
            sql.AppendLine("SupAdmUnitName VARCHAR(500), ");
            sql.AppendLine("SectionName VARCHAR(500), ");
            sql.AppendLine("Resolution VARCHAR(100), ");
            sql.AppendLine("RowNumber INT ");
            sql.AppendLine(") ");

            if (!string.IsNullOrEmpty(filter.Processing))
            {
                sql.AppendLine("DECLARE @Situation AS TABLE (Situation INT NOT NULL) ");
                sql.AppendLine("INSERT INTO @Situation(Situation) ");
                sql.AppendLine("SELECT Items FROM dbo.SplitString('" + filter.Processing + "',',') ");
            }

            sql.AppendLine("; WITH NumberedResult AS ");
            sql.AppendLine("( ");
            sql.AppendLine("SELECT DISTINCT BF.[Id], BF.[CreateDate], BF.[UpdateDate], BF.[File_Id], BF.[Sent], BF.[Situation] AS Processing, BF.[Section_Id] AS SectionId_BatchFile, BF.[School_Id] AS SchoolId_BatchFile, BF.[Student_Id] ");
            sql.AppendLine(", B.[Description], B.[Test_Id], B.[SupAdmUnit_Id] AS SupAdmUnitId, B.[School_Id] AS SchoolId, B.[Section_Id] AS SectionId, B.[OwnerEntity] ");
            sql.AppendLine(", BF.[Situation]  ");
            sql.AppendLine(string.Format(", ROW_NUMBER() OVER (ORDER BY {0} DESC) AS RowNumber ", filter.FilterDateUpdate ? "BF.[UpdateDate]" : "BF.[CreateDate]"));
            sql.AppendLine("FROM [AnswerSheetBatchFiles] BF WITH (NOLOCK) ");
            if (!string.IsNullOrEmpty(filter.Processing) && !filter.ShowAllStudentsLot)
            {
                sql.AppendLine("INNER JOIN @Situation AS Sit ON BF.[Situation] = Sit.[Situation] ");
            }
            sql.Append(GetInner(filter));
            sql.Append(GetWhere(filter));
            sql.AppendLine(") ");

            #region AnswerSheetBatchResult
            sql.AppendLine("INSERT INTO @TBDados ");
            sql.AppendLine("SELECT DISTINCT NR.Id, NR.CreateDate, NR.UpdateDate, NR.Sent, NR.Processing, NR.SectionId_BatchFile, NR.SchoolId_BatchFile ");
            sql.AppendLine(", NR.Description, NR.Test_Id, NR.SupAdmUnitId, NR.SchoolId, T.tur_id AS SectionId, NR.OwnerEntity ");
            sql.AppendLine(", NR.Situation ");
            sql.AppendLine(", F.[OriginalName] AS FileName, F.[Id] AS FileId, F.[Path] AS FilePath ");
            sql.AppendLine(", S.[alu_id] AS StudentId, S.[alu_nome] AS StudentName ");
            sql.AppendLine(", E.[esc_nome] AS SchoolName ");
            sql.AppendLine(", UAD.[uad_sigla] AS UadSigla ");
            sql.AppendLine(", CASE WHEN UAD.[uad_sigla] IS NOT NULL THEN UAD.[uad_sigla] + ' - ' + UAD.[uad_nome] ELSE UAD.[uad_nome] END AS SupAdmUnitName ");
            sql.AppendLine(", T.[tur_codigo] + ' - ' + TN.[ttn_nome] AS SectionName ");
            sql.AppendLine(", (CASE WHEN(ISNULL(F.HorizontalResolution, 0) = 0 OR ISNULL(F.VerticalResolution, 0) = 0) THEN NULL ELSE CONVERT(VARCHAR, CAST(F.HorizontalResolution AS INT), 0) +' x ' + CONVERT(VARCHAR, CAST(F.VerticalResolution AS INT), 0) END) AS Resolution ");
            sql.AppendLine(", NR.RowNumber");
            sql.AppendLine("FROM NumberedResult NR ");
            sql.AppendLine("INNER JOIN [File] F WITH (NOLOCK) ON F.[Id] = NR.[File_Id] AND F.[State] = @State  ");
            sql.AppendLine("LEFT JOIN [SGP_ACA_Aluno] S WITH (NOLOCK) ON S.[alu_id] = NR.[Student_Id] ");
            sql.AppendLine("LEFT JOIN [SGP_ESC_Escola] E WITH (NOLOCK) ON E.[esc_id] = NR.[SchoolId_BatchFile] AND E.[esc_situacao] = @State ");
            sql.AppendLine("LEFT JOIN [SGP_SYS_UnidadeAdministrativa] UAD WITH (NOLOCK) ON UAD.[uad_id] = E.[uad_idSuperiorGestao] AND UAD.[uad_situacao] = @State ");
            sql.AppendLine("LEFT JOIN [SGP_TUR_Turma] T WITH (NOLOCK) ON T.[tur_id] = NR.[SectionId_BatchFile] AND T.[tur_situacao] = @State ");
            sql.AppendLine("LEFT JOIN [SGP_ACA_TipoTurno] TN WITH (NOLOCK) ON TN.[ttn_id] = T.[ttn_id] AND TN.[ttn_situacao] = @State ");
            sql.AppendLine("WHERE (@AluNome IS NULL OR S.alu_nome LIKE '%' + @AluNome + '%')");
            sql.AppendLine("AND (@TurCodigo IS NULL OR T.tur_codigo LIKE '%' + @TurCodigo + '%')");

            if (pager != null)
			{
                sql.AppendLine("SELECT ");
                sql.AppendLine("Id, ");
                sql.AppendLine("CreateDate, ");
                sql.AppendLine("UpdateDate, ");
                sql.AppendLine("[Sent], ");
                sql.AppendLine("Processing, ");
                sql.AppendLine("SectionId_BatchFile, ");
                sql.AppendLine("SchoolId_BatchFile, ");
                sql.AppendLine("[Description], ");
                sql.AppendLine("[Test_Id], ");
                sql.AppendLine("SupAdmUnitId, ");
                sql.AppendLine("SchoolId, ");
                sql.AppendLine("SectionId, ");
                sql.AppendLine("[OwnerEntity], ");
                sql.AppendLine("[Situation], ");
                sql.AppendLine("[FileName], ");
                sql.AppendLine("FileId, ");
                sql.AppendLine("FilePath, ");
                sql.AppendLine("[Student_Id], ");
                sql.AppendLine("StudentName, ");
                sql.AppendLine("SchoolName, ");
                sql.AppendLine("UadSigla, ");
                sql.AppendLine("SupAdmUnitName, ");
                sql.AppendLine("SectionName, ");
                sql.AppendLine("Resolution, RowNumber FROM @TBDados ");
                sql.AppendLine("WHERE RowNumber > ( @pageSize * @page ) ");
				sql.AppendLine("AND RowNumber <= ( ( @page + 1 ) * @pageSize ) ");
                sql.AppendLine("ORDER BY StudentName, [Situation] ");
            }

			#endregion

			#region Count

			if (pager != null)
			{
				sql.AppendLine("SELECT COUNT(Id) FROM @TBDados");
            }

			#endregion

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var query = cn.QueryMultiple(sql.ToString(),
					new
					{
						State = EnumState.ativo,
						SupAdmUnit_Id = filter.SupAdmUnitId,
						School_Id = filter.SchoolId,
						Section_Id = filter.SectionId,
						StartDate = filter.StartDate,
						EndDate = filter.EndDate,
						BatchId = filter.BatchId,
                        BatchQueueId = filter.BatchQueueId,
						CurriculumTypeId = filter.CurriculumTypeId,
						ShiftTypeId = filter.ShiftTypeId,
						Test_Id = filter.TestId,
						UserId = filter.UserId,
                        AluNome = nomeAluno,
                        TurCodigo = turma,
						pageSize = pager != null ? pager.PageSize : 0,
						page = pager != null ? pager.CurrentPage : 0
					});

				var batches = query.Read<AnswerSheetBatchResult>();

				if (pager != null)
				{
					var count = query.Read<int>().FirstOrDefault();
					pager.SetTotalItens(count);
					pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));
				}

				return batches;
			}
		}

		public AnswerSheetBatchFileCountResult GetCountBatchInformation(AnswerSheetBatchFilter filter)
		{
			AnswerSheetBatchFileCountResult entity = new AnswerSheetBatchFileCountResult();

			if (filter.CoreVisionId == (int)EnumSYS_Visao.Gestao || filter.CoreVisionId == (int)EnumSYS_Visao.UnidadeAdministrativa)
			{
				filter.UadList = GetUserPermission(filter);
				if (filter.UadList == null || (filter.UadList != null && filter.UadList.Count() <= 0))
					return new AnswerSheetBatchFileCountResult();
			}

			var sqlMulti = new StringBuilder("");
            if (!string.IsNullOrEmpty(filter.Processing))
            {
                sqlMulti.AppendLine("DECLARE @Situation AS TABLE (Situation INT NOT NULL) ");
                sqlMulti.AppendLine("INSERT INTO @Situation(Situation) ");
                sqlMulti.AppendLine("SELECT Items FROM dbo.SplitString('" + filter.Processing + "',',') ");
            }
            sqlMulti.AppendLine(";WITH Result AS (");
			sqlMulti.AppendLine("SELECT BF.Situation, COUNT(DISTINCT ISNULL(BF.Student_Id, BF.Id)) as SituationCount ");
			sqlMulti.AppendLine("FROM [AnswerSheetBatchFiles] BF WITH (NOLOCK) ");
            if (!string.IsNullOrEmpty(filter.Processing))
            {
                sqlMulti.AppendLine("INNER JOIN @Situation AS Sit ON BF.[Situation] = Sit.[Situation] ");
            }
            sqlMulti.AppendLine(GetInner(filter));
			sqlMulti.AppendLine(GetWhere(filter));
			sqlMulti.AppendLine("GROUP BY BF.Situation ");
			sqlMulti.AppendLine(") ");
			sqlMulti.AppendLine("SELECT Situation, SituationCount, SUM(SituationCount) OVER() AS TotalResult FROM [Result] WITH (NOLOCK) ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var files = cn.QueryMultiple(
					sqlMulti.ToString(),
					 new
					 {
						 State = EnumState.ativo,
						 SupAdmUnit_Id = filter.SupAdmUnitId,
						 School_Id = filter.SchoolId,
						 Section_Id = filter.SectionId,
						 StartDate = filter.StartDate,
						 EndDate = filter.EndDate,
						 BatchId = filter.BatchId,
						 CurriculumTypeId = filter.CurriculumTypeId,
						 ShiftTypeId = filter.ShiftTypeId,
						 Test_Id = filter.TestId,
						 UserId = filter.UserId,
                         BatchQueueId = filter.BatchQueueId
					 });

				var listFiles = files.Read<AnswerSheetBatchFileCountDTO>();

				var count = listFiles.FirstOrDefault();
				entity.Total = count != null ? count.TotalResult : 0;

				if (listFiles.Any())
				{
					entity.Errors = listFiles.Where(i => i.Situation == (int)EnumBatchSituation.Error).Select(s => s.SituationCount).FirstOrDefault();
					entity.Warnings = listFiles.Where(i => i.Situation == (int)EnumBatchSituation.Warning).Select(s => s.SituationCount).FirstOrDefault();
					entity.Success = listFiles.Where(i => i.Situation == (int)EnumBatchSituation.Success).Select(s => s.SituationCount).FirstOrDefault();
					entity.PendingIdentification = listFiles.Where(i => i.Situation == (int)EnumBatchSituation.PendingIdentification).Select(s => s.SituationCount).FirstOrDefault();
					entity.NotIdentified = listFiles.Where(i => i.Situation == (int)EnumBatchSituation.NotIdentified).Select(s => s.SituationCount).FirstOrDefault();
					entity.Pending = listFiles.Where(i => i.Situation == (int)EnumBatchSituation.Pending).Select(s => s.SituationCount).FirstOrDefault();
                    entity.Absents = listFiles.Where(i => i.Situation == (int)EnumBatchSituation.Absent).Select(s => s.SituationCount).FirstOrDefault();
                }
			}

			return entity;
		}

		public IEnumerable<AnswerSheetBatchFileResult> GetBatchFiles(long batchId, bool sent, int rows)
		{
			StringBuilder sql = new StringBuilder("SELECT ");

			if (rows > 0)
				sql.AppendFormat("TOP {0} ", rows);

			sql.Append("BF.[Id],F.[Id] AS FileId,F.[Name] AS FileName,F.[OriginalName] AS FileOriginalName,F.[ContentType] AS FileContentType,F.[Path] AS FilePath ");
			sql.Append("FROM [AnswerSheetBatchFiles] BF WITH (NOLOCK) ");
			sql.Append("INNER JOIN [File] F WITH (NOLOCK) ON F.[Id] = BF.[File_Id] ");
			sql.Append("WHERE BF.[State] <> @State AND F.[State] <> @State AND BF.[Sent] = @Sent ");

			if (batchId > 0)
				sql.Append("AND BF.[AnswerSheetBatch_Id] = @Id ");
			sql.Append("ORDER BY BF.CreateDate ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = cn.Query<AnswerSheetBatchFileResult>(sql.ToString(), new { State = (Byte)EnumState.excluido, Id = batchId, Sent = sent });

				return result;
			}
		}

		public IEnumerable<AnswerSheetBatchFileResult> GetBatchFiles(EnumBatchSituation situation, int rows)
		{
			StringBuilder sql = new StringBuilder("SELECT ");

			if (rows > 0)
				sql.AppendFormat("TOP {0} ", rows);

			sql.Append("BF.[Id],F.[Id] AS FileId,F.[Name] AS FileName,F.[OriginalName] AS FileOriginalName,F.[ContentType] AS FileContentType,F.[Path] AS FilePath ");
			sql.Append("FROM [AnswerSheetBatchFiles] BF WITH (NOLOCK) ");
			sql.Append("INNER JOIN [File] F WITH (NOLOCK) ON F.[Id] = BF.[File_Id] ");
			sql.Append("WHERE BF.[State] <> @State AND F.[State] <> @State ");

			if (situation.Equals(EnumBatchSituation.PendingIdentification))
				sql.Append("AND BF.[Sent] = 0 ");

			if (situation > 0)
				sql.Append("AND BF.[Situation] = @Situation ");
			sql.Append("ORDER BY BF.CreateDate ");
			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = cn.Query<AnswerSheetBatchFileResult>(sql.ToString(), new { State = (Byte)EnumState.excluido, Situation = situation });

				return result;
			}
		}

		public int GetFilesCount(long batchId)
		{
			StringBuilder sql = new StringBuilder("SELECT COUNT(Id) ");
			sql.Append("FROM [AnswerSheetBatchFiles] WITH (NOLOCK) ");
			sql.Append("WHERE [State] <> @State ");
			if (batchId > 0)
				sql.Append("AND [AnswerSheetBatch_Id] = @Id ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = (int)cn.ExecuteScalar(sql.ToString(), new { State = (Byte)EnumState.excluido, Id = batchId });

				return result;
			}
		}

		public int GetFilesNotSentCount(long batchId)
		{
			StringBuilder sql = new StringBuilder("SELECT COUNT(Id) ");
			sql.Append("FROM [AnswerSheetBatchFiles] WITH (NOLOCK) ");
			sql.Append("WHERE [State] <> @State ");
			sql.Append("AND  [Sent] = 0 ");
			if (batchId > 0)
				sql.Append("AND [AnswerSheetBatch_Id] = @Id ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = (int)cn.ExecuteScalar(sql.ToString(), new { State = (Byte)EnumState.excluido, Id = batchId });

				return result;
			}
		}

		public IEnumerable<AnswerSheetBatchFiles> GetFiles(long batchId, bool excludeErrorFiles)
		{
			StringBuilder sql = new StringBuilder("SELECT F.Id, F.File_Id, F.AnswerSheetBatch_Id, F.Student_Id, F.Sent, F.CreateDate, F.UpdateDate, F.State, F.Situation, ");
			sql.AppendLine("F.Section_Id, F.SupAdmUnit_Id, F.School_Id ");
			sql.AppendLine("FROM [AnswerSheetBatchFiles] F WITH (NOLOCK) ");
			sql.AppendLine("WHERE F.[State] <> @State ");

			if (excludeErrorFiles)
			{
				sql.AppendLine("AND F.Situation <> @Error ");
			}

			if (batchId > 0)
			{
				sql.AppendLine("AND F.AnswerSheetBatch_Id = @Id ");
			}
			sql.AppendLine("ORDER BY F.CreateDate ");
			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = cn.Query<AnswerSheetBatchFiles>(sql.ToString(),
					new
					{
						State = (Byte)EnumState.excluido,
						Id = batchId,
						Error = (Byte)EnumBatchSituation.Error
					});

				return result;
			}
		}

		public AnswerSheetBatchFiles GetFile(long Id, long fileId)
		{
			StringBuilder sql = new StringBuilder("SELECT TOP 1 [Id],[File_Id],[AnswerSheetBatch_Id],[Student_Id] ");
			sql.Append(",[Section_Id],[Sent],[Situation],[CreateDate],[UpdateDate],[State] ");
			sql.Append("FROM [AnswerSheetBatchFiles] WITH (NOLOCK) ");
			sql.Append("WHERE [State] <> @State ");

			if (Id > 0)
				sql.Append("AND [Id] = @Id ");
			else if (fileId > 0)
				sql.Append("AND [File_Id] = @fileId ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = cn.Query<AnswerSheetBatchFiles>(sql.ToString(), new { State = (Byte)EnumState.excluido, Id = Id, fileId = fileId });

				return result.FirstOrDefault();
			}
		}

		public AnswerSheetBatchFileResult GetStudentFile(long testId, long studentId, long sectionId)
		{
			StringBuilder sql = new StringBuilder("SELECT TOP 1 BF.[Id],BF.[AnswerSheetBatch_Id] ");
			sql.AppendLine(",F.[Id] AS FileId,F.[Name] AS FileName,F.[OriginalName] AS FileOriginalName,F.[ContentType] AS FileContentType,F.[Path] AS FilePath ");
			sql.AppendLine("FROM [AnswerSheetBatchFiles] BF WITH (NOLOCK) ");
			sql.AppendLine("INNER JOIN [AnswerSheetBatch] B WITH (NOLOCK) ON B.[Id] = BF.[AnswerSheetBatch_Id]");
			sql.AppendLine("INNER JOIN [File] F WITH (NOLOCK) ON F.[Id] = BF.[File_Id] ");
			sql.AppendLine("WHERE BF.[State] <> @State  AND BF.[Student_Id] = @Student_Id AND BF.[Section_Id] = @Section_Id ");
			sql.AppendLine("AND B.[State] <> @State AND B.[Test_Id] = @Test_Id ");
			sql.AppendLine("AND BF.[Situation] = @Situation ");
			sql.AppendLine("AND F.[State] <> @State");
			sql.AppendLine("ORDER BY BF.[CreateDate] DESC ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = cn.Query<AnswerSheetBatchFileResult>(sql.ToString(),
					new
					{
						State = EnumState.excluido,
						Test_Id = testId,
						Student_Id = studentId,
						Section_Id = sectionId,
						Situation = EnumBatchSituation.Warning
					});

				return result.FirstOrDefault();
			}
		}

		public AnswerSheetBatchFiles Get(long Id)
		{
			StringBuilder sql = new StringBuilder("SELECT TOP 1 [Id],[File_Id],[AnswerSheetBatch_Id],[Student_Id],");
			sql.AppendLine("[Section_Id],[Sent],[Situation],[CreateDate],[UpdateDate],[State]");
			sql.AppendLine("FROM [AnswerSheetBatchFiles] WITH (NOLOCK)");
			sql.AppendLine("WHERE [State] <> @State AND [Id] = @Id");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = cn.Query<AnswerSheetBatchFiles>(sql.ToString(), new { State = (Byte)EnumState.excluido, Id = Id });

				return result.SingleOrDefault();
			}
		}

		public IEnumerable<AnswerSheetFollowUpIdentificationResult> GetIdentificationList(ref Pager pager, AnswerSheetBatchFilter filter)
		{
			if (filter.CoreVisionId == (int)EnumSYS_Visao.Gestao || filter.CoreVisionId == (int)EnumSYS_Visao.UnidadeAdministrativa)
			{
				filter.UadList = GetUserPermission(filter);
				if (filter.UadList == null || (filter.UadList != null && filter.UadList.Count() <= 0))
					return null;
			}

            StringBuilder sql = new StringBuilder();

			sql.AppendLine(";WITH NumberedResult AS (");
			sql.Append(@"SELECT R.[Id],
						   ISNULL(R.[TotalSent], 0) AS [TotalSent],
						   ISNULL(R.[TotalPendingIdentification], 0) AS [TotalPendingIdentification],
						   ISNULL(R.[TotalIdentified], 0) AS [TotalIdentified],
						   ISNULL(R.[TotalNotIdentified], 0) AS [TotalNotIdentified],
                           ISNULL(R.[TotalResolutionNotOk], 0) AS [TotalResolutionNotOk] ");
			sql.AppendLine(GetTotalResult(filter));
			sql.AppendLine("FROM ( ");
			sql.AppendLine(GetSelectByView(filter, (byte)EnumFollowUpIdentificationReportDataType.Sent));
			filter.Processing = string.Empty;
			sql.Append(GetWhereByView(filter));
			sql.Append(GetGroupByView(filter, (byte)EnumFollowUpIdentificationReportDataType.Sent));
			sql.AppendLine("UNION");
            sql.AppendLine(GetSelectByView(filter, (byte)EnumFollowUpIdentificationReportDataType.ResolutionNotOk));
            filter.Processing = string.Empty;
            sql.Append(GetWhereByView(filter, (byte)EnumFollowUpIdentificationReportDataType.ResolutionNotOk));
            sql.Append(GetGroupByView(filter, (byte)EnumFollowUpIdentificationReportDataType.ResolutionNotOk));
            sql.AppendLine("UNION");
            sql.AppendLine(GetSelectByView(filter, (byte)EnumFollowUpIdentificationReportDataType.Identified));
			var listSituationIdentified = new string[] { Convert.ToString((byte)EnumBatchSituation.Pending), Convert.ToString((byte)EnumBatchSituation.Success), Convert.ToString((byte)EnumBatchSituation.Error), Convert.ToString((byte)EnumBatchSituation.Warning) };
			filter.Processing = string.Join(",", listSituationIdentified);
			sql.Append(GetWhereByView(filter));
			sql.Append(GetGroupByView(filter, (byte)EnumFollowUpIdentificationReportDataType.Identified));
			sql.AppendLine("UNION");
			sql.AppendLine(GetSelectByView(filter, (byte)EnumFollowUpIdentificationReportDataType.NotIdentified));
			var listSituationNotIdentified = new string[] { Convert.ToString((byte)EnumBatchSituation.PendingIdentification), Convert.ToString((byte)EnumBatchSituation.NotIdentified) };
			filter.Processing = string.Join(",", listSituationNotIdentified);
			sql.Append(GetWhereByView(filter));
			sql.Append(GetGroupByView(filter, (byte)EnumFollowUpIdentificationReportDataType.NotIdentified));
			sql.AppendLine(@") f
								PIVOT (
								  SUM(Total) FOR
								  Situation IN ([TotalSent],[TotalPendingIdentification],[TotalIdentified],[TotalNotIdentified],[TotalResolutionNotOk])
								) AS R");
			sql.AppendLine(")");

			if (filter.View == EnumFollowUpIdentificationView.DRE || filter.View == EnumFollowUpIdentificationView.School)
			{
				sql.AppendLine(GetUAResult(filter));
				sql.AppendLine("SELECT Id, Name, TotalResult, TotalSent, TotalPendingIdentification, TotalIdentified, TotalNotIdentified, TotalResolutionNotOk, RowNumber FROM (");
			}

			sql.AppendLine(GetSelectNumberedResultReport(filter));
			sql.AppendLine(GetInnerByView(filter));

			if (filter.View == EnumFollowUpIdentificationView.DRE || filter.View == EnumFollowUpIdentificationView.School)
			{
				sql.AppendLine(") AS Result");
			}

			if (pager != null)
			{
				sql.AppendLine("WHERE RowNumber > ( @pageSize * @page ) ");
				sql.AppendLine("AND RowNumber <= ( ( @page + 1 ) * @pageSize ) ");
			}

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var query = cn.QueryMultiple(
					sql.ToString(),
					 new
					 {
						 State = EnumState.ativo,
						 StartDate = filter.StartDate,
						 EndDate = filter.EndDate,
						 SupAdmUnit_Id = filter.SupAdmUnitId,
						 School_Id = filter.SchoolId,
						 pageSize = pager != null ? pager.PageSize : 0,
						 page = pager != null ? pager.CurrentPage : 0,
						 UserId = filter.UserId,
						 VisionId = filter.CoreVisionId,
						 SystemId = filter.CoreSystemId
					 });

				var result = query.Read<AnswerSheetFollowUpIdentificationResult>();

				if (pager != null)
				{
					var entity = result.FirstOrDefault();
					var count = entity != null ? entity.TotalResult : 0;
					pager.SetTotalItens(count);
					pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));
				}

				return result;
			}
		}

		public IEnumerable<AnswerSheetFollowUpIdentificationResult> GetIdentificationFilesList(ref Pager pager, AnswerSheetBatchFilter filter)
		{
			if (filter.CoreVisionId == (int)EnumSYS_Visao.Gestao || filter.CoreVisionId == (int)EnumSYS_Visao.UnidadeAdministrativa)
			{
				filter.UadList = GetUserPermission(filter);
				if (filter.UadList == null || (filter.UadList != null && filter.UadList.Count() <= 0))
					return null;
			}

			StringBuilder sql = new StringBuilder();

			sql.AppendLine(";WITH NumberedResult AS (");
			sql.AppendLine(@"SELECT R.[Id], R.[Situation], R.[CreateDate], R.[SupAdmUnit_Id], R.[School_Id], R.[AnswerSheetBatch_Id], R.[RowNumber]");
			sql.AppendLine(GetTotalResult(filter));
			sql.AppendLine("FROM ( ");
			sql.AppendLine(GetSelectByView(filter, 0));
			sql.Append(GetWhereByView(filter));
			sql.AppendLine(") AS R");
			sql.AppendLine(")");
			sql.AppendLine(GetSelectNumberedResultReport(filter));
			sql.AppendLine(GetInnerByView(filter));
			if (pager != null)
			{
				sql.AppendLine("WHERE RowNumber > ( @pageSize * @page ) ");
				sql.AppendLine("AND RowNumber <= ( ( @page + 1 ) * @pageSize ) ");
			}

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var query = cn.QueryMultiple(
					sql.ToString(),
					 new
					 {
						 State = EnumState.ativo,
						 StartDate = filter.StartDate,
						 EndDate = filter.EndDate,
						 SupAdmUnit_Id = filter.SupAdmUnitId,
						 School_Id = filter.SchoolId,
						 pageSize = pager != null ? pager.PageSize : 0,
						 page = pager != null ? pager.CurrentPage : 0,
						 UserId = filter.UserId,
						 VisionId = filter.CoreVisionId,
						 SystemId = filter.CoreSystemId
					 });

				var result = query.Read<AnswerSheetFollowUpIdentificationResult>();

				if (pager != null)
				{
					var entity = result.FirstOrDefault();
					var count = entity != null ? entity.TotalResult : 0;
					pager.SetTotalItens(count);
					pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));
				}

				return result;
			}
		}

		public AnswerSheetFollowUpIdentification GetIdentificationReportInfo(AnswerSheetBatchFilter filter)
		{
			StringBuilder sql = new StringBuilder();

			if (filter.SchoolId != null)
			{
				sql.Append(@"SELECT UA.uad_id AS SupAdmUnit_Id, UA.uad_nome AS SupAdmUnitName, UA.uad_sigla AS SupAdmUnitInitials, E.esc_id AS SchoolId, E.esc_nome AS SchoolName
								FROM [SGP_ESC_Escola] E WITH (NOLOCK)
								INNER JOIN [SGP_SYS_UnidadeAdministrativa] UA WITH (NOLOCK) ON UA.uad_id = E.uad_idSuperiorGestao AND UA.uad_situacao = @State
								WHERE E.esc_id = @School_Id");
			}
			else
			{
				sql.Append(@"SELECT UA.uad_id AS SupAdmUnit_Id, UA.uad_nome AS SupAdmUnitName, UA.uad_sigla AS SupAdmUnitInitials, NULL AS SchoolId, NULL AS SchoolName
							FROM [SGP_SYS_UnidadeAdministrativa] UA WITH (NOLOCK)
							WHERE UA.uad_id = @SupAdmUnit_Id AND UA.uad_situacao = @State");
			}

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var query = cn.Query<AnswerSheetFollowUpIdentification>(
					sql.ToString(),
					 new
					 {
						 State = EnumState.ativo,
						 SupAdmUnit_Id = filter.SupAdmUnitId,
						 School_Id = filter.SchoolId,
					 });

				return query.FirstOrDefault();
			}   
		}

		#endregion

		#region Write

		public void SaveList(List<AnswerSheetBatchFiles> list)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				foreach (AnswerSheetBatchFiles entity in list)
				{
					GestaoAvaliacaoContext.AnswerSheetBatchFiles.Add(entity);
					GestaoAvaliacaoContext.SaveChanges();
				}
			}
		}

		public void Update(AnswerSheetBatchFiles entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				AnswerSheetBatchFiles file = GestaoAvaliacaoContext.AnswerSheetBatchFiles.FirstOrDefault(a => a.Id == entity.Id);
				if (file != null)
				{
					if (!file.AnswerSheetBatch_Id.Equals(entity.AnswerSheetBatch_Id))
						file.AnswerSheetBatch_Id = entity.AnswerSheetBatch_Id;

					if (!file.Sent.Equals(entity.Sent))
						file.Sent = entity.Sent;

					if (!file.Student_Id.Equals(entity.Student_Id))
						file.Student_Id = entity.Student_Id;

					if (!file.Section_Id.Equals(entity.Section_Id))
						file.Section_Id = entity.Section_Id;

					if (entity.School_Id != null && !file.School_Id.Equals(entity.School_Id))
						file.School_Id = entity.School_Id;

					if (entity.SupAdmUnit_Id != null && !file.SupAdmUnit_Id.Equals(entity.SupAdmUnit_Id))
						file.SupAdmUnit_Id = entity.SupAdmUnit_Id;

					if (!file.Situation.Equals(entity.Situation))
						file.Situation = entity.Situation;

					file.UpdateDate = DateTime.Now;

					GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Modified;
					GestaoAvaliacaoContext.SaveChanges();
				}
			}
		}

		public void UpdateList(List<AnswerSheetBatchFiles> list)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				foreach (AnswerSheetBatchFiles entity in list)
				{
					AnswerSheetBatchFiles file = GestaoAvaliacaoContext.AnswerSheetBatchFiles.FirstOrDefault(a => a.Id == entity.Id);
					if (file != null)
					{
						if (!file.AnswerSheetBatch_Id.Equals(entity.AnswerSheetBatch_Id))
							file.AnswerSheetBatch_Id = entity.AnswerSheetBatch_Id;

						if (!file.Sent.Equals(entity.Sent))
							file.Sent = entity.Sent;

						if (!file.Student_Id.Equals(entity.Student_Id))
							file.Student_Id = entity.Student_Id;

						if (!file.Section_Id.Equals(entity.Section_Id))
							file.Section_Id = entity.Section_Id;

						if (entity.School_Id != null && !file.School_Id.Equals(entity.School_Id))
							file.School_Id = entity.School_Id;

						if (entity.SupAdmUnit_Id != null && !file.SupAdmUnit_Id.Equals(entity.SupAdmUnit_Id))
							file.SupAdmUnit_Id = entity.SupAdmUnit_Id;

						if (!file.Situation.Equals(entity.Situation))
							file.Situation = entity.Situation;

						file.UpdateDate = DateTime.Now;

						GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Modified;
						GestaoAvaliacaoContext.SaveChanges();
					}
				}
			}
			
		}

		public void UpdateSentList(List<AnswerSheetBatchFiles> list)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				foreach (AnswerSheetBatchFiles entity in list)
				{
					AnswerSheetBatchFiles file = GestaoAvaliacaoContext.AnswerSheetBatchFiles.FirstOrDefault(a => a.Id == entity.Id);

					if (!file.Sent.Equals(entity.Sent))
						file.Sent = entity.Sent;

					file.UpdateDate = DateTime.Now;
					GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Modified;
				}
				GestaoAvaliacaoContext.SaveChanges();
			}
		}

		public void DeleteList(List<AnswerSheetBatchFiles> list)
		{

			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				foreach (AnswerSheetBatchFiles entity in list)
				{
					AnswerSheetBatchFiles file = GestaoAvaliacaoContext.AnswerSheetBatchFiles.FirstOrDefault(a => a.Id == entity.Id);

					file.State = Convert.ToByte(EnumState.excluido);
					file.UpdateDate = DateTime.Now;

					GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Modified;
					GestaoAvaliacaoContext.SaveChanges();
				}
			}
			
		}

		#endregion

		#region Private Methods

		private string GetInner(AnswerSheetBatchFilter filter)
		{
			StringBuilder sql = new StringBuilder();

			sql.AppendLine("LEFT JOIN [AnswerSheetBatch] B WITH (NOLOCK) ON B.[Id] = BF.[AnswerSheetBatch_Id] AND B.[State] = @State ");
			if ((filter.SectionId == null || filter.SectionId < 0) && (filter.SchoolId == null && filter.SchoolId < 0))
			{
				sql.AppendFormat("AND B.[OwnerEntity] = {0} ", (byte)EnumAnswerSheetBatchOwner.Test);
			}

			if (filter.CurriculumTypeId > 0 || filter.ShiftTypeId > 0)
			{
				sql.AppendLine("LEFT JOIN [SGP_TUR_Turma] T WITH (NOLOCK) ON T.[tur_id] = BF.[Section_Id] AND T.[tur_situacao] = @State");
			}

			if (filter.CurriculumTypeId > 0)
			{
				sql.AppendLine("LEFT JOIN [SGP_ACA_TipoTurno] TN WITH (NOLOCK) ON TN.[ttn_id] = T.[ttn_id] AND TN.[ttn_situacao] = @State ");
				sql.AppendLine("LEFT JOIN [SGP_TUR_TurmaTipoCurriculoPeriodo] ttcp WITH (NOLOCK) ON ttcp.[tur_id] = T.[tur_id] AND ttcp.[ttcr_situacao] = @State ");
			}

			return sql.ToString();
		}

		private string GetWhere(AnswerSheetBatchFilter filter)
		{
			StringBuilder sql = new StringBuilder();

			sql.AppendLine("WHERE BF.[State] = @State");

            if (filter.TestId > 0)
            {
                sql.AppendLine("AND B.[Test_Id] = @Test_Id ");
            }

            if (filter.SectionId != null && filter.SectionId > 0)
            {
                sql.AppendFormat("AND BF.[Section_Id] = @Section_Id AND BF.[School_Id] = @School_Id))");
            }
            else if (filter.SchoolId != null && filter.SchoolId > 0)
            {
                sql.AppendFormat("AND BF.[School_Id] = @School_Id ");
            }

			if (filter.BatchId > 0)
			{
				sql.AppendLine("AND B.[Id] = @BatchId ");
			}

			if (filter.SupAdmUnitId != null)
			{
				sql.AppendLine("AND BF.[SupAdmUnit_Id] = @SupAdmUnit_Id ");
			}

            if (filter.BatchQueueId > 0)
            {
                sql.AppendLine("AND BF.[AnswerSheetBatchQueue_Id] = @BatchQueueId ");
            }


            switch (filter.CoreVisionId)
			{
				case (int)EnumSYS_Visao.Gestao:
					sql.AppendLine(string.Format("AND BF.[SupAdmUnit_Id] IN ({0}) ", string.Join(",", filter.UadList)));
					break;
				case (int)EnumSYS_Visao.UnidadeAdministrativa:
					sql.AppendLine(string.Format("AND BF.[School_Id] IN ({0}) ", string.Join(",", filter.UadList)));
					break;
				case (int)EnumSYS_Visao.Individual:
					sql.AppendLine("AND BF.[CreatedBy_Id] = @UserId ");
					break;
				default: break;
			}

            if (!filter.ShowAllStudentsLot)
            {

                if (filter.CurriculumTypeId > 0)
                {
                    sql.AppendLine("AND ttcp.[crp_ordem] = @CurriculumTypeId ");
                }

                if (filter.ShiftTypeId > 0)
                {
                    sql.AppendLine("AND T.[ttn_id] = @ShiftTypeId ");
                }

                if (filter.StartDate != null && filter.StartDate.Equals(DateTime.MinValue))
                    filter.StartDate = null;
                if (filter.EndDate != null && filter.EndDate.Equals(DateTime.MinValue))
                    filter.EndDate = null;

                if (filter.FilterDateUpdate)
                {
                    if (filter.StartDate == null && filter.EndDate != null)
                        sql.AppendLine("AND CAST(BF.[UpdateDate] AS DATE) <= CAST(@EndDate AS DATE) ");
                    else if (filter.StartDate != null && filter.EndDate == null)
                        sql.AppendLine("AND CAST(BF.[UpdateDate] AS DATE) >= CAST(@StartDate AS DATE) ");
                    else if (filter.StartDate != null && filter.EndDate != null)
                        sql.AppendLine("AND CAST(BF.[UpdateDate] AS DATE) >= CAST(@StartDate AS DATE) AND CAST(BF.[UpdateDate] AS DATE) <= CAST(@EndDate AS DATE) ");
                }
                else
                {
                    if (filter.StartDate == null && filter.EndDate != null)
                        sql.AppendLine("AND CAST(BF.[CreateDate] AS DATE) <= CAST(@EndDate AS DATE) ");
                    else if (filter.StartDate != null && filter.EndDate == null)
                        sql.AppendLine("AND CAST(BF.[CreateDate] AS DATE) >= CAST(@StartDate AS DATE) ");
                    else if (filter.StartDate != null && filter.EndDate != null)
                        sql.AppendLine("AND CAST(BF.[CreateDate] AS DATE) >= CAST(@StartDate AS DATE) AND CAST(BF.[CreateDate] AS DATE) <= CAST(@EndDate AS DATE) ");
                }

                if (!string.IsNullOrEmpty(filter.Processing))
                    sql.AppendLine("AND ( BF.[Situation] IN (SELECT Items FROM dbo.SplitString('" + filter.Processing + "',',')) ) ");
            }

            return sql.ToString();
		}

		private string GetSelectByView(AnswerSheetBatchFilter filter, int type)
		{
			StringBuilder sql = new StringBuilder();

			string situationColumn = string.Empty;
			string totalColumn = string.Empty;
			if (filter.View != EnumFollowUpIdentificationView.Files)
			{
				switch (type)
				{
					case (byte)EnumFollowUpIdentificationReportDataType.Sent:
						situationColumn = "'TotalSent'";
						totalColumn = "BF.[Id]"; break;
                    case (byte)EnumFollowUpIdentificationReportDataType.ResolutionNotOk:
                        situationColumn = "'TotalResolutionNotOk'";
                        totalColumn = "BF.[Id]"; break;
                    case (byte)EnumFollowUpIdentificationReportDataType.Identified:
						situationColumn = "'TotalIdentified'";
						totalColumn = "BF.[Id]"; break;
					case (byte)EnumFollowUpIdentificationReportDataType.NotIdentified:
						situationColumn = string.Format(@"CASE WHEN BF.Situation = {0} THEN 'TotalPendingIdentification'
														   WHEN BF.Situation = {1} THEN 'TotalNotIdentified'
													  END",
										(byte)EnumBatchSituation.PendingIdentification, (byte)EnumBatchSituation.NotIdentified);
						totalColumn = "BF.[Situation]"; break;
					default: break;
				}
			}

			string idColumn = string.Empty;
			switch (filter.View)
			{
				case EnumFollowUpIdentificationView.Total:
					if (filter.CoreVisionId != (int)EnumSYS_Visao.Administracao)
						idColumn = "BF.[SupAdmUnit_Id]";
					else
						idColumn = "0";
					break;
				case EnumFollowUpIdentificationView.DRE:
					idColumn = "BF.[SupAdmUnit_Id]";
					break;
				case EnumFollowUpIdentificationView.School:
					idColumn = "BF.[School_Id]";
					break;
				case EnumFollowUpIdentificationView.Files:
					situationColumn = "BF.[Situation]";
					idColumn = "BF.[File_Id]";
					break;
				default: break;
			}

			sql.AppendLine(string.Format("SELECT {0} AS Situation, {1} AS Id", situationColumn, idColumn));

			if (filter.View != EnumFollowUpIdentificationView.Files)
			{
				sql.Append(string.Format(", COUNT({0}) AS Total ", totalColumn));
			}
			else
			{
				sql.Append(", BF.[SupAdmUnit_Id], BF.[School_Id], BF.[AnswerSheetBatch_Id] ");
				sql.AppendLine(string.Format(", {0}", filter.FilterDateUpdate ? "BF.[UpdateDate] AS CreateDate" : "BF.[CreateDate]"));
				sql.AppendLine(string.Format(", ROW_NUMBER() OVER ({0}) AS RowNumber", GetOrderByView(filter)));
			}

			sql.AppendLine("FROM [AnswerSheetBatchFiles] BF WITH (NOLOCK) ");
            if (type == (byte)EnumFollowUpIdentificationReportDataType.ResolutionNotOk)
            {
                sql.AppendLine("INNER JOIN [File] F WITH (NOLOCK) ON BF.File_Id = F.Id ");
            }

            return sql.ToString();
		}

		private string GetSelectNumberedResultReport(AnswerSheetBatchFilter filter)
		{
			StringBuilder sql = new StringBuilder();

			string nameColumn = string.Empty;
			switch (filter.View)
			{
				case EnumFollowUpIdentificationView.Total:
					nameColumn = "'Total de arquivos' AS Name";
					break;
				case EnumFollowUpIdentificationView.DRE:
					nameColumn = "UA.Name";
					break;
				case EnumFollowUpIdentificationView.School:
					nameColumn = "E.Name";
					break;
				case EnumFollowUpIdentificationView.Files:
					nameColumn = "F.[OriginalName] AS Name";
					break;
				default: break;
			}

			sql.AppendLine(string.Format("SELECT NumberedResult.Id, {0}, NumberedResult.TotalResult", nameColumn));

			if (filter.View == EnumFollowUpIdentificationView.Files)
			{
				sql.AppendLine(", NumberedResult.Situation, NumberedResult.CreateDate, NumberedResult.SupAdmUnit_Id, NumberedResult.School_Id, F.[Path] AS FilePath, B.[Test_Id] AS TestId, (CASE WHEN (ISNULL(F.HorizontalResolution, 0) = 0 OR ISNULL(F.VerticalResolution, 0) = 0) THEN NULL ELSE CONVERT(VARCHAR, CAST(F.HorizontalResolution AS INT), 0) + ' x ' + CONVERT(VARCHAR, CAST(F.VerticalResolution AS INT), 0) END) AS Resolution ");
			}
			else
			{
				sql.AppendLine(", NumberedResult.TotalSent, NumberedResult.TotalPendingIdentification, NumberedResult.TotalIdentified, NumberedResult.TotalNotIdentified, NumberedResult.TotalResolutionNotOk ");
			}

			if (filter.View == EnumFollowUpIdentificationView.DRE || filter.View == EnumFollowUpIdentificationView.School)
			{
				sql.AppendLine(string.Format(", ROW_NUMBER() OVER ({0}) AS RowNumber", GetOrderByView(filter)));
			}

			sql.AppendLine("FROM NumberedResult WITH (NOLOCK)");

			return sql.ToString();
		}

		private string GetUAResult(AnswerSheetBatchFilter filter)
		{
			StringBuilder sql = new StringBuilder();

			sql.AppendLine(", UAResult AS (");

			switch (filter.View)
			{
				case EnumFollowUpIdentificationView.DRE:
					sql.AppendLine("SELECT UA.uad_id, CASE WHEN UA.uad_sigla IS NOT NULL THEN UA.uad_sigla + ' - ' + UA.uad_nome ELSE UA.uad_nome END AS Name");
					break;
				case EnumFollowUpIdentificationView.School:
					sql.AppendLine("SELECT E.esc_id, E.uad_idSuperiorGestao, E.esc_nome AS Name");
					break;
				default: break;
			}

			sql.AppendLine("FROM NumberedResult WITH (NOLOCK)");

			switch (filter.View)
			{
				case EnumFollowUpIdentificationView.DRE:
					sql.AppendLine("INNER JOIN [SGP_SYS_UnidadeAdministrativa] UA WITH (NOLOCK) ON UA.uad_id = NumberedResult.[Id] AND UA.uad_situacao = @State");
					break;
				case EnumFollowUpIdentificationView.School:
					sql.AppendLine("INNER JOIN [SGP_ESC_Escola] E WITH (NOLOCK) ON E.esc_id = NumberedResult.[Id] AND E.esc_situacao = @State");
					break;
				default: break;
			}

			sql.AppendLine(")");

			return sql.ToString();
		}

		private string GetInnerByView(AnswerSheetBatchFilter filter)
		{
			StringBuilder sql = new StringBuilder();

			switch (filter.View)
			{
				case EnumFollowUpIdentificationView.DRE:
					sql.AppendLine("INNER JOIN UAResult UA WITH (NOLOCK) ON UA.uad_id = NumberedResult.[Id]");
					break;
				case EnumFollowUpIdentificationView.School:
					sql.AppendLine("INNER JOIN UAResult E WITH (NOLOCK) ON E.esc_id = NumberedResult.[Id]");
					break;
				case EnumFollowUpIdentificationView.Files:
					sql.AppendLine("INNER JOIN [File] F WITH (NOLOCK) ON F.Id = NumberedResult.[Id] AND F.[State] = @State");
					sql.AppendLine("LEFT JOIN [AnswerSheetBatch] B WITH (NOLOCK) ON B.Id = NumberedResult.[AnswerSheetBatch_Id] AND B.[State] = @State");
					break;
				default: break;
			}

			return sql.ToString();
		}

		private string GetWhereByView(AnswerSheetBatchFilter filter, byte type = 0)
		{
			StringBuilder sql = new StringBuilder();

			sql.AppendLine("WHERE BF.[State] = @State");

			if (filter.StartDate != null && filter.StartDate.Equals(DateTime.MinValue))
				filter.StartDate = null;
			if (filter.EndDate != null && filter.EndDate.Equals(DateTime.MinValue))
				filter.EndDate = null;

			if (filter.FilterDateUpdate)
			{
				if (filter.StartDate == null && filter.EndDate != null)
					sql.AppendLine("AND CAST(BF.[UpdateDate] AS DATE) <= CAST(@EndDate AS DATE) ");
				else if (filter.StartDate != null && filter.EndDate == null)
					sql.AppendLine("AND CAST(BF.[UpdateDate] AS DATE) >= CAST(@StartDate AS DATE) ");
				else if (filter.StartDate != null && filter.EndDate != null)
					sql.AppendLine("AND CAST(BF.[UpdateDate] AS DATE) >= CAST(@StartDate AS DATE) AND CAST(BF.[UpdateDate] AS DATE) <= CAST(@EndDate AS DATE) ");
			}
			else
			{
				if (filter.StartDate == null && filter.EndDate != null)
					sql.AppendLine("AND CAST(BF.[CreateDate] AS DATE) <= CAST(@EndDate AS DATE) ");
				else if (filter.StartDate != null && filter.EndDate == null)
					sql.AppendLine("AND CAST(BF.[CreateDate] AS DATE) >= CAST(@StartDate AS DATE) ");
				else if (filter.StartDate != null && filter.EndDate != null)
					sql.AppendLine("AND CAST(BF.[CreateDate] AS DATE) >= CAST(@StartDate AS DATE) AND CAST(BF.[CreateDate] AS DATE) <= CAST(@EndDate AS DATE) ");
			}

			if (!string.IsNullOrEmpty(filter.Processing))
				sql.AppendLine("AND ( BF.[Situation] IN (SELECT Items FROM dbo.SplitString('" + filter.Processing + "',',')) ) ");

			if (filter.SupAdmUnitId != null && (filter.View == EnumFollowUpIdentificationView.DRE || filter.View == EnumFollowUpIdentificationView.School))
				sql.AppendLine("AND BF.[SupAdmUnit_Id] = @SupAdmUnit_Id");

			if (filter.SchoolId != null && (filter.View == EnumFollowUpIdentificationView.School || filter.View == EnumFollowUpIdentificationView.Files))
				sql.AppendLine("AND BF.[School_Id] = @School_Id");

			switch (filter.CoreVisionId)
			{
				case (int)EnumSYS_Visao.Gestao:
					sql.AppendLine(string.Format("AND BF.[SupAdmUnit_Id] IN ({0}) ", string.Join(",", filter.UadList)));
					break;
				case (int)EnumSYS_Visao.UnidadeAdministrativa:
					sql.AppendLine(string.Format("AND BF.[School_Id] IN ({0}) ", string.Join(",", filter.UadList)));
					break;
				case (int)EnumSYS_Visao.Individual:
					sql.AppendLine("AND BF.[CreatedBy_Id] = @UserId ");
					break;
				default: break;
			}

            if (type == (byte)EnumFollowUpIdentificationReportDataType.ResolutionNotOk)
            {
                sql.AppendLine("AND ((ISNULL(HorizontalResolution, 0) > 0 AND (ISNULL(HorizontalResolution, 0) < 200 OR ISNULL(HorizontalResolution, 0) > 300)) ");
                sql.AppendLine("OR(ISNULL(VerticalResolution, 0) > 0 AND(ISNULL(VerticalResolution, 0) < 200 OR ISNULL(VerticalResolution, 0) > 300))) ");
            }

			return sql.ToString();
		}

		private string GetGroupByView(AnswerSheetBatchFilter filter, int type)
		{
			StringBuilder sql = new StringBuilder();

			switch (filter.View)
			{
				case EnumFollowUpIdentificationView.Total:
					switch (type)
					{
						case (byte)EnumFollowUpIdentificationReportDataType.Sent:
						case (byte)EnumFollowUpIdentificationReportDataType.Identified:
                        case (byte)EnumFollowUpIdentificationReportDataType.ResolutionNotOk:
                            if (filter.CoreVisionId != (int)EnumSYS_Visao.Administracao)
								sql.AppendLine("GROUP BY BF.[SupAdmUnit_Id]"); break;
						case (byte)EnumFollowUpIdentificationReportDataType.NotIdentified:
							sql.AppendLine("GROUP BY BF.[Situation], BF.[SupAdmUnit_Id]"); break;
						default: break;
					}
					break;
				case EnumFollowUpIdentificationView.DRE:
					switch (type)
					{
						case (byte)EnumFollowUpIdentificationReportDataType.Sent:
						case (byte)EnumFollowUpIdentificationReportDataType.Identified:
                        case (byte)EnumFollowUpIdentificationReportDataType.ResolutionNotOk:
                            sql.AppendLine("GROUP BY BF.[SupAdmUnit_Id]"); break;
						case (byte)EnumFollowUpIdentificationReportDataType.NotIdentified:
							sql.AppendLine("GROUP BY BF.[Situation], BF.[SupAdmUnit_Id]"); break;
						default: break;
					}
					break;
				case EnumFollowUpIdentificationView.School:
					switch (type)
					{
						case (byte)EnumFollowUpIdentificationReportDataType.Sent:
						case (byte)EnumFollowUpIdentificationReportDataType.Identified:
                        case (byte)EnumFollowUpIdentificationReportDataType.ResolutionNotOk:
                            sql.AppendLine("GROUP BY BF.[School_Id]"); break;
						case (byte)EnumFollowUpIdentificationReportDataType.NotIdentified:
							sql.AppendLine("GROUP BY BF.[Situation], BF.[School_Id]"); break;
						default: break;
					}
					break;
				default: break;
			}

			return sql.ToString();
		}

		private string GetOrderByView(AnswerSheetBatchFilter filter)
		{
			StringBuilder sql = new StringBuilder();

			switch (filter.View)
			{
				case EnumFollowUpIdentificationView.DRE:
					sql.Append("ORDER BY UA.Name ASC"); break;
				case EnumFollowUpIdentificationView.School:
					sql.Append("ORDER BY E.Name ASC"); break;
				case EnumFollowUpIdentificationView.Files:
					sql.Append(string.Format("ORDER BY {0} ASC", filter.FilterDateUpdate ? "BF.[UpdateDate]" : "BF.[CreateDate]")); break;
				default: break;
			}

			return sql.ToString();
		}

		private string GetTotalResult(AnswerSheetBatchFilter filter)
		{
			StringBuilder sql = new StringBuilder();

			if (filter.CoreVisionId != (int)EnumSYS_Visao.Administracao)
			{
				switch (filter.View)
				{
					case EnumFollowUpIdentificationView.Total:
					case EnumFollowUpIdentificationView.DRE:
						sql.AppendLine(", COUNT(1) OVER (PARTITION BY R.[Id]) AS TotalResult");
						break;
					case EnumFollowUpIdentificationView.School:
					case EnumFollowUpIdentificationView.Files:
						sql.AppendLine(", COUNT(1) OVER () AS TotalResult");
						break;
					default: break;
				}
			}
			else
			{
				sql.AppendLine(", COUNT(1) OVER () AS TotalResult");
			}

			return sql.ToString();
		}

		private IEnumerable<string> GetUserPermission(AnswerSheetBatchFilter filter)
		{
			StringBuilder sql = new StringBuilder();

			switch (filter.CoreVisionId)
			{
				case (int)EnumSYS_Visao.Gestao:
					sql.Append("SELECT DISTINCT UA.uad_id FROM [SGP_Sys_UnidadeAdministrativa] UA WITH (NOLOCK)");
					break;
				case (int)EnumSYS_Visao.UnidadeAdministrativa:
					sql.Append("SELECT DISTINCT E.esc_id FROM [SGP_ESC_Escola] E WITH (NOLOCK)");
					break;
				default: break;
			}

			switch (filter.CoreVisionId)
			{
				case (int)EnumSYS_Visao.Gestao:
				case (int)EnumSYS_Visao.UnidadeAdministrativa:
					sql.AppendLine("INNER JOIN [Synonym_Core_SYS_Grupo] G WITH (NOLOCK) ON G.vis_id = @VisionId AND G.sis_id = @SystemId AND G.gru_situacao = @State");
					sql.AppendLine("INNER JOIN [Synonym_Core_SYS_UsuarioGrupoUA] UG WITH (NOLOCK) ON UG.usu_id = @UserId AND UG.gru_id = G.gru_id");
					break;
				default: break;
			}

			switch (filter.CoreVisionId)
			{
				case (int)EnumSYS_Visao.Gestao:
					sql.AppendLine("WHERE UA.uad_id = UG.uad_id");
					break;
				case (int)EnumSYS_Visao.UnidadeAdministrativa:
					sql.AppendLine("WHERE E.uad_id = UG.uad_id");
					break;
				default: break;
			}

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var query = cn.QueryMultiple(
					sql.ToString(),
					 new
					 {
						 State = EnumState.ativo,
						 UserId = filter.UserId,
						 PesId = filter.PesId,
						 VisionId = filter.CoreVisionId,
						 SystemId = filter.CoreSystemId
					 });

				switch (filter.CoreVisionId)
				{
					case (int)EnumSYS_Visao.Gestao:
						var result1 = query.Read<Guid>();
						return result1.AsEnumerable().Select(x => string.Concat("'", x, "'"));
					case (int)EnumSYS_Visao.UnidadeAdministrativa:
						var result2 = query.Read<int>();
						return result2.AsEnumerable().Select(x => string.Concat("'", x, "'"));
					default: return null;
				}
			}
		}

		#endregion
	}
}
