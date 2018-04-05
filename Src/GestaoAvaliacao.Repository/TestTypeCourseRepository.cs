using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Repository
{
    public class TestTypeCourseRepository : ITestTypeCourseRepository
    {
        #region CRUD

        public TestTypeCourse Save(TestTypeCourse entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                gestaoAvaliacaoContext.TestTypeCourse.Add(entity);
                gestaoAvaliacaoContext.SaveChanges();
            }
            return entity;
        }

        public void Update(TestTypeCourse entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                gestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        public TestTypeCourse Get(long id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.TestTypeCourse.AsNoTracking().Include("TestTypeCourseCurriculumGrades").FirstOrDefault(bt => bt.Id == id);
                }
            }
        }

        public IEnumerable<TestTypeCourse> Load(ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.TestTypeCourse.AsNoTracking().Where(x => x.State == (Byte)EnumState.ativo).OrderBy(a => a.CreateDate)).AsQueryable();
                }
            }
        }

        public void Delete(TestTypeCourse entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                TestTypeCourse testTypeCourse = GestaoAvaliacaoContext.TestTypeCourse.Include("TestTypeCourseCurriculumGrades").FirstOrDefault(a => a.Id == entity.Id);

                foreach (var testTypeCourseCurriculumGrades in testTypeCourse.TestTypeCourseCurriculumGrades)
                {
                    testTypeCourseCurriculumGrades.State = Convert.ToByte(EnumState.excluido);
                    testTypeCourseCurriculumGrades.UpdateDate = DateTime.Now;
                }

                testTypeCourse.State = Convert.ToByte(EnumState.excluido);
                testTypeCourse.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(testTypeCourse).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion

        #region Custom methods

        public IEnumerable<TestTypeCourse> Search(int testTypeId, ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.TestTypeCourse.Include("TestTypeCourseCurriculumGrades").AsNoTracking()
                        .Where(a => a.TestType.Id == testTypeId && a.State == (Byte)EnumState.ativo
                            && a.TestType.State == (Byte)EnumState.ativo).OrderBy(a => a.CreateDate)).AsQueryable();
                }
            }
        }

        public bool ExistCourse(int courseId, int modalityId, long testTypeId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.TestTypeCourse.AsNoTracking().FirstOrDefault(
                        a => a.CourseId == courseId && a.ModalityId == modalityId && a.TestType.Id == testTypeId &&
                            a.State == (Byte)EnumState.ativo && a.TestType.State == (Byte)EnumState.ativo) != null;
                }
            }
        }

        #endregion
    }
}
