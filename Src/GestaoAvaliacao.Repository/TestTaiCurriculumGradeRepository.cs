using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
   public  class TestTaiCurriculumGradeRepository : ConnectionReadOnly , ITestTaiCurriculumGradeRepository
    {
        public TestTaiCurriculumGrade Save(TestTaiCurriculumGrade entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                try
                {
                    entity.CreateDate = DateTime.Now;
                    entity.State = Convert.ToByte(EnumState.ativo);

                    GestaoAvaliacaoContext.TestTaiCurriculumGrade.Add(entity);
                    GestaoAvaliacaoContext.SaveChanges();
                    return entity;
                }
            
                catch (Exception ex)
                {

                    throw ex ;
                }
            }
        }

        public List<TestTaiCurriculumGrade> GetListByTestId(long testId)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                try
                {
                  return  GestaoAvaliacaoContext.TestTaiCurriculumGrade.Where(x => x.TestId == testId && x.State == 1).ToList();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public TestTaiCurriculumGrade Update(TestTaiCurriculumGrade entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                try
                {
                    GestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    GestaoAvaliacaoContext.SaveChanges();
                
                    return entity;
                }

                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }

     
    }
}



