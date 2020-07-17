using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class TestTypeRepository : ConnectionReadOnly, ITestTypeRepository
    {
        #region CRUD

        public async Task<TestType> SaveAsync(TestType entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                if (entity.FormatType != null)
                    entity.FormatType = GestaoAvaliacaoContext.FormatType.FirstOrDefault(f => f.Id == entity.FormatType.Id);

                if (entity.ItemType != null)
                    entity.ItemType = GestaoAvaliacaoContext.ItemType.FirstOrDefault(f => f.Id == entity.ItemType.Id);

                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;
                entity.State = Convert.ToByte(EnumState.ativo);

                foreach (var t in entity.TestTypeItemLevel)
                {
                    t.ItemLevel = GestaoAvaliacaoContext.ItemLevel.FirstOrDefault(f => f.Id == t.ItemLevel.Id);
                }

                GestaoAvaliacaoContext.TestType.Add(entity);
                await GestaoAvaliacaoContext.SaveChangesAsync();

                return entity;
            }
        }

        public async Task<TestType> UpdateAsync(TestType entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var testType = await gestaoAvaliacaoContext.TestType
                    .Include("TestTypeItemLevel.ItemLevel")
                    .Include("FormatType")
                    .Include("ItemType")
                    .Include("TestTypeDeficiencies")
                    .FirstOrDefaultAsync(a => a.Id == entity.Id);

                var itemLevelDatabase = testType.TestTypeItemLevel != null ? testType.TestTypeItemLevel.Where(x => x.State == (byte)EnumState.ativo).Select(x => x.ItemLevel.Id).ToList() : null;
                var itemLevelFront = entity.TestTypeItemLevel != null ? entity.TestTypeItemLevel.Select(x => x.ItemLevel.Id).ToList() : null;

                var testTypeItemLevelToExclude = itemLevelDatabase.Except(itemLevelFront);
                var testTypeItemLevelToAdd = itemLevelFront.Except(itemLevelDatabase);
                //Exclui testTypeItemLevel que não vieram no front e estão ativos no banco de dados
                if (testTypeItemLevelToExclude != null && testTypeItemLevelToExclude.Any())
                {
                    foreach (var testTypeItemLevel in testType.TestTypeItemLevel.Where(s => s.State == (Byte)EnumState.ativo && testTypeItemLevelToExclude.Contains(s.ItemLevel.Id)))
                    {

                        if (testTypeItemLevel != null)
                        {
                            testTypeItemLevel.State = Convert.ToByte(EnumState.excluido);
                            testTypeItemLevel.UpdateDate = DateTime.Now;
                        }
                    }
                }
                //inclui testTypeItemLevel novos
                if (testTypeItemLevelToAdd != null && testTypeItemLevelToAdd.Any())
                {
                    foreach (TestTypeItemLevel testTypeItemLevel in entity.TestTypeItemLevel.Where(s => testTypeItemLevelToAdd.Contains(s.ItemLevel.Id)))
                    {
                        if (testTypeItemLevel != null)
                        {
                            testTypeItemLevel.ItemLevel = await gestaoAvaliacaoContext.ItemLevel.FirstOrDefaultAsync(f => f.Id == testTypeItemLevel.ItemLevel.Id);
                            testTypeItemLevel.TestType = await gestaoAvaliacaoContext.TestType.FirstOrDefaultAsync(f => f.Id == entity.Id);
                            gestaoAvaliacaoContext.TestTypeItemLevel.Add(testTypeItemLevel);
                            gestaoAvaliacaoContext.SaveChanges();
                        }
                    }
                }
                //atualiza valores dos testTypeItemLevel
                foreach (var testTypeItemLevel in testType.TestTypeItemLevel.Where(s => s.State == (Byte)EnumState.ativo
                    && !testTypeItemLevelToExclude.Contains(s.ItemLevel.Id) && !testTypeItemLevelToAdd.Contains(s.ItemLevel.Id)))
                {
                    if (testTypeItemLevel != null)
                    {
                        testTypeItemLevel.Value = entity.TestTypeItemLevel.FirstOrDefault(t => t.Id == testTypeItemLevel.Id).Value;
                        testTypeItemLevel.UpdateDate = DateTime.Now;
                    }
                }

                ExcludeTestTypeDeficiencies(testType);
                if (entity.TargetToStudentsWithDeficiencies)
                {
                    var testTypeDeficienciesIdsInDatabase = testType.TestTypeDeficiencies.Select(x => x.Id);
                    foreach(var testTypeDeficiencyToUpdate in entity.TestTypeDeficiencies)
                    {
                        var testTypeDeficiency = testType.TestTypeDeficiencies.FirstOrDefault(x => x.Id == testTypeDeficiencyToUpdate.Id);
                        if(testTypeDeficiency is null)
                        {
                            testType.TestTypeDeficiencies.Add(testTypeDeficiencyToUpdate);
                        }
                        else
                        {
                            testTypeDeficiency.State = Convert.ToByte(EnumState.ativo);
                        }
                    }
                }

                if (entity.FormatType != null)
                    testType.FormatType = await gestaoAvaliacaoContext.FormatType.FirstOrDefaultAsync(f => f.Id == entity.FormatType.Id);
                else
                    testType.FormatType = null;

                if (entity.ItemType != null)
                    testType.ItemType = await gestaoAvaliacaoContext.ItemType.FirstOrDefaultAsync(f => f.Id == entity.ItemType.Id);
                else
                    testType.ItemType = null;

                testType.Description = entity.Description;
                testType.FrequencyApplication = entity.FrequencyApplication;
                testType.Global = entity.Global;
                testType.TypeLevelEducationId = entity.TypeLevelEducationId;
                testType.ModelTest_Id = entity.ModelTest_Id;

                testType.UpdateDate = DateTime.Now;

                gestaoAvaliacaoContext.Entry(testType).State = System.Data.Entity.EntityState.Modified;
                await gestaoAvaliacaoContext.SaveChangesAsync();

                return testType;
            }
        }

        private void ExcludeTestTypeDeficiencies(TestType testType)
        {
            foreach (var testTypeDecifiency in testType?.TestTypeDeficiencies)
            {
                testTypeDecifiency.State = Convert.ToByte(EnumState.excluido);
                testTypeDecifiency.UpdateDate = DateTime.Now;
            }
        }

        public void UnsetModelTest(TestType entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                TestType testType = gestaoAvaliacaoContext.TestType.FirstOrDefault(a => a.Id == entity.Id);

                testType.ModelTest_Id = null;
                testType.UpdateDate = DateTime.Now;

                gestaoAvaliacaoContext.Entry(testType).State = System.Data.Entity.EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();
            }
        }


        public async Task<TestType> GetAsync(long id, Guid EntityId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return await GestaoAvaliacaoContext.TestType
                        .AsNoTracking()
                        .Include("TestTypeItemLevel.ItemLevel")
                        .Include("FormatType")
                        .Include("ItemType")
                        .Include("TestTypeCourses.TestTypeCourseCurriculumGrades")
                        .Include("TestTypeCourses")
                        .Include("TestTypeDeficiencies")
                        .FirstOrDefaultAsync(a => a.Id == id && a.EntityId == EntityId);
                }
            }
        }

        public IEnumerable<TestType> Load(ref Pager pager, Guid EntityId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.TestType
                        .AsNoTracking()
                        .Where(x => x.State == (Byte)EnumState.ativo && x.EntityId == EntityId)
                        .OrderBy(x => x.Description));
                }
            }
        }


        public async Task DeleteAsync(long id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var testType = await GestaoAvaliacaoContext.TestType
                    .Include("TestTypeItemLevel")
                    .Include("TestTypeCourses.TestTypeCourseCurriculumGrades")
                    .FirstOrDefaultAsync(a => a.Id == id);

                foreach (var testTypeItemLevel in testType.TestTypeItemLevel)
                {
                    testTypeItemLevel.State = Convert.ToByte(EnumState.excluido);
                    testTypeItemLevel.UpdateDate = DateTime.Now;
                }

                foreach (var TestTypeCourses in testType.TestTypeCourses)
                {
                    TestTypeCourses.State = Convert.ToByte(EnumState.excluido);
                    TestTypeCourses.UpdateDate = DateTime.Now;

                    foreach (var TestTypeCourseCurriculumGrades in TestTypeCourses.TestTypeCourseCurriculumGrades)
                    {
                        TestTypeCourseCurriculumGrades.State = Convert.ToByte(EnumState.excluido);
                        TestTypeCourseCurriculumGrades.UpdateDate = DateTime.Now;
                    }
                }

                if(testType.TestTypeDeficiencies?.Any() ?? false)
                {
                    foreach(var testTypeDeficiency in testType.TestTypeDeficiencies)
                    {
                        testTypeDeficiency.State = Convert.ToByte(EnumState.excluido);
                        testTypeDeficiency.UpdateDate = DateTime.Now;
                    }
                }

                testType.State = Convert.ToByte(EnumState.excluido);
                testType.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(testType).State = System.Data.Entity.EntityState.Modified;
                await GestaoAvaliacaoContext.SaveChangesAsync();
            }
        }

        #endregion

        #region Custom methods

        public IEnumerable<TestType> Search(String search, ref Pager pager, Guid EntityId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(GestaoAvaliacaoContext.TestType.AsNoTracking()
                        .Where(x => x.Description.Contains(search)
                            && x.State == (Byte)EnumState.ativo
                            && x.EntityId == EntityId).OrderBy(x => x.Description));
                }
            }
        }

        public bool ExistsDescriptionNamed(string Description, long id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.TestType.AsNoTracking().FirstOrDefault(
                        x => x.Description.ToLower().Equals(Description.ToLower())
                            && (x.State == (Byte)EnumState.ativo)
                            && x.Id != id) != null;
                }
            }
        }

        public async Task<IEnumerable<TestType>> LoadNotGlobalAsync(Guid EntityId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return await gestaoAvaliacaoContext.TestType
                        .AsNoTracking()
                        .Where(x => x.State == (Byte)EnumState.ativo && x.EntityId == EntityId && !x.Global)
                        .OrderBy(x => x.Description)
                        .ToListAsync();
                }
            }
        }

        public async Task<IEnumerable<TestType>> LoadAllAsync(Guid EntityId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return await gestaoAvaliacaoContext.TestType
                        .AsNoTracking()
                        .Where(x => x.State == (Byte)EnumState.ativo && x.EntityId == EntityId)
                        .OrderBy(x => x.Description)
                        .ToListAsync();
                }
            }
        }

        public IEnumerable<TestType> GetByModelTest(long modelTestId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.TestType.AsNoTracking().
                        Where(x => x.State == (Byte)EnumState.ativo
                        && x.ModelTest_Id == modelTestId).ToList();
                }
            }
        }

        public bool ExistsTestAssociated(long Id)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(Id) ");
            sql.Append("FROM dbo.Test WITH (NOLOCK) ");
            sql.Append("WHERE State = @state AND TestType_Id = @id");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return (int)cn.ExecuteScalar(sql.ToString(), new { state = (Byte)EnumState.ativo, id = Id }) > 0;
            }
        }

        public TestType Get(long Id)
        {
            var sql = new StringBuilder("SELECT [Id],[Description],[FormatType_Id],[EntityId],[FrequencyApplication],[Bib],[Global], ");
            sql.Append("[TypeLevelEducationId],[ItemType_Id],[ModelTest_Id],[CreateDate],[UpdateDate],[State] ");
            sql.Append("FROM [TestType] WITH (NOLOCK) ");
            sql.Append("WHERE [Id] = @id AND [State] <> @state ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<TestType>(sql.ToString(), new { id = Id, state = (byte)EnumState.excluido }).FirstOrDefault();
            }
        }

        public IEnumerable<TestType> LoadFiltered(Guid EntityId, bool global)
        {
            var sql = new StringBuilder("SELECT [Id],[Description],[FormatType_Id],[AnswerSheet_Id],[EntityId],[FrequencyApplication],[Bib],[Global] ");
            sql.Append(",[TypeLevelEducationId],[ItemType_Id],[ModelTest_Id],[CreateDate],[UpdateDate],[State] ");
            sql.Append("FROM [TestType] WITH (NOLOCK) ");
            sql.Append("WHERE [EntityId] = @ent_id AND [State] <> @state AND [Global] = @global ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<TestType>(sql.ToString(), new { ent_id = EntityId, state = (byte)EnumState.excluido, global = global });
            }
        }

        #endregion
    }
}
