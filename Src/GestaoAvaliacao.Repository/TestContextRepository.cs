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
    public class TestContextRepository : ConnectionReadOnly, ITestContextRepository
    {
        private const string CACHE_KEY_GETOBJECT = "TestContextRepository_GetObject_{0}";
        public void Delete(long id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                TestContext testContext = GestaoAvaliacaoContext.TestContext.FirstOrDefault(a => a.Id == id);
                if (testContext != null)
                {
                    testContext.State = Convert.ToByte(EnumState.excluido);
                    testContext.UpdateDate = DateTime.Now;
                    GestaoAvaliacaoContext.Entry(testContext).State = System.Data.Entity.EntityState.Modified;

                    GestaoAvaliacaoContext.SaveChanges();
                    LimparCache_GetObject(id);
                }
            }
        }

        private void LimparCache_GetObject(long Id)
        {
            string key = string.Format(CACHE_KEY_GETOBJECT, Id);
            Cache.ClearCache(key);
        }

        public TestContext Save(TestContext entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                entity.State = Convert.ToByte(EnumState.ativo);

                GestaoAvaliacaoContext.TestContext.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();
                LimparCache_GetObject(entity.Id);

                return entity;
            }
        }

        public TestContext Update(TestContext entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                GestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
                LimparCache_GetObject(entity.Id);

                return entity;
            }
        }

        public TestContext Update(long Id, TestContext entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                TestContext testContext = GestaoAvaliacaoContext.TestContext.FirstOrDefault(a => a.Id == entity.Id);

                testContext.ImagePath = entity.ImagePath;
                testContext.ImagePosition = entity.ImagePosition;
                testContext.Text = entity.Text;
                testContext.Title = entity.Title;



                testContext.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(testContext).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
                LimparCache_GetObject(Id);

                return testContext;
            }
        }

        public void DeleteByTestId(long id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                List<TestContext> testContexts = GestaoAvaliacaoContext.TestContext.Where(i => i.Test_Id == id).ToList();

                if (testContexts != null)
                {
                    testContexts.ForEach(i =>
                    {
                        GestaoAvaliacaoContext.TestContext.Remove(i);
                    });
                }
                GestaoAvaliacaoContext.SaveChanges();
            }
        }
    }
}