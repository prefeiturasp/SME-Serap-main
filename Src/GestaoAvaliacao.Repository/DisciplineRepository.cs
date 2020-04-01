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
	public class DisciplineRepository : ConnectionReadOnly, IDisciplineRepository
	{
		#region ReadyOnly

		public IEnumerable<Discipline> Load(Guid EntityId, ref Pager pager)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"WITH NumberedDiscipline AS " +
							"( " +
							   "SELECT Id, Description, DisciplineTypeId, TypeLevelEducationId, " +
							   "ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber " +
							   "FROM Discipline " +
							   "WHERE State  = @state " +
							   "AND EntityId = @entityid " +
							") " +
						   "SELECT Id, Description, DisciplineTypeId, TypeLevelEducationId " +
						   "FROM NumberedDiscipline " +
						   "WHERE RowNumber > ( @pageSize * @page ) " +
						   "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
						   "ORDER BY RowNumber";

				var countSql = @"SELECT COUNT(id) " +
								"FROM Discipline " +
								"WHERE State = @state " +
								"AND EntityId = @entityid";

				var discipline = cn.Query<Discipline>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage });
				var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, entityid = EntityId });

				pager.SetTotalPages((int)Math.Ceiling(discipline.Count() / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return discipline;
			}
		}

		public Discipline Get(long id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT Id, Description, DisciplineTypeId, TypeLevelEducationId " +
						   "FROM Discipline " +
						   "WHERE Id = @id ";

				var discipline = cn.Query<Discipline>(sql, new { id = id }).FirstOrDefault();

				return discipline;
			}
		}

		public IEnumerable<Discipline> Search(Guid EntityId, String search, ref Pager pager)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"WITH NumberedDiscipline AS " +
							"( " +
							   "SELECT Id, Description, DisciplineTypeId, TypeLevelEducationId, " +
							   "ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber " +
							   "FROM Discipline " +
							   "WHERE State  = @state " +
							   "AND EntityId = @entityid " +
							   "AND (@search IS NULL OR Description Like '%' + @search + '%') " +
							") " +
						   "SELECT Id, Description, DisciplineTypeId, TypeLevelEducationId " +
						   "FROM NumberedDiscipline " +
						   "WHERE RowNumber > ( @pageSize * @page ) " +
						   "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
						   "ORDER BY RowNumber";

				var countSql = @"SELECT COUNT(id) " +
								"FROM Discipline " +
								"WHERE State = @state " +
								"AND (@search IS NULL OR Description Like '%' + @search + '%') " +
								"AND EntityId = @entityid";

				var discipline = cn.Query<Discipline>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage, search = search });
				var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, entityid = EntityId, search = search });

				pager.SetTotalPages((int)Math.Ceiling(discipline.Count() / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return discipline;
			}
		}

		public bool ExistsMatrix(long Id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT COUNT(Id) " +
						   "FROM EvaluationMatrix " +
						   "WHERE Discipline_Id = @id " +
						   "AND State = @state ";

				var count = cn.Query<int>(sql, new { id = Id, state = (Byte)EnumState.ativo }).FirstOrDefault();

				return count > 0;
			}
		}

		public IEnumerable<Discipline> LoadCustom(Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT Id, Description, DisciplineTypeId, TypeLevelEducationId " +
						   "FROM Discipline " +
						   "WHERE State = @state " +
						   "AND EntityId = @entityid " +
						   "ORDER BY Description ";

				var discipline = cn.Query<Discipline>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId });

				return discipline;
			}
		}

		public IEnumerable<Discipline> SearchAllDisciplines(Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT Id, (Description + ' - (' +  tne_nome + ')') AS Description, DisciplineTypeId, TypeLevelEducationId " +
						   "FROM Discipline AS DIS WITH (NOLOCK) " +
						   "INNER JOIN SGP_ACA_TipoNivelEnsino TNE WITH (NOLOCK) ON TNE.tne_id = DIS.TypeLevelEducationId " +
						   "WHERE State = @state " +
						   "AND EntityId = @entityid " +
						   "ORDER BY Description ";

				var discipline = cn.Query<Discipline>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId });

				return discipline;
			}
		}

		public IEnumerable<Discipline> LoadComboHasMatrix(Guid entityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT d.Id, d.Description, d.DisciplineTypeId, d.TypeLevelEducationId " +
						   "FROM Discipline d " +
						   "INNER JOIN EvaluationMatrix e ON d.Id = e.Discipline_Id " +
						   "WHERE d.State  = @state " +
						   "AND d.EntityId = @entityid " +
						   "AND e.State    = @state " +
						   "GROUP BY d.Id, d.Description, d.DisciplineTypeId, d.TypeLevelEducationId " +
						   "ORDER BY d.Description ";
				var discipline = cn.Query<Discipline>(sql, new { state = (Byte)EnumState.ativo, entityid = entityId });

				return discipline;
			}
		}

		public IEnumerable<Discipline> LoadComboByTest(long test_id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT 
								ISNULL(T.Discipline_Id, Em.Discipline_Id) AS Id, 
								D.Description
							FROM 
								Test AS T WITH (NOLOCK)
								INNER JOIN Block B WITH (NOLOCK) ON T.Id = B.Test_Id AND B.State <> 3
								INNER JOIN BlockItem Bi WITH (NOLOCK) ON B.Id = Bi.Block_Id AND Bi.State <> 3
								INNER JOIN Item I WITH (NOLOCK) ON Bi.Item_Id = I.Id AND I.State <> 3
								INNER JOIN EvaluationMatrix Em WITH (NOLOCK) ON I.EvaluationMatrix_Id = Em.Id AND Em.State <> 3
								INNER JOIN Discipline D WITH (NOLOCK) ON D.Id = ISNULL(T.Discipline_Id, Em.Discipline_Id) AND D.State <> 3
							WHERE
								T.Id = @Test_Id
								AND T.State <> 3
							GROUP BY
								ISNULL(T.Discipline_Id, Em.Discipline_Id), D.Description ";

				var discipline = cn.Query<Discipline>(sql, new { Test_Id = test_id });

				return discipline;
			}
		}
		public IEnumerable<Discipline> GetDisciplinesByTestSubGroup_Id(long TestSubGroup_Id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT 
								ISNULL(T.Discipline_Id, Em.Discipline_Id) AS Id, 
								D.Description
							FROM 
								Test AS T WITH (NOLOCK)
								INNER JOIN Block B WITH (NOLOCK) ON T.Id = B.Test_Id AND B.State <> 3
								INNER JOIN BlockItem Bi WITH (NOLOCK) ON B.Id = Bi.Block_Id AND Bi.State <> 3
								INNER JOIN Item I WITH (NOLOCK) ON Bi.Item_Id = I.Id AND I.State <> 3
								INNER JOIN EvaluationMatrix Em WITH (NOLOCK) ON I.EvaluationMatrix_Id = Em.Id AND Em.State <> 3
								INNER JOIN Discipline D WITH (NOLOCK) ON D.Id = ISNULL(T.Discipline_Id, Em.Discipline_Id) AND D.State <> 3
							WHERE
								T.TestSubGroup_Id = @TestSubGroup_Id
								AND T.State <> 3
							GROUP BY
								ISNULL(T.Discipline_Id, Em.Discipline_Id), D.Description ";

				var discipline = cn.Query<Discipline>(sql, new { TestSubGroup_Id = TestSubGroup_Id });

				return discipline;
			}
		}

		public List<AJX_Select2> LoadDisciplineByKnowledgeArea(string description, string knowledgeAreas, Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();

				StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT DIS.Id, DIS.Description ");
                sql.AppendLine("FROM KnowledgeArea AS KA WITH (NOLOCK) ");
                sql.AppendLine("INNER JOIN KnowledgeAreaDiscipline AS KAD WITH (NOLOCK) ");
                sql.AppendLine("ON KAD.KnowledgeArea_Id = KA.Id ");
                sql.AppendLine("INNER JOIN dbo.Discipline AS DIS WITH (NOLOCK) ");
                sql.AppendLine("ON DIS.Id = KAD.Discipline_Id ");
                sql.AppendLine(string.Format("WHERE @knowledgeAreas IS NOT NULL AND KA.Id IN ({0}) ", knowledgeAreas));
                sql.AppendLine("AND DIS.State = @state ");
                sql.AppendLine("AND KAD.State = @state ");
                sql.AppendLine("AND KA.State = @state ");
                sql.AppendLine("AND KA.EntityId = @entityid ");
                sql.AppendLine("AND KA.EntityId = @entityid ");
                sql.AppendLine("AND (@Description IS NULL OR DIS.Description LIKE '%' + @Description + '%') "); 
                sql.AppendLine("GROUP BY DIS.Id, DIS.Description ");
                sql.AppendLine("ORDER BY DIS.Description ");

				var lstKnowledgeArea = cn.Query<KnowledgeArea>(sql.ToString(), new { state = (Byte)EnumState.ativo, entityid = EntityId, knowledgeAreas = knowledgeAreas, Description = description });

				List<AJX_Select2> lstAJX_Select2 = new List<AJX_Select2>();

				foreach (KnowledgeArea knowledgeArea in lstKnowledgeArea)
				{
					AJX_Select2 AJX_Select2 = new AJX_Select2();

					AJX_Select2.id = knowledgeArea.Id.ToString();
					AJX_Select2.text = knowledgeArea.Description;

					lstAJX_Select2.Add(AJX_Select2);
				}

				return lstAJX_Select2;
			}
		}

		#endregion

		#region CRUD

		public Discipline Save(Discipline entity)
		{
			using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				DateTime dateNow = DateTime.Now;
				entity.UpdateDate = dateNow;
				entity.State = Convert.ToByte(EnumState.ativo);

				gestaoAvaliacaoContext.Discipline.Add(entity);
				gestaoAvaliacaoContext.SaveChanges();
			}
			return entity;
		}

		public void Delete(Discipline entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				Discipline discipline = GestaoAvaliacaoContext.Discipline.FirstOrDefault(a => a.Id == entity.Id);

				discipline.State = Convert.ToByte(EnumState.excluido);
				discipline.UpdateDate = DateTime.Now;

				GestaoAvaliacaoContext.Entry(discipline).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();
			}
		}

		#endregion

		#region Custom methods

		public List<Discipline> SaveRange(List<Discipline> listEntity)
		{
			using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				DateTime dateNow = DateTime.Now;

				foreach (Discipline disciplina in listEntity)
				{
					disciplina.UpdateDate = dateNow;
					disciplina.State = Convert.ToByte(EnumState.ativo);
				}

				gestaoAvaliacaoContext.Discipline.AddRange(listEntity);
				gestaoAvaliacaoContext.SaveChanges();
			}
			return listEntity;
		}

		#endregion
	}
}
