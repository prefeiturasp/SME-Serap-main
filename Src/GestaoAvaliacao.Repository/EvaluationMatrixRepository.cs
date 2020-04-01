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
    public class EvaluationMatrixRepository : ConnectionReadOnly, IEvaluationMatrixRepository
    {
        #region CRUD

        public EvaluationMatrix Save(EvaluationMatrix entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;
                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;

                entity.Discipline = gestaoAvaliacaoContext.Discipline.FirstOrDefault(s => s.Id == entity.Discipline.Id);
                entity.ModelEvaluationMatrix = gestaoAvaliacaoContext.ModelEvaluationMatrix.FirstOrDefault(s => s.Id == entity.ModelEvaluationMatrix.Id);

                gestaoAvaliacaoContext.EvaluationMatrix.Add(entity);
                gestaoAvaliacaoContext.SaveChanges();
            }
            return entity;
        }

        public EvaluationMatrix Update(EvaluationMatrix entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                EvaluationMatrix evaluationMatrix = gestaoAvaliacaoContext.EvaluationMatrix.FirstOrDefault(il => il.Id == entity.Id);

                if (!string.IsNullOrEmpty(entity.Description))
                    evaluationMatrix.Description = entity.Description;
                if (!string.IsNullOrEmpty(entity.Edition))
                    evaluationMatrix.Edition = entity.Edition;


                evaluationMatrix.State = entity.State;

                evaluationMatrix.Discipline = gestaoAvaliacaoContext.Discipline.FirstOrDefault(s => s.Id == entity.Discipline.Id);

                evaluationMatrix.UpdateDate = DateTime.Now;

                gestaoAvaliacaoContext.Entry(evaluationMatrix).State = System.Data.Entity.EntityState.Modified;

                gestaoAvaliacaoContext.SaveChanges();

                return evaluationMatrix;
            }
        }

        public IEnumerable<EvaluationMatrix> Load(Guid ent_id, ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.EvaluationMatrix.AsNoTracking().Include("Discipline").Where(x => x.State != (Byte)EnumState.excluido && x.Discipline.State == (Byte)EnumState.ativo && x.ModelEvaluationMatrix.EntityId == ent_id).OrderBy(x => x.Description));
                }
            }
        }

        public void Delete(EvaluationMatrix entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                EvaluationMatrix evaluationMatrix = GestaoAvaliacaoContext.EvaluationMatrix.Include("Skills").Include("EvaluationMatrixCourse").Include("EvaluationMatrixCourse.EvaluationMatrixCourseCurriculumGrades").FirstOrDefault(a => a.Id == entity.Id);

                foreach (var evaluationMatrixSkills in evaluationMatrix.Skills)
                {
                    evaluationMatrixSkills.State = Convert.ToByte(EnumState.excluido);
                    evaluationMatrixSkills.UpdateDate = DateTime.Now;
                }

                foreach (var evaluationMatrixCourses in evaluationMatrix.EvaluationMatrixCourse)
                {
                    evaluationMatrixCourses.State = Convert.ToByte(EnumState.excluido);
                    evaluationMatrixCourses.UpdateDate = DateTime.Now;

                    foreach (var evaluationMatrixCoursesCurriculum in evaluationMatrixCourses.EvaluationMatrixCourseCurriculumGrades)
                    {
                        evaluationMatrixCoursesCurriculum.State = Convert.ToByte(EnumState.excluido);
                        evaluationMatrixCoursesCurriculum.UpdateDate = DateTime.Now;
                    }
                }

                evaluationMatrix.State = Convert.ToByte(EnumState.excluido);
                evaluationMatrix.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(evaluationMatrix).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();

            }
        }

        #endregion

        #region Custom methods

        public IEnumerable<EvaluationMatrix> GetByDiscipline(long idDiscipline)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {

                    IEnumerable<EvaluationMatrix> query =
                        ctx.EvaluationMatrix.AsNoTracking().Include("Discipline").Include("ModelEvaluationMatrix")
                        .Include("EvaluationMatrixCourse").Include("EvaluationMatrixCourse.EvaluationMatrixCourseCurriculumGrades").Where(
                            m => m.Discipline.Id == idDiscipline && m.State == (byte)EnumState.ativo).ToList();

                    return query;
                }
            }
        }
        public IEnumerable<EvaluationMatrix> GetByMatriz(long idMatriz)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {

                    IEnumerable<EvaluationMatrix> query =
                        ctx.EvaluationMatrix.AsNoTracking().Include("Discipline").Include("ModelEvaluationMatrix").Include("EvaluationMatrixCourse").Where(
                            m => m.Id == idMatriz && m.State == (byte)EnumState.ativo).ToList();

                    return query;
                }
            }
        }

        public IEnumerable<EvaluationMatrix> GetComboByDiscipline(long idDiscipline)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT e.Id, e.Description " +
                           "FROM EvaluationMatrix e " +
                           "WHERE e.Discipline_Id = @idDiscipline " +
                           "AND e.State = @state";

                var evaluationMatrix = cn.Query<EvaluationMatrix>(sql, new
                {
                    idDiscipline = idDiscipline,
                    state = (Byte)EnumState.ativo
                });

                return evaluationMatrix;
            }
        }

        public IEnumerable<EvaluationMatrix> Search(String search, String searchEdition, Guid ent_id, ref Pager pager)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return pager.Paginate(gestaoAvaliacaoContext.EvaluationMatrix.Include("Discipline").Where(a => a.Description.Contains(search) && a.Edition.Contains(searchEdition) && a.State != (Byte)EnumState.excluido && a.Discipline.State == (Byte)EnumState.ativo && a.ModelEvaluationMatrix.EntityId == ent_id).OrderBy(x => x.Description));
            }
        }

        public bool ExistsItemMatrix(long idMatrix)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.Item.FirstOrDefault(
                        a => a.EvaluationMatrix.Id == idMatrix && a.State == (Byte)EnumState.ativo && a.EvaluationMatrix.State == (Byte)EnumState.ativo) != null;
                }
            }
        }

        public IEnumerable<EvaluationMatrix> LoadCombo(Guid entityId)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return gestaoAvaliacaoContext.EvaluationMatrix.AsNoTracking().Include("ModelEvaluationMatrix").Where(
                    x => x.State == (Byte)EnumState.ativo && x.ModelEvaluationMatrix.EntityId == entityId).OrderBy(x => x.Description).ToList();
            }
        }

        public IEnumerable<EvaluationMatrix> LoadComboSimple(Guid entityId, long? typeLevelEducation)
        {

            StringBuilder sql = new StringBuilder("SELECT e.Id, e.Description ");
            sql.Append("FROM EvaluationMatrix e ");
            sql.Append("INNER JOIN ModelEvaluationMatrix m ON e.ModelEvaluationMatrix_Id = m.Id ");
            sql.Append("INNER JOIN Discipline d ON e.Discipline_Id = d.Id ");
            sql.Append("WHERE m.EntityId = @entityId ");
            if (typeLevelEducation.HasValue)
                sql.Append("AND d.TypeLevelEducationId = @typeLevelEducation ");
            sql.Append("AND e.State = @state ");
            sql.Append("ORDER BY e.Description DESC ");
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var evaluationMatrix = cn.Query<EvaluationMatrix>(sql.ToString(), new
                {
                    entityId = entityId,
                    typeLevelEducation = typeLevelEducation,
                    state = (Byte)EnumState.ativo
                });

                return evaluationMatrix;
            }
        }

        public EvaluationMatrix LoadUpdate(long evaluationMatrixId)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return gestaoAvaliacaoContext.EvaluationMatrix.Include("ModelEvaluationMatrix")
                    .Include("EvaluationMatrixCourse")
                    .Include("Discipline")
                    .AsNoTracking()
                    .Where(x => x.Id == evaluationMatrixId && x.State != (Byte)EnumState.excluido && x.Discipline.State == (Byte)EnumState.ativo).OrderBy(x => x.Description).FirstOrDefault();
            }
        }

        public IEnumerable<AJX_Select2> LoadMatrizByDiscipline(string description, string discipline, Guid EntityId)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT EM.Id, EM.Description ");
                sql.AppendLine("FROM EvaluationMatrix AS EM WITH (NOLOCK) ");
                sql.AppendLine(string.Format("WHERE @discipline IS NOT NULL AND EM.Discipline_Id IN ({0}) ", discipline));
                sql.AppendLine("AND EM.State = @state ");
                sql.AppendLine("AND (@Description IS NULL OR EM.Description LIKE '%' + @Description + '%') ");
                sql.AppendLine("GROUP BY EM.Id, EM.Description ");
                sql.AppendLine("ORDER BY EM.Description ");

                var lstEvaluationMatrix = cn.Query<EvaluationMatrix>(sql.ToString(), new { state = (Byte)EnumState.ativo, entityid = EntityId, discipline = discipline, Description = description });

                List<AJX_Select2> lstAJX_Select2 = new List<AJX_Select2>();

                foreach (EvaluationMatrix evaluationMatrix in lstEvaluationMatrix)
                {
                    AJX_Select2 AJX_Select2 = new AJX_Select2();

                    AJX_Select2.id = evaluationMatrix.Id.ToString();
                    AJX_Select2.text = evaluationMatrix.Description;

                    lstAJX_Select2.Add(AJX_Select2);
                }

                return lstAJX_Select2;
            }
        }

        public bool ExistsItemMatrix(int value, long id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.ItemLevel.AsNoTracking().FirstOrDefault(
                        a => a.Value.Equals(value) && a.State == (Byte)EnumState.ativo && a.Id != id) != null;
                }
            }
        }

        public EvaluationMatrix GetGradeByMatrix(int Id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query = ctx.EvaluationMatrix.AsNoTracking()
                        .Include("EvaluationMatrixCurriculumGrade")
                        .FirstOrDefault(i => i.Id == Id && i.State == (Byte)EnumState.ativo);

                    return query;
                }
            }
        }

        public EvaluationMatrix Get(long Id)
        {
            var sql = new StringBuilder("SELECT [Id],[Description],[Edition],[CreateDate],[UpdateDate],[State],[Discipline_Id],[ModelEvaluationMatrix_Id] ");
            sql.Append("FROM [EvaluationMatrix] WITH (NOLOCK) ");
            sql.Append("WHERE [Id] = @id AND [State] = @state ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<EvaluationMatrix>(sql.ToString(), new { id = Id, state = (Byte)EnumState.ativo }).FirstOrDefault();
            }
        }

        #endregion
    }
}
