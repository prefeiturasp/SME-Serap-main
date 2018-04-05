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
    public class ExportAnalysisRepository : ConnectionReadOnly, IExportAnalysisRepository
	{
		#region ReadOnly
		public IEnumerable<ExportAnalysisDTO> Search(ref Pager pager, ExportAnalysisFilter filter)
		{
			var from = "FROM TestSectionStatusCorrection tssc WITH(NOLOCK) ";
			var inner = @"INNER JOIN Test t WITH(NOLOCK) ON t.Id = tssc.Test_Id
						  INNER JOIN TestType tt WITH(NOLOCK) ON tt.Id = t.TestType_Id AND tt.State = @state
						  LEFT JOIN ExportAnalysis ea WITH(NOLOCK) ON ea.Test_Id = t.Id AND ea.State = @state
						  LEFT JOIN[File] f WITH(NOLOCK) ON f.OwnerId = ea.Id AND f.OwnerType = @OwnerType AND f.State = @state ";
			StringBuilder where = new StringBuilder(@"WHERE t.State = @state ");
			if (filter.Code > 0)
				where.AppendLine("AND t.Id = @TestId");
			if (filter.StartDate != null)
			{
				where.AppendLine("AND CAST(t.ApplicationStartDate AS Date) >= CAST(@StartDate AS Date) ");
			}
			if (filter.EndDate != null)
			{
				where.AppendLine("AND CAST(t.CorrectionEndDate AS Date) <= CAST(@EndDate AS Date) ");
			}
			//var sql = ExportAnalysisQueryFactory.GenerateQuerySearchByFilter(filter);
			var sql = @"WITH DistinctExportAnalysis AS (
							SELECT DISTINCT t.Id AS Test_Id, 
								t.Description AS TestDescription, 
								ea.CreateDate, 
								ea.UpdateDate, 
								tt.Description AS TestTypeDescription, 
								ISNULL(ea.StateExecution, 1) AS StateExecution,
								f.Id AS FileId " +
							from +
							inner +
							where +
						@"),
						NumberedExportAnalysis AS ( 
							SELECT Test_Id, 
								TestDescription, 
								CreateDate, 
								UpdateDate, 
								TestTypeDescription, 
								StateExecution,
								FileId,
								ROW_NUMBER() OVER(ORDER BY Test_Id) AS RowNumber 
							FROM DistinctExportAnalysis
						) 
						SELECT Test_Id, TestDescription, CreateDate, UpdateDate, TestTypeDescription, 
							StateExecution, FileId 
						FROM NumberedExportAnalysis 
						WHERE RowNumber > ( @pageSize * @page ) 
						AND RowNumber <= ( ( @page + 1 ) * @pageSize ) 
						ORDER BY RowNumber";

			var countSql = @"SELECT COUNT(DISTINCT t.id) " +
							from +
							inner +
							where;

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var retorno = cn.Query< ExportAnalysisDTO>(sql,
					new
					{
						state = EnumState.ativo,
						TestId = filter.Code,
						OwnerType = EnumFileType.AnalysisItem,
						StartDate = filter.StartDate,
						EndDate = filter.EndDate,
						page = pager.CurrentPage,
						pageSize = pager.PageSize
					});

				var count = (int)cn.ExecuteScalar(countSql, new
				{
					state = EnumState.ativo,
					TestId = filter.Code,
					OwnerType = EnumFileType.AnalysisItem,
					StartDate = filter.StartDate,
					EndDate = filter.EndDate,
					page = pager.CurrentPage,
					pageSize = pager.PageSize
				});

				pager.SetTotalPages((int)Math.Ceiling(Convert.ToInt32(count) / (double)pager.PageSize));
				pager.SetTotalItens(Convert.ToInt32(count));

				return retorno;
			}
		}

		public ExportAnalysis GetByTestId(long TestId)
		{
			var sql = new StringBuilder();
			sql.AppendLine("SELECT Id, Test_Id, StateExecution, CreateDate, UpdateDate, State");
			sql.AppendLine("FROM dbo.ExportAnalysis");
			sql.AppendLine("WHERE Test_Id = @TestId And State = @state");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var retorno = cn.Query<ExportAnalysis>(sql.ToString(), new { TestId = TestId, state = EnumState.ativo }).FirstOrDefault();

				return retorno;
			}
		}

		public IEnumerable<ExportAnalysis> GetByExecutionState(EnumServiceState state)
		{
			var sql = new StringBuilder();
			sql.AppendLine("SELECT Id, Test_Id, StateExecution, CreateDate, UpdateDate, State");
			sql.AppendLine("FROM dbo.ExportAnalysis");
			sql.AppendLine("WHERE StateExecution = @StateExecution");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var retorno = cn.Query<ExportAnalysis>(sql.ToString(), new { StateExecution = state });

				return retorno;
			}
		}
		#endregion

		#region Persist
		public ExportAnalysis Save(ExportAnalysis entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				DateTime dateNow = DateTime.Now;

				entity.CreateDate = dateNow;
				entity.UpdateDate = dateNow;
				entity.State = Convert.ToByte(EnumState.ativo);

				GestaoAvaliacaoContext.ExportAnalysis.Add(entity);
				GestaoAvaliacaoContext.SaveChanges();

				return entity;
			}
		}

		public ExportAnalysis Update(ExportAnalysis entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				ExportAnalysis _entity = GestaoAvaliacaoContext.ExportAnalysis.FirstOrDefault(a => a.Id == entity.Id);
				_entity.UpdateDate = DateTime.Now;
				_entity.StateExecution = entity.StateExecution;

				GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();

				return _entity;
			}
		}

		#endregion
	}
}
