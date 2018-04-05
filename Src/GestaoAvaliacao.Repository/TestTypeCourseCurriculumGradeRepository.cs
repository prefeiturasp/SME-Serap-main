using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Repository
{
    public class TestTypeCourseCurriculumGradeRepository : ITestTypeCourseCurriculumGradeRepository
    {
        #region CRUD
        public TestTypeCourseCurriculumGrade SaveList(TestTypeCourseCurriculumGrade entity, int typeLevelEducationId, int modalityId)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                gestaoAvaliacaoContext.TestTypeCourseCurriculumGrade.Add(entity);
                gestaoAvaliacaoContext.SaveChanges();
            }
            return entity;
        }

        public void Delete(TestTypeCourseCurriculumGrade entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                TestTypeCourseCurriculumGrade testTypeCourseCurriculumGrade = GestaoAvaliacaoContext.TestTypeCourseCurriculumGrade.FirstOrDefault(a => a.Id == entity.Id);

                testTypeCourseCurriculumGrade.State = Convert.ToByte(EnumState.excluido);
                testTypeCourseCurriculumGrade.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(testTypeCourseCurriculumGrade).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void Update(TestTypeCourseCurriculumGrade entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                TestTypeCourseCurriculumGrade testTypeCourseCurriculumGrade = gestaoAvaliacaoContext.TestTypeCourseCurriculumGrade.FirstOrDefault(il => il.Id == entity.Id);

                entity.UpdateDate = DateTime.Now;

                gestaoAvaliacaoContext.Entry(testTypeCourseCurriculumGrade).CurrentValues.SetValues(entity);

                gestaoAvaliacaoContext.Entry(testTypeCourseCurriculumGrade).State = System.Data.Entity.EntityState.Modified;

                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        public TestTypeCourseCurriculumGrade Get(long id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.TestTypeCourseCurriculumGrade.AsNoTracking().FirstOrDefault(bt => bt.Id == id);
                }
            }
        }
        #endregion

        #region Custom methods

        public List<TestTypeCourseCurriculumGrade> GetCurriculumGradesByTestType(int testTypeId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    var curriculumGradesDistinct = GestaoAvaliacaoContext.TestTypeCourseCurriculumGrade.AsNoTracking().Where(a => a.TestTypeCourse.TestType.Id == testTypeId
                        && a.State == (Byte)EnumState.ativo && a.TypeCurriculumGradeId > 0).Select(i => new
                        {
                            TypeCurriculumGradeId = i.TypeCurriculumGradeId
                        }).Distinct().ToList();

                    List<TestTypeCourseCurriculumGrade> query = curriculumGradesDistinct.Select(i => new TestTypeCourseCurriculumGrade()
                    {
                        TypeCurriculumGradeId = i.TypeCurriculumGradeId
                    }).ToList();

                    return query;
                }
            }
        }

        public List<TestTypeCourseCurriculumGrade> GetCurriculumGradesByTestTypeCourse(int testTypeId, int courseId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.TestTypeCourseCurriculumGrade.AsNoTracking().Where(a => a.TestTypeCourse.Id == testTypeId && a.TestTypeCourse.CourseId == courseId
                        && a.State == (Byte)EnumState.ativo).ToList();

                }
            }
        }

        public TestTypeCourseCurriculumGrade GetByCurriculumGradeId(long curriculumGradeId, int testTypeId, int courseId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.TestTypeCourseCurriculumGrade.AsNoTracking().FirstOrDefault(x => x.CurriculumGradeId == curriculumGradeId && x.TestTypeCourse.TestType.Id == testTypeId && x.TestTypeCourse.CourseId == courseId);
                }
            }
        }

        #endregion
    }
}
