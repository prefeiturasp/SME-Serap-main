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
    public class AnswerSheetLotRepository : ConnectionReadOnly, IAnswerSheetLotRepository
	{
		#region Read

		public IEnumerable<AnswerSheetLotDTO> GetTestLot(ref Pager pager, AnswerSheetLotFilter filter)
		{
			#region Consulta
			var sql = new StringBuilder("WITH PaginatedResult AS (");
            sql.AppendLine("SELECT R.TestCode, R.Description, R.TestTypeDescription, R.StateExecution, R.Id, R.FileId, R.FilePath");
            sql.AppendLine(", ROW_NUMBER() OVER (ORDER BY R.TestCode DESC) AS RowNumber");
            sql.AppendLine("FROM (SELECT DISTINCT t.Id AS TestCode, t.Description, tt.Description AS TestTypeDescription");
            if ((byte)filter.StateExecution == 1)
            {
                sql.AppendLine(", 0 AS Id, 0 AS StateExecution, 0 AS FileId, NULL AS FilePath");
            }
            else
            {
                sql.AppendLine(", l.Id, l.StateExecution, f.Id AS FileId, f.Path AS FilePath");
            }
            sql.AppendLine("FROM Test t WITH (NOLOCK)");
            sql.AppendLine("INNER JOIN TestType tt WITH (NOLOCK) ON tt.Id = t.TestType_Id AND tt.State = @state");
            if ((byte)filter.StateExecution > 1)
            {
                sql.AppendLine("INNER JOIN AnswerSheetLot l WITH (NOLOCK) ON l.Test_Id = t.Id AND l.State = @state AND l.[Type] = @lotType");
                sql.AppendLine("AND l.StateExecution = @stateExecution");
            }
            else if ((byte)filter.StateExecution <= 0) { 
                sql.AppendLine("LEFT JOIN AnswerSheetLot l WITH (NOLOCK) ON l.Test_Id = t.Id AND l.State = @state AND l.[Type] = @lotType");
            }

            if ((byte)filter.StateExecution != 1)
            {
                sql.AppendLine("LEFT JOIN [File] f WITH (NOLOCK) ON f.OwnerId = l.Id AND f.State = @state AND f.OwnerType = @fileType");
            }

			sql.AppendLine("WHERE tt.Global = 1 AND T.State = @state AND (T.[AllAdhered] = 1 OR EXISTS (SELECT TOP 1 A.Id FROM [Adherence] A WITH (NOLOCK) WHERE A.Test_Id = T.Id AND A.State = @state AND (A.TypeSelection = @TypeSelectionSelected OR A.TypeSelection = @TypeSelectionPartial))) ");
            sql.AppendLine(") AS R ");
            if (filter.Test_Id > 0)
            {
                sql.AppendLine("WHERE R.TestCode = ISNULL(@testId, R.TestCode)");
            }
            sql.AppendLine(")");
            sql.AppendLine("SELECT TestCode, Description, TestTypeDescription, StateExecution, Id, FileId, FilePath");
			sql.AppendLine("FROM PaginatedResult");
			sql.AppendLine("WHERE RowNumber > (@pageSize * @page)");
			sql.AppendLine("AND RowNumber <= ((@page + 1) * @pageSize)");
			sql.AppendLine("ORDER BY TestCode DESC");

            sql.AppendLine("SELECT COUNT(R.[TestCode]) FROM ( SELECT t.Id AS TestCode FROM [Test] t WITH (NOLOCK)");
            sql.AppendLine("INNER JOIN TestType tt WITH (NOLOCK) ON tt.Id = t.TestType_Id AND tt.State = @state");
            if ((byte)filter.StateExecution > 1)
            {
                sql.AppendLine("INNER JOIN AnswerSheetLot l WITH (NOLOCK) ON l.Test_Id = t.Id AND l.State = @state AND l.[Type] = @lotType");
                sql.AppendLine("AND l.StateExecution = @stateExecution");
            }
            else if ((byte)filter.StateExecution <= 0)
            {
                sql.AppendLine("LEFT JOIN AnswerSheetLot l WITH (NOLOCK) ON l.Test_Id = t.Id AND l.State = @state AND l.[Type] = @lotType");
            }

            if ((byte)filter.StateExecution != 1)
            {
                sql.AppendLine("LEFT JOIN [File] f WITH (NOLOCK) ON f.OwnerId = l.Id AND f.State = @state AND f.OwnerType = @fileType");
            }
            sql.AppendLine("WHERE tt.Global = 1 AND T.State = @state AND (T.[AllAdhered] = 1 OR EXISTS (SELECT TOP 1 A.Id FROM [Adherence] A WITH (NOLOCK) WHERE A.Test_Id = T.Id AND A.State = @state AND (A.TypeSelection = @TypeSelectionSelected OR A.TypeSelection = @TypeSelectionPartial))) ");
            sql.Append("GROUP BY t.Id ) AS R ");
            if (filter.Test_Id > 0)
            {
                sql.AppendLine("WHERE R.TestCode = ISNULL(@testId, R.TestCode)");
            }
            #endregion

            using (IDbConnection cn = Connection)
			{
				cn.Open();
				
				var query = cn.QueryMultiple(sql.ToString(),
					new
					{
						state = (byte)EnumState.ativo,
						testId = filter.Test_Id,
						pageSize = pager.PageSize,
						page = pager.CurrentPage,
						fileType = (byte)EnumFileType.AnswerSheetLot,
                        lotType = (byte)EnumAnswerSheetBatchOwner.Test,
                        stateExecution = (byte)filter.StateExecution,
                        TypeSelectionSelected = EnumAdherenceSelection.Selected,
                        TypeSelectionPartial = EnumAdherenceSelection.Partial
                    });

				var retorno = query.Read<AnswerSheetLotDTO>();
				var count = query.Read<int>().FirstOrDefault();

				pager.SetTotalItens(count);
				pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));

				return retorno;
			}
		}

        public IEnumerable<AnswerSheetLot> GetLotList(AnswerSheetLotFilter filter, ref Pager pager)
        {
            #region Consulta

            StringBuilder sql = new StringBuilder();

            sql.AppendLine("WITH PaginatedResult AS (");
            sql.AppendLine(@"SELECT [Id]
                                    ,[Test_Id]
                                    ,[StateExecution]
                                    ,[uad_id]
                                    ,[esc_id]
                                    ,[CreateDate]
                                    ,[UpdateDate]
                                    ,[State]
                                    ,[Type]
                                    ,[RequestDate]
                                    ,[Parent_Id]");
            sql.AppendLine(", ROW_NUMBER() OVER (ORDER BY [CreateDate] DESC) AS RowNumber");
            sql.AppendLine("FROM [AnswerSheetLot] WITH (NOLOCK)");

            #region WHERE

            StringBuilder sqlWhere = new StringBuilder();

            sqlWhere.AppendLine("WHERE [State] <> @state AND [Type] = @lotType AND [Parent_Id] IS NULL");

            if (filter.Lot_Id > 0)
                sqlWhere.AppendLine("AND [Id] = @lot_Id");

            if (filter.Test_Id > 0)
                sqlWhere.AppendLine("AND [Test_Id] = @test_Id");

            if ((byte)filter.StateExecution > 0)
                sqlWhere.AppendLine("AND [StateExecution] = @stateExecution");

            if (filter.StartDate != null && filter.StartDate.Equals(DateTime.MinValue))
                filter.StartDate = null;

            if (filter.EndDate != null && filter.EndDate.Equals(DateTime.MinValue))
                filter.EndDate = null;

            if (filter.StartDate == null && filter.EndDate != null)
                sqlWhere.AppendLine("AND CAST([CreateDate] AS DATE) <= CAST(@endDate AS DATE) ");
            else if (filter.StartDate != null && filter.EndDate == null)
                sqlWhere.AppendLine("AND CAST([CreateDate] AS DATE) >= CAST(@startDate AS DATE) ");
            else if (filter.StartDate != null && filter.EndDate != null)
                sqlWhere.AppendLine("AND CAST([CreateDate] AS DATE) >= CAST(@startDate AS DATE) AND CAST([CreateDate] AS DATE) <= CAST(@endDate AS DATE) ");
            
            #endregion

            sql.Append(sqlWhere);            
            sql.AppendLine(")");
            sql.AppendLine(@"SELECT [Id]
                                    ,[Test_Id]
                                    ,[StateExecution]
                                    ,[uad_id]
                                    ,[esc_id]
                                    ,[CreateDate]
                                    ,[UpdateDate]
                                    ,[State]
                                    ,[Type]
                                    ,[RequestDate]
                                    ,[Parent_Id]
                                FROM PaginatedResult WITH (NOLOCK)");

            if (pager != null)
            {
                sql.AppendLine("WHERE RowNumber > ( @pageSize * @page ) ");
                sql.AppendLine("AND RowNumber <= ( ( @page + 1 ) * @pageSize ) ");
            }
            sql.AppendLine("ORDER BY [CreateDate] DESC");
            sql.AppendLine("SELECT COUNT([Id]) FROM [AnswerSheetLot] WITH (NOLOCK)");
            sql.Append(sqlWhere);

            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(),
                    new
                    {
                        lot_Id = filter.Lot_Id,
                        test_Id = filter.Test_Id,
                        stateExecution = (byte)filter.StateExecution,
                        startDate = filter.StartDate,
                        endDate = filter.EndDate,
                        state = (byte)EnumState.excluido,
                        lotType = (byte)filter.Type,
                        pageSize = pager.PageSize,
                        page = pager.CurrentPage
                    });

                var retorno = query.Read<AnswerSheetLot>();
                var count = query.Read<int>().FirstOrDefault();

                pager.SetTotalItens(count);
                pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));

                return retorno;
            }
        }

        public IEnumerable<AnswerSheetLotDTO> GetLotFiles(AnswerSheetLotFilter filter, ref Pager pager)
        {
            #region Consulta

            StringBuilder sql = new StringBuilder();
            StringBuilder sqlInner = new StringBuilder();

            sql.AppendLine("WITH PaginatedResult AS (");
            sql.AppendLine(@"SELECT E.esc_nome AS SchoolName, UA.uad_nome AS SupAdmUnitName, F.Id AS FileId, F.Path AS FilePath");
            sql.AppendLine(", ROW_NUMBER() OVER (ORDER BY E.esc_nome ASC) AS RowNumber");
            sql.AppendLine("FROM [AnswerSheetLot] L WITH (NOLOCK)");

            sqlInner.AppendLine("INNER JOIN [File] F WITH (NOLOCK) ON F.OwnerId = L.Id AND F.OwnerType = @ownerType");
            sqlInner.AppendLine("INNER JOIN [SGP_ESC_Escola] E WITH (NOLOCK) ON E.esc_id = F.ParentOwnerId AND E.esc_situacao = @state");
            sqlInner.AppendLine("INNER JOIN [SGP_SYS_UnidadeAdministrativa] UA WITH (NOLOCK) ON UA.uad_id = E.uad_idSuperiorGestao AND UA.uad_situacao = @state");

            #region WHERE

            StringBuilder sqlWhere = new StringBuilder();

            sqlWhere.AppendLine("WHERE L.[State] = @state AND F.[State] = @state AND L.Parent_Id = @lot_Id");

            if (filter.SchoolId != null && filter.SchoolId > 0)
                sqlWhere.AppendLine("AND E.esc_id = @esc_id");

            if (filter.SupAdmUnitId != null && filter.SupAdmUnitId != Guid.Empty)
                sqlWhere.AppendLine("AND UA.uad_id = @uad_id");

            #endregion

            sql.Append(sqlInner);
            sql.Append(sqlWhere);
            sql.AppendLine(")");
            sql.AppendLine(@"SELECT SchoolName, SupAdmUnitName, FileId, FilePath
                                FROM PaginatedResult WITH (NOLOCK)");
            if (pager != null)
            {
                sql.AppendLine("WHERE RowNumber > ( @pageSize * @page ) ");
                sql.AppendLine("AND RowNumber <= ( ( @page + 1 ) * @pageSize ) ");
            }
            sql.AppendLine("ORDER BY SchoolName ASC");
            sql.AppendLine("SELECT COUNT(L.[Id]) FROM [AnswerSheetLot] L WITH (NOLOCK)");
            sql.Append(sqlInner);
            sql.Append(sqlWhere);

            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(),
                    new
                    {
                        lot_Id = filter.Lot_Id,
                        state = (byte)EnumState.ativo,
                        esc_id = filter.SchoolId,
                        uad_id = filter.SupAdmUnitId,
                        ownerType = (byte)EnumFileType.AnswerSheetLot,
                        pageSize = pager.PageSize,
                        page = pager.CurrentPage
                    });

                var retorno = query.Read<AnswerSheetLotDTO>();
                var count = query.Read<int>().FirstOrDefault();

                pager.SetTotalItens(count);
                pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));

                return retorno;
            }
        }

        public IEnumerable<AnswerSheetLotDTO> GetAdheredTests(AnswerSheetLotFilter filter, ref Pager pager)
        {
            #region Consulta

            StringBuilder sql = new StringBuilder();
            StringBuilder sqlInner = new StringBuilder();

            sql.AppendLine("WITH PaginatedResult AS (");
            sql.AppendLine("SELECT R.TestCode, R.Description, R.TestTypeDescription, R.CreateDate, R.AllAdhered, R.TotalAdherence");
            sql.AppendLine(", ROW_NUMBER() OVER (ORDER BY R.CreateDate DESC) AS RowNumber");
            sql.AppendLine("FROM ( SELECT DISTINCT T.Id AS TestCode, T.Description AS Description, TT.Description AS TestTypeDescription, T.CreateDate, T.State, T.TestType_Id, T.AllAdhered");
            sql.AppendLine(", CASE WHEN T.AllAdhered = 0 THEN (SELECT COUNT(Adherence.EntityId) FROM Adherence WITH(NOLOCK) WHERE Adherence.TypeEntity = @typeAdherence AND Adherence.Test_Id = T.Id GROUP BY Adherence.Test_Id) END AS TotalAdherence");
            sql.AppendLine("FROM [Test] T WITH (NOLOCK)");
            sqlInner.AppendLine("INNER JOIN [TestType] TT WITH (NOLOCK) ON TT.Id = T.TestType_Id AND TT.State <> @state");

            #region WHERE

            StringBuilder sqlWhere = new StringBuilder();

            sqlWhere.AppendLine("WHERE R.State <> @state");

            if (filter.Test_Id > 0)
                sqlWhere.AppendLine("AND R.TestCode = @test_Id");

            if (filter.TestType_Id != null && filter.TestType_Id > 0)
                sqlWhere.AppendLine("AND R.TestType_Id = @testType_Id");

            if (filter.StartDate != null && filter.StartDate.Equals(DateTime.MinValue))
                filter.StartDate = null;

            if (filter.EndDate != null && filter.EndDate.Equals(DateTime.MinValue))
                filter.EndDate = null;

            if (filter.StartDate == null && filter.EndDate != null)
                sqlWhere.AppendLine("AND CAST(R.CreateDate AS DATE) <= CAST(@endDate AS DATE) ");
            else if (filter.StartDate != null && filter.EndDate == null)
                sqlWhere.AppendLine("AND CAST(R.CreateDate AS DATE) >= CAST(@startDate AS DATE) ");
            else if (filter.StartDate != null && filter.EndDate != null)
                sqlWhere.AppendLine("AND CAST(R.CreateDate AS DATE) >= CAST(@startDate AS DATE) AND CAST(R.CreateDate AS DATE) <= CAST(@endDate AS DATE) ");

            #endregion

            sql.Append(sqlInner);
            sql.AppendLine("WHERE T.State <> @state AND (T.[AllAdhered] = 1 OR EXISTS (SELECT TOP 1 A.Id FROM [Adherence] A WITH (NOLOCK) WHERE A.Test_Id = T.Id AND A.State <> @state AND (A.TypeSelection = @TypeSelectionSelected OR A.TypeSelection = @TypeSelectionPartial))) ");
            sql.AppendLine(") AS R ");
            sql.Append(sqlWhere);
            sql.AppendLine(")");
            sql.AppendLine(@"SELECT TestCode, Description, TestTypeDescription, CreateDate, AllAdhered, TotalAdherence
                                FROM PaginatedResult WITH (NOLOCK)");
            if (pager != null)
            {
                sql.AppendLine("WHERE RowNumber > ( @pageSize * @page ) ");
                sql.AppendLine("AND RowNumber <= ( ( @page + 1 ) * @pageSize ) ");
            }
            sql.AppendLine("ORDER BY CreateDate DESC");

            sql.AppendLine("SELECT COUNT(R.TestCode) FROM ( SELECT DISTINCT T.Id AS TestCode, T.Description AS Description, TT.Description AS TestTypeDescription, T.CreateDate, T.State, T.TestType_Id");
            sql.AppendLine("FROM [Test] T WITH (NOLOCK)");
            sql.Append(sqlInner);
            sql.AppendLine("WHERE T.State <> @state AND (T.[AllAdhered] = 1 OR EXISTS (SELECT TOP 1 A.Id FROM [Adherence] A WITH (NOLOCK) WHERE A.Test_Id = T.Id AND A.State <> @state AND (A.TypeSelection = @TypeSelectionSelected OR A.TypeSelection = @TypeSelectionPartial))) ");
            sql.Append(") AS R ");
            sql.Append(sqlWhere);

            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(),
                    new
                    {
                        test_Id = filter.Test_Id,
                        testType_Id = filter.TestType_Id,
                        startDate = filter.StartDate,
                        endDate = filter.EndDate,
                        state = (byte)EnumState.excluido,
                        typeAdherence = (byte)EnumAdherenceEntity.School,
                        pageSize = pager.PageSize,
                        page = pager.CurrentPage,
                        TypeSelectionSelected = EnumAdherenceSelection.Selected,
                        TypeSelectionPartial = EnumAdherenceSelection.Partial
                    });

                var retorno = query.Read<AnswerSheetLotDTO>();
                var count = query.Read<int>().FirstOrDefault();

                pager.SetTotalItens(count);
                pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));

                return retorno;
            }
        }

        public AnswerSheetLot GetByTest(long TestId)
		{
			#region Consulta
			var sql = new StringBuilder(@"SELECT [Id]
                                                ,[Test_Id]
                                                ,[StateExecution]
                                                ,[uad_id]
                                                ,[esc_id]
                                                ,[CreateDate]
                                                ,[UpdateDate]
                                                ,[State]
                                                ,[Type]
                                                ,[RequestDate]
                                                ,[Parent_Id]
                                                ,[ExecutionOwner]");
			sql.AppendLine("FROM AnswerSheetLot WITH (NOLOCK)");
			sql.AppendLine("WHERE Test_Id = @testId AND [State] = @state AND [Type] = @lotType");
			#endregion

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<AnswerSheetLot>(sql.ToString(),
					new
					{
						testId = TestId,
						state = (byte)EnumState.ativo,
                        lotType = (byte)EnumAnswerSheetBatchOwner.Test
                    }).FirstOrDefault();
			}
		}

		public AnswerSheetLot GetById(long Id)
		{
			#region Consulta
			var sql = new StringBuilder(@"SELECT [Id]
                                                ,[Test_Id]
                                                ,[StateExecution]
                                                ,[uad_id]
                                                ,[esc_id]
                                                ,[CreateDate]
                                                ,[UpdateDate]
                                                ,[State]
                                                ,[Type]
                                                ,[RequestDate]
                                                ,[Parent_Id]
                                                ,[ExecutionOwner]");
			sql.AppendLine("FROM AnswerSheetLot WITH (NOLOCK)");
			sql.AppendLine("WHERE Id = @Id AND [State] <> @State");
			#endregion

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<AnswerSheetLot>(sql.ToString(),
					new
					{
						Id = Id,
                        State = (byte)EnumState.excluido
                    }).FirstOrDefault();
			}
		}

        public IEnumerable<AnswerSheetLot> GetByParentId(long ParentId)
        {
            #region Consulta
            var sql = new StringBuilder(@"SELECT [Id]
                                                ,[Test_Id]
                                                ,[StateExecution]
                                                ,[uad_id]
                                                ,[esc_id]
                                                ,[CreateDate]
                                                ,[UpdateDate]
                                                ,[State]
                                                ,[Type]
                                                ,[RequestDate]
                                                ,[Parent_Id]
                                                ,[ExecutionOwner]");
            sql.AppendLine("FROM AnswerSheetLot WITH (NOLOCK)");
            sql.AppendLine("WHERE Parent_Id = @Id AND [State] <> @State");
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<AnswerSheetLot>(sql.ToString(),
                    new
                    {
                        Id = ParentId,
                        State = (byte)EnumState.excluido
                    });
            }
        }

        public IEnumerable<AnswerSheetLot> GetByExecutionState(EnumServiceState state)
        {
            #region Consulta
            var sql = new StringBuilder(@"SELECT [Id]
                                                ,[Test_Id]
                                                ,[StateExecution]
                                                ,[uad_id]
                                                ,[esc_id]
                                                ,[CreateDate]
                                                ,[UpdateDate]
                                                ,[State]
                                                ,[Type]
                                                ,[RequestDate]
                                                ,[Parent_Id]
                                                ,[ExecutionOwner]");
            sql.AppendLine("FROM AnswerSheetLot WITH (NOLOCK) ");
            sql.AppendLine("WHERE [StateExecution] = @StateExecution AND [State] <> @State");
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<AnswerSheetLot>(sql.ToString(),
                    new
                    {
                        StateExecution = (byte)state,
                        State = (byte)EnumState.excluido
                    });
            }
        }

        public IEnumerable<AnswerSheetLot> GetParentByExecutionState(EnumServiceState state)
        {
            #region Consulta
            var sql = new StringBuilder(@"SELECT [Id]
                                                ,[Test_Id]
                                                ,[StateExecution]
                                                ,[uad_id]
                                                ,[esc_id]
                                                ,[CreateDate]
                                                ,[UpdateDate]
                                                ,[State]
                                                ,[Type]
                                                ,[RequestDate]
                                                ,[Parent_Id]
                                                ,[ExecutionOwner]");
            sql.AppendLine("FROM AnswerSheetLot WITH (NOLOCK) ");
            sql.AppendLine("WHERE [StateExecution] = @StateExecution  AND Parent_Id IS NULL AND [State] <> @State");
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<AnswerSheetLot>(sql.ToString(),
                    new
                    {
                        StateExecution = (byte)state,
                        State = (byte)EnumState.excluido
                    });
            }
        }

        public IEnumerable<Test> GetTestList(long Id)
        {
            #region Consulta
            var sql = new StringBuilder("SELECT T.Id, T.Description ");
            sql.AppendLine("FROM AnswerSheetLot L WITH (NOLOCK) ");
            sql.AppendLine("INNER JOIN Test T WITH (NOLOCK) ON T.Id = L.Test_Id ");
            sql.AppendLine("WHERE L.[Parent_Id] = @id AND L.[State] <> @State AND T.[State] <> @State");
            sql.AppendLine("ORDER BY T.Description ASC");
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<Test>(sql.ToString(),
                    new
                    {
                        id = Id,
                        State = (byte)EnumState.excluido
                    });
            }
        }

        public int GetTestCount(long Id)
        {
            #region Consulta
            var sql = new StringBuilder(@"SELECT COUNT([Test_Id])");
            sql.AppendLine("FROM AnswerSheetLot WITH (NOLOCK) ");
            sql.AppendLine("WHERE [Parent_Id] = @id AND [State] <> @State");
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<int>(sql.ToString(),
                    new
                    {
                        id = Id,
                        State = (byte)EnumState.excluido
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<long> GetFiles(long parent_Id)
        {
            var sql = new StringBuilder(@"SELECT F.[Id]
                                                FROM [File] F WITH (NOLOCK)");
            sql.AppendLine("INNER JOIN AnswerSheetLot L WITH (NOLOCK) ON F.OwnerId = L.Id");
            sql.AppendLine("WHERE L.Parent_Id = @id AND L.[State] <> @state AND F.[State] <> @state AND F.OwnerType = @ownerType");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<Entities.File>(sql.ToString(),
                    new
                    {
                        id = parent_Id,
                        updateDate = DateTime.Now,
                        state = (byte)EnumState.excluido,
                        ownerType = (byte)EnumFileType.AnswerSheetLot
                    }).Select(i => i.Id);
            }
        }

        #endregion

        #region Persist

        public AnswerSheetLot Save(AnswerSheetLot entity)
		{
			using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				var answerSheetLot = GetByTest(entity.TestId);
				if (answerSheetLot != null)
				{
                    entity.Id = answerSheetLot.Id;
                    Update(entity);
				}
				else
				{
					entity.CreateDate = DateTime.Now;
                    entity.UpdateDate = DateTime.Now;
                    entity.State = Convert.ToByte(EnumState.ativo);

					gestaoAvaliacaoContext.AnswerSheetLot.Add(entity);
				}

				gestaoAvaliacaoContext.SaveChanges();

				return entity;
			}
		}

        public AnswerSheetLot SaveLot(AnswerSheetLot entity, List<AnswerSheetLot> list)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var answerSheetLot = GetById(entity.Id);
                if (answerSheetLot != null)
                {
                    entity.Id = answerSheetLot.Id;
                    UpdateLot(entity);
                    Update(entity);
                }
                else
                {
                    entity.CreateDate = DateTime.Now;
                    entity.UpdateDate = DateTime.Now;
                    entity.State = Convert.ToByte(EnumState.ativo);

                    gestaoAvaliacaoContext.AnswerSheetLot.Add(entity);
                    gestaoAvaliacaoContext.SaveChanges();

                    foreach (AnswerSheetLot ent in list)
                    {
                        ent.CreateDate = DateTime.Now;
                        ent.UpdateDate = DateTime.Now;
                        ent.State = Convert.ToByte(EnumState.ativo);
                        ent.Parent_Id = entity.Id;

                        gestaoAvaliacaoContext.AnswerSheetLot.Add(ent);
                    }
                }

                gestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public AnswerSheetLot Update(AnswerSheetLot entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				AnswerSheetLot _entity = GestaoAvaliacaoContext.AnswerSheetLot.FirstOrDefault(a => a.Id == entity.Id);
				_entity.UpdateDate = DateTime.Now;
                _entity.RequestDate = DateTime.Now;
                _entity.State = Convert.ToByte(EnumState.ativo);
				_entity.StateExecution = entity.StateExecution;
                if (!string.IsNullOrEmpty(entity.ExecutionOwner))
                {
                    _entity.ExecutionOwner = entity.ExecutionOwner;
                }

				GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();

				return _entity;
			}
		}

        public void UpdateLot(AnswerSheetLot entity)
        {
            var sql = new StringBuilder("UPDATE AnswerSheetLot SET UpdateDate = @updateDate, RequestDate = @updateDate, State = @state, StateExecution = @stateExecution, ExecutionOwner = @executionOwner ");
            sql.AppendLine("WHERE Parent_Id = @id");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                cn.Execute(sql.ToString(),
                    new
                    {
                        id = entity.Id,
                        updateDate = DateTime.Now,
                        state = Convert.ToByte(EnumState.ativo),
                        stateExecution = entity.StateExecution,
                        executionOwner = entity.ExecutionOwner
                    });
            }
        }

        public AnswerSheetLot Delete(AnswerSheetLot entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				AnswerSheetLot _entity = GestaoAvaliacaoContext.AnswerSheetLot.FirstOrDefault(a => a.Id == entity.Id);
				_entity.UpdateDate = DateTime.Now;
				_entity.State = Convert.ToByte(EnumState.excluido);

				GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();

				return _entity;
			}
		}

        public void DeleteLot(long parent_Id)
        {
            var sql = new StringBuilder("UPDATE AnswerSheetLot SET UpdateDate = @updateDate, State = @state ");
            sql.AppendLine("WHERE Parent_Id = @id");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                cn.Execute(sql.ToString(),
                    new
                    {
                        id = parent_Id,
                        updateDate = DateTime.Now,
                        state = (byte)EnumState.excluido
                    });
            }
        }

        #endregion
    }
}