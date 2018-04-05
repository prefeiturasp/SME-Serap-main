using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Repository
{
    public class EvaluationMatrixCourseCurriculumGradeRepository : IEvaluationMatrixCourseCurriculumGradeRepository
    {
        #region CRUD
        public EvaluationMatrixCourseCurriculumGrade SaveList(EvaluationMatrixCourseCurriculumGrade entity, int typeLevelEducationId, int modalityId)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                gestaoAvaliacaoContext.EvaluationMatrixCourseCurriculumGrade.Add(entity);
                gestaoAvaliacaoContext.SaveChanges();
            }
            return entity;
        }

        public void Delete(EvaluationMatrixCourseCurriculumGrade entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                EvaluationMatrixCourseCurriculumGrade evaluationMatrixCourseCurriculumGrade = GestaoAvaliacaoContext.EvaluationMatrixCourseCurriculumGrade.FirstOrDefault(a => a.Id == entity.Id);

                evaluationMatrixCourseCurriculumGrade.State = Convert.ToByte(EnumState.excluido);
                evaluationMatrixCourseCurriculumGrade.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(evaluationMatrixCourseCurriculumGrade).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void Update(EvaluationMatrixCourseCurriculumGrade entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                entity.UpdateDate = DateTime.Now;

                gestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;

                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        public EvaluationMatrixCourseCurriculumGrade Get(long id)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return gestaoAvaliacaoContext.EvaluationMatrixCourseCurriculumGrade.AsNoTracking().FirstOrDefault(bt => bt.Id == id);
            }

        }
        #endregion

        #region Custom methods

        public List<EvaluationMatrixCourseCurriculumGrade> GetCurriculumGradesByMatrix(int evaluationMatrixId)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var curriculumGradesDistinct = GestaoAvaliacaoContext.EvaluationMatrixCourseCurriculumGrade.AsNoTracking().Where(a => a.EvaluationMatrixCourse.EvaluationMatrix.Id == evaluationMatrixId
                    && a.State == (Byte)EnumState.ativo && a.TypeCurriculumGradeId > 0).Select(i => new
                    {
                        TypeCurriculumGradeId = i.TypeCurriculumGradeId
                    }).Distinct().ToList();

                List<EvaluationMatrixCourseCurriculumGrade> query = curriculumGradesDistinct.Select(i => new EvaluationMatrixCourseCurriculumGrade()
                {
                    TypeCurriculumGradeId = i.TypeCurriculumGradeId
                }).ToList();

                return query;
            }
        }

        public List<EvaluationMatrixCourseCurriculumGrade> GetCurriculumGradesByMatrixCourse(int evaluationMatrixId, int courseId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.EvaluationMatrixCourseCurriculumGrade.AsNoTracking().Where(a => a.EvaluationMatrixCourse.EvaluationMatrix.Id == evaluationMatrixId && a.EvaluationMatrixCourse.CourseId == courseId
                        && a.State == (Byte)EnumState.ativo).ToList();

                }
            }
        }

        public EvaluationMatrixCourseCurriculumGrade GetByCurriculumGradeId(long curriculumGradeId, int evaluationMatrixId, int courseId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {

                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.EvaluationMatrixCourseCurriculumGrade.AsNoTracking().FirstOrDefault(x => x.CurriculumGradeId == curriculumGradeId && x.EvaluationMatrixCourse.EvaluationMatrix.Id == evaluationMatrixId && x.EvaluationMatrixCourse.CourseId == courseId);
                }
            }
        }

        #endregion
    }
}
