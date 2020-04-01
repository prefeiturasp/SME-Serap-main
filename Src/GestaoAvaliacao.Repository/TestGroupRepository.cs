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

namespace GestaoAvaliacao.Repository
{
    public class TestGroupRepository : ConnectionReadOnly, ITestGroupRepository
    {
        #region ReadOnly

        public TestGroup Get(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT Id, Description " +
                            "FROM TestGroup " +
                            "WHERE Id = @id ";

                var testGroup = cn.Query<TestGroup>(sql, new { id = id }).FirstOrDefault();

                return testGroup;
            }

        }

        public IEnumerable<TestGroup> LoadPaginate(ref Pager pager, Guid ent_id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.TestGroup.AsNoTracking().Include("TestSubGroups").Where(
                        x => x.State != (Byte)EnumState.excluido && x.EntityId == ent_id && x.TestSubGroups.Any(m => m.State == (Byte)EnumState.ativo)).OrderBy(x => x.Description).AsQueryable());
                }
            }
        }

        public IEnumerable<TestGroup> Search(ref Pager pager, Guid EntityId, String search = null, int levelqtd = 0)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    if (search != null && levelqtd != 0)
                    {
                        return pager.Paginate(gestaoAvaliacaoContext.TestGroup.AsNoTracking().Include("TestSubGroups").Where(
                                a => a.Description.Contains(search)
                                && a.State != (Byte)EnumState.excluido
                                && a.EntityId == EntityId
                                && a.TestSubGroups.Where(x => x.State == (Byte)EnumState.ativo).ToList().Count == levelqtd).OrderBy(x => x.Description));
                    }
                    else if (search != null)
                    {
                        return pager.Paginate(gestaoAvaliacaoContext.TestGroup.AsNoTracking().Include("TestSubGroups").Where(
                                a => a.Description.Contains(search)
                                && a.State != (Byte)EnumState.excluido
                                && a.EntityId == EntityId).OrderBy(x => x.Description));
                    }
                    else
                    {
                        return pager.Paginate(gestaoAvaliacaoContext.TestGroup.AsNoTracking().Include("TestSubGroups").Where(
                                a => a.TestSubGroups.Where(x => x.State == (Byte)EnumState.ativo).ToList().Count == levelqtd
                                && a.State != (Byte)EnumState.excluido
                                && a.EntityId == EntityId).OrderBy(x => x.Description));
                    }
                }
            }
        }

        public IEnumerable<TestGroup> Load(Guid ent_id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT Id, Description " +
                           "FROM TestGroup " +
                           "WHERE EntityId = @entityid " +
                           "AND State = @state " +
                           "ORDER BY Description ";

                var testGroup = cn.Query<TestGroup>(sql, new { entityid = ent_id, state = (Byte)EnumState.ativo });

                return testGroup;
            }
        }

        public TestGroup GetTestGroup(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT Id, Description, State " +
                           "FROM TestGroup " +
                           "WHERE id = @id " +
                           "AND state != @state " +
                           "SELECT Id, Description " +
                           "FROM TestSubGroup " +
                           "WHERE TestGroup_Id = @id " +
                           "AND state != @state ";

                var multi = cn.QueryMultiple(sql, new { id = id, state = (Byte)EnumState.excluido });

                var listTestGroup = multi.Read<TestGroup>();
                var listTestSubGroup = multi.Read<TestSubGroup>();

                var testGroup = listTestGroup.FirstOrDefault();

                foreach (var testSubGroup in listTestSubGroup)
                {
                    testGroup.TestSubGroups.Add(testSubGroup);
                }

                return testGroup;
            }
        }

        public IEnumerable<TestGroup> LoadGroupsSubGroups(Guid ent_id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT SGRP.Id, GRP.Description + ' - ' + SGRP.Description AS Description " +
                           "FROM TestGroup AS GRP WITH (NOLOCK) " +
                           "INNER JOIN TestSubGroup AS SGRP WITH (NOLOCK) " +
                           "ON GRP.Id = SGRP.TestGroup_Id " +
                           "WHERE EntityId = @entityid AND GRP.State = @state AND SGRP.State = @state " +
                           "ORDER BY GRP.Description, SGRP.Description ";

                var testGroup = cn.Query<TestGroup>(sql, new { entityid = ent_id, state = (Byte)EnumState.ativo });

                return testGroup;
            }
        }
         
        public bool ExistsTestRelated(TestGroup entity)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT COUNT(t.Id) " +
                           "FROM Test t WITH (NOLOCK) " +
                           "INNER JOIN TestSubGroup sg WITH (NOLOCK)" +
                           "ON t.TestSubGroup_Id = sg.Id " +
                           "WHERE sg.TestGroup_Id = @id " +
                           "AND t.State != @state ";

                var count = (int)cn.ExecuteScalar(sql, new { id = entity.Id, state = (Byte)EnumState.excluido });

                return count > 0;
            }
        }


        public bool ExistsModelDescription(string description, Guid ent_id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT COUNT(Id) " +
                           "FROM TestGroup " +
                           "WHERE Description COLLATE Latin1_General_CI_AS = @Description " +
                           "AND EntityId = @entityid " +
                           "AND State != @state ";

                var count = (int)cn.ExecuteScalar(sql, new { Description = description, entityid = ent_id, state = (Byte)EnumState.excluido });

                return count > 0;
            }
        }

        public bool ExistsModelDescriptionUpdate(string description, long Id, Guid ent_id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT COUNT(Id) " +
                           "FROM TestGroup " +
                           "WHERE Description COLLATE Latin1_General_CI_AS = @Description " +
                           "AND id != @id " +
                           "AND EntityId = @entityid " +
                           "AND State != @state ";

                var count = (int)cn.ExecuteScalar(sql, new { Description = description, id = Id, entityid = ent_id, state = (Byte)EnumState.excluido });

                return count > 0;
            }
        }

        public bool VerifyDeleteSubGroup(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT COUNT(Id) " +
                           "FROM Test WITH (NOLOCK) " +                           
                           "WHERE TestSubGroup_Id = @id " +
                           "AND State != @state ";

                var count = (int)cn.ExecuteScalar(sql, new { id = id, state = (Byte)EnumState.excluido });

                return count == 0;
            }
        }

        #endregion

        #region CRUD

        public TestGroup Save(TestGroup entity)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                TestGroup _entity = new TestGroup
                {
                    Description = entity.Description,
                    EntityId = entity.EntityId,
                    State = entity.State
                };

                foreach (var modelskill in entity.TestSubGroups)
                {
                    modelskill.State = (Byte)EnumState.ativo;
                    modelskill.CreateDate = dateNow;
                    modelskill.UpdateDate = dateNow;
                }

                var _allEntitys = gestaoAvaliacaoContext.TestGroup.Include("TestSubGroups").Select(p => p.TestSubGroups);
                int order = _allEntitys.Max(p => p.Max(q => q.Order)) + 1;

                entity.TestSubGroups.Select(c => { c.Order = order;  order++; return c; }).ToList();

                _entity.TestSubGroups = new List<TestSubGroup>();
                _entity.TestSubGroups.AddRange(entity.TestSubGroups);
                
                gestaoAvaliacaoContext.TestGroup.Add(_entity);
                gestaoAvaliacaoContext.SaveChanges();

                return _entity;
            }
        }

        public TestGroup Update(TestGroup entity)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                var _entity = gestaoAvaliacaoContext.TestGroup.Include("TestSubGroups").Where(x => x.Id == entity.Id && x.TestSubGroups.Any(m => m.State == (Byte)EnumState.ativo)).FirstOrDefault();

                gestaoAvaliacaoContext.Entry(_entity).CurrentValues.SetValues(entity);

                _entity.UpdateDate = dateNow;

                foreach (var existingChild in _entity.TestSubGroups.ToList())
                {
                    if (!entity.TestSubGroups.Any(c => c.Id == existingChild.Id))
                    {
                        existingChild.State = (Byte)EnumState.excluido;
                        existingChild.UpdateDate = dateNow;
                        gestaoAvaliacaoContext.Entry(existingChild);
                    }
                }
                List<TestSubGroup> newTestSubGroup = new List<TestSubGroup>();

                var _allEntitys = gestaoAvaliacaoContext.TestGroup.Include("TestSubGroups").Select(p => p.TestSubGroups);
                int order = _allEntitys.Max(p => p.Max(q => q.Order)) + 1;

                foreach (var childModel in entity.TestSubGroups)
                {
                    var existingChild = _entity.TestSubGroups.SingleOrDefault(c => c.Id == childModel.Id);

                    if (existingChild != null)
                    {
                        var item = _entity.TestSubGroups.FirstOrDefault(p => p.Id == childModel.Id);
                        if (item != null)
                        {
                            childModel.Order = item.Order;
                        }
                        existingChild.UpdateDate = dateNow;
                        gestaoAvaliacaoContext.Entry(existingChild).CurrentValues.SetValues(childModel);
                    }
                    else
                    {
                        var newChild = new TestSubGroup
                        {
                            Description = childModel.Description,
                            State = (Byte)EnumState.ativo,
                            CreateDate = dateNow,
                            UpdateDate = dateNow,
                            Order = order
                        };
                        newTestSubGroup.Add(newChild);

                        order++;
                    }
                }
                _entity.TestSubGroups.AddRange(newTestSubGroup);
                gestaoAvaliacaoContext.SaveChanges();

                return _entity;
            }

        }

        public void Delete(TestGroup entity)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;
                TestGroup _entity = gestaoAvaliacaoContext.TestGroup.FirstOrDefault(x => x.Id == entity.Id);

                _entity.State = Convert.ToByte(EnumState.excluido);
                _entity.UpdateDate = dateNow;

                foreach (TestSubGroup item in _entity.TestSubGroups)
                {
                    item.State = Convert.ToByte(EnumState.excluido);
                    item.UpdateDate = dateNow;
                }

                gestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion

        #region Custom Methods

        public bool IsDeletedModelSkillBeenUsed(TestGroup entity)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    var _entity = GestaoAvaliacaoContext.TestGroup.Include("TestSubGroups").Where(x => x.Id == entity.Id && x.TestSubGroups.Any(m => m.State == (Byte)EnumState.ativo)).FirstOrDefault();
                    foreach (var existingChild in _entity.TestSubGroups.ToList())
                    {
                        if (!entity.TestSubGroups.Any(c => c.Id == existingChild.Id))
                        {
                            return GestaoAvaliacaoContext.Skill.FirstOrDefault(x => x.ModelSkillLevel.Id == existingChild.Id) != null;
                        }
                    }
                    return false;
                }
            }
        }

        #endregion
    }
}
