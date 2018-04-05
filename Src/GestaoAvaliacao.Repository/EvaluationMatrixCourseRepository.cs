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
    public class EvaluationMatrixCourseRepository : ConnectionReadOnly, IEvaluationMatrixCourseRepository
    {
        #region Entity Framework

        public EvaluationMatrixCourse Save(EvaluationMatrixCourse entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                gestaoAvaliacaoContext.EvaluationMatrixCourse.Add(entity);
                gestaoAvaliacaoContext.SaveChanges();
            }
            return entity;
        }

        public void Update(EvaluationMatrixCourse entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                gestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void Delete(EvaluationMatrixCourse entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                EvaluationMatrixCourse evaluationMatrixCourse = GestaoAvaliacaoContext.EvaluationMatrixCourse.Include("EvaluationMatrixCourseCurriculumGrades").FirstOrDefault(a => a.Id == entity.Id);

                foreach (var evaluationMatrixCourseCurriculumGrades in evaluationMatrixCourse.EvaluationMatrixCourseCurriculumGrades)
                {
                    evaluationMatrixCourseCurriculumGrades.State = Convert.ToByte(EnumState.excluido);
                    evaluationMatrixCourseCurriculumGrades.UpdateDate = DateTime.Now;
                }

                evaluationMatrixCourse.State = Convert.ToByte(EnumState.excluido);
                evaluationMatrixCourse.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(evaluationMatrixCourse).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public bool ExistsItemAndLastCourse(long idCourse, long evaluationMatrixId)
        {
            bool lastType = false;
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    //Retorna todos os itens curriculuns grades da matriz
                    IEnumerable<ItemCurriculumGrade> itemCurriculumGrade = GestaoAvaliacaoContext.ItemCurriculumGrade.Where(
                        icg => icg.State == (Byte)EnumState.ativo && icg.Item.EvaluationMatrix.Id == evaluationMatrixId && icg.Item.State == (Byte)EnumState.ativo
                            && icg.Item.EvaluationMatrix.State == (Byte)EnumState.ativo);

                    if (itemCurriculumGrade != null)
                    {
                        //Retorna todos os curriculuns grades daquela matriz que não são do curso passado por parametro.
                        IEnumerable<EvaluationMatrixCourseCurriculumGrade> evaluationMatrixCourseCurriculumGradeAll = GestaoAvaliacaoContext.EvaluationMatrixCourseCurriculumGrade.Include("EvaluationMatrixCourse").Where(
                     a => a.State == (Byte)EnumState.ativo && a.EvaluationMatrixCourse.State == (Byte)EnumState.ativo
                         && a.EvaluationMatrixCourse.EvaluationMatrix.Id == evaluationMatrixId && a.EvaluationMatrixCourse.Id != idCourse).AsEnumerable();

                        //verifica se na lista de todos os curriculuns grades da matriz contém todos os itens curriculums grades da matriz
                        var contains = (
                                from ev in evaluationMatrixCourseCurriculumGradeAll
                                join item in itemCurriculumGrade
                                    on ev.TypeCurriculumGradeId equals item.TypeCurriculumGradeId
                                select
                                    ev
                                    ).Count() < itemCurriculumGrade.Count();

                        //se o count das listas forem iguais retorna false e deixa excluir o curso, senão retorna true e impede de excluir o curso
                        lastType = contains;
                    }

                    return lastType;
                }
            }
        }

        public bool ExistCourse(int courseId, int modalityId, long evaluationMatrixId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.EvaluationMatrixCourse.AsNoTracking().FirstOrDefault(
                        a => a.CourseId == courseId && a.ModalityId == modalityId && a.EvaluationMatrix.Id == evaluationMatrixId &&
                            a.State == (Byte)EnumState.ativo && a.EvaluationMatrix.State == (Byte)EnumState.ativo) != null;
                }
            }
        }

        #endregion

        #region Dapper

        public EvaluationMatrixCourse Get(long Id)
        {
            var sql = new StringBuilder("SELECT [Id],[CourseId],[TypeLevelEducationId],[ModalityId],[CreateDate],[UpdateDate],[State],[EvaluationMatrix_Id] ");
            sql.Append("FROM [EvaluationMatrixCourse] WITH (NOLOCK) ");
            sql.Append("WHERE [Id] = @id AND [State] <> @state ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<EvaluationMatrixCourse>(sql.ToString(), new { id = Id, state = (byte)EnumState.excluido }).FirstOrDefault();
            }
        }

        public IEnumerable<EvaluationMatrixCourse> Search(int evaluationMatrixId, ref Pager pager)
        {
            var sql = new StringBuilder("WITH NumberedResult AS ( ");
            sql.Append("SELECT EMC.[Id],EMC.[CourseId],EMC.[ModalityId],EMC.[State] ");
            sql.Append(", ROW_NUMBER() OVER (ORDER BY EMC.[CreateDate]) AS RowNumber ");
            sql.Append("FROM [EvaluationMatrixCourse] EMC WITH (NOLOCK) ");
            sql.Append("WHERE EMC.[State] = @state ");
            sql.Append("AND EMC.[EvaluationMatrix_Id] = @evaluationMatrixId ");
            sql.Append(") ");

            sql.Append("SELECT Id,CourseId,ModalityId,State ");
            sql.Append("FROM NumberedResult ");
            sql.Append("WHERE RowNumber > ( @pageSize * @page ) ");
            sql.Append("AND RowNumber <= ( ( @page + 1 ) * @pageSize ) ");

            sql.Append("SELECT COUNT(EMC.[Id]) ");
            sql.Append("FROM [EvaluationMatrixCourse] EMC WITH (NOLOCK) ");
            sql.Append("WHERE EMC.[State] = @state ");
            sql.Append("AND EMC.[EvaluationMatrix_Id] = @evaluationMatrixId ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(), new { evaluationMatrixId = evaluationMatrixId, state = (Byte)EnumState.ativo, pageSize = pager.PageSize, page = pager.CurrentPage });

                var entries = query.Read<EvaluationMatrixCourse>();
                var count = query.Read<int>().FirstOrDefault();

                pager.SetTotalItens(count);
                pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));

                foreach (var entry in entries)
                {
                    var parentId = entry.Id;

                    var sqlMulti = new StringBuilder("SELECT EMG.[Id],EMG.[CurriculumGradeId],EMG.[TypeCurriculumGradeId],EMG.[Ordem],EMG.[State] ");
                    sqlMulti.Append("FROM [EvaluationMatrixCourseCurriculumGrade] EMG WITH (NOLOCK) ");
                    sqlMulti.Append("WHERE EMG.[State] = @state ");
                    sqlMulti.Append("AND EMG.[EvaluationMatrixCourse_Id] = @parentId");

                    var list = cn.Query<EvaluationMatrixCourseCurriculumGrade>(sqlMulti.ToString(), new { state = (Byte)EnumState.ativo, parentId = parentId });

                    entry.EvaluationMatrixCourseCurriculumGrades.AddRange(list);
                }

                return entries;
            }
        }

        public IEnumerable<EvaluationMatrixCourse> Load(ref Pager pager)
        {
            var sql = new StringBuilder("WITH NumberedResult AS ( ");
            sql.Append("SELECT EMC.[Id],EMC.[CourseId],EMC.[ModalityId],EMC.[State] ");
            sql.Append(", ROW_NUMBER() OVER (ORDER BY EMC.[CreateDate]) AS RowNumber ");
            sql.Append("FROM [EvaluationMatrixCourse] EMC WITH (NOLOCK) ");
            sql.Append("WHERE EMC.[State] = @state ");
            sql.Append(") ");

            sql.Append("SELECT Id,CourseId,ModalityId,State ");
            sql.Append("FROM NumberedResult ");
            sql.Append("WHERE RowNumber > ( @pageSize * @page ) ");
            sql.Append("AND RowNumber <= ( ( @page + 1 ) * @pageSize ) ");

            sql.Append("SELECT COUNT(EMC.[Id]) ");
            sql.Append("FROM [EvaluationMatrixCourse] EMC WITH (NOLOCK) ");
            sql.Append("WHERE EMC.[State] = @state ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(), new { state = (Byte)EnumState.ativo, pageSize = pager.PageSize, page = pager.CurrentPage });

                var entries = query.Read<EvaluationMatrixCourse>();
                var count = query.Read<int>().FirstOrDefault();

                pager.SetTotalItens(count);
                pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));

                foreach (var entry in entries)
                {
                    var parentId = entry.Id;

                    var sqlMulti = new StringBuilder("SELECT EMG.[Id],EMG.[CurriculumGradeId],EMG.[TypeCurriculumGradeId],EMG.[Ordem],EMG.[State] ");
                    sqlMulti.Append("FROM [EvaluationMatrixCourseCurriculumGrade] EMG WITH (NOLOCK) ");
                    sqlMulti.Append("WHERE EMG.[State] = @state ");
                    sqlMulti.Append("AND EMG.[EvaluationMatrixCourse_Id] = @parentId");

                    var list = cn.Query<EvaluationMatrixCourseCurriculumGrade>(sqlMulti.ToString(), new { state = (Byte)EnumState.ativo, parentId = parentId });

                    entry.EvaluationMatrixCourseCurriculumGrades.AddRange(list);
                }

                return entries;
            }
        }

        #endregion
    }
}
