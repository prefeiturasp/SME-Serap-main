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
    public class ModelEvaluationMatrixRepository : ConnectionReadOnly, IModelEvaluationMatrixRepository
    {
        #region ReadOnly

        public ModelEvaluationMatrix Get(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT Id, Description " +
                            "FROM ModelEvaluationMatrix " +
                            "WHERE Id = @id ";

                var modelEvaluationMatrix = cn.Query<ModelEvaluationMatrix>(sql, new { id = id }).FirstOrDefault();

                return modelEvaluationMatrix;
            }

        }
        public IEnumerable<ModelEvaluationMatrix> LoadPaginate(ref Pager pager, Guid ent_id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.ModelEvaluationMatrix.AsNoTracking().Include("ModelSkillLevels").Where(
                        x => x.State != (Byte)EnumState.excluido && x.EntityId == ent_id && x.ModelSkillLevels.Any(m => m.State == (Byte)EnumState.ativo)).OrderBy(x => x.Description).AsQueryable());
                }
            }
        }

        public IEnumerable<ModelEvaluationMatrix> Search(ref Pager pager, Guid EntityId, String search = null, int levelqtd = 0)
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
                        return pager.Paginate(gestaoAvaliacaoContext.ModelEvaluationMatrix.AsNoTracking().Include("ModelSkillLevels").Where(
                                a => a.Description.Contains(search)
                                && a.State != (Byte)EnumState.excluido
                                && a.EntityId == EntityId
                                && a.ModelSkillLevels.Where(x => x.State == (Byte)EnumState.ativo).ToList().Count == levelqtd).OrderBy(x => x.Description));
                    }
                    else if (search != null)
                    {
                        return pager.Paginate(gestaoAvaliacaoContext.ModelEvaluationMatrix.AsNoTracking().Include("ModelSkillLevels").Where(
                                a => a.Description.Contains(search)
                                && a.State != (Byte)EnumState.excluido
                                && a.EntityId == EntityId).OrderBy(x => x.Description));
                    }
                    else
                    {
                        return pager.Paginate(gestaoAvaliacaoContext.ModelEvaluationMatrix.AsNoTracking().Include("ModelSkillLevels").Where(
                                a => a.ModelSkillLevels.Where(x => x.State == (Byte)EnumState.ativo).ToList().Count == levelqtd
                                && a.State != (Byte)EnumState.excluido
                                && a.EntityId == EntityId).OrderBy(x => x.Description));
                    }
                }
            }
        }

        public IEnumerable<ModelEvaluationMatrix> Load(Guid ent_id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT Id, Description " +
                           "FROM ModelEvaluationMatrix " +
                           "WHERE EntityId = @entityid " +
                           "AND State = @state " +
                           "ORDER BY Description ";

                var modelEvaluationMatrix = cn.Query<ModelEvaluationMatrix>(sql, new { entityid = ent_id, state = (Byte)EnumState.ativo });

                return modelEvaluationMatrix;
            }
        }

        public ModelEvaluationMatrix GetModelEvaluationMatrix(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT Id, Description, State " +
                           "FROM ModelEvaluationMatrix " +
                           "WHERE id = @id " +
                           "AND state != @state " +
                           "SELECT Id, Description, Level, LastLevel " +
                           "FROM ModelSkillLevel " +
                           "WHERE ModelEvaluationMatrix_Id = @id " +
                           "AND state != @state ";

                var multi = cn.QueryMultiple(sql, new { id = id, state = (Byte)EnumState.excluido });

                var listModelEvaluationMatrix = multi.Read<ModelEvaluationMatrix>();
                var listModelSkillLevel = multi.Read<ModelSkillLevel>();

                var modelEvaluationMatrix = listModelEvaluationMatrix.FirstOrDefault();

                foreach (var modelSkillLevel in listModelSkillLevel)
                {
                    modelEvaluationMatrix.ModelSkillLevels.Add(modelSkillLevel);
                }

                return modelEvaluationMatrix;
            }
        }

        public bool ExistsModelDescription(string description, Guid ent_id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT COUNT(Id) " +
                           "FROM ModelEvaluationMatrix " +
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
                           "FROM ModelEvaluationMatrix " +
                           "WHERE Description COLLATE Latin1_General_CI_AS = @Description " +
                           "AND id != @id " +
                           "AND EntityId = @entityid " +
                           "AND State != @state ";

                var count = (int)cn.ExecuteScalar(sql, new { Description = description, id = Id, entityid = ent_id, state = (Byte)EnumState.excluido });

                return count > 0;
            }
        }

        public bool ExistsMatrixRelated(ModelEvaluationMatrix entity)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT COUNT(e.Id) " +
                           "FROM EvaluationMatrix e " +
                           "INNER JOIN ModelEvaluationMatrix m ON e.ModelEvaluationMatrix_Id = m.id " +
                           "WHERE m.Id = @id " +
                           "AND e.State != @state ";

                var count = (int)cn.ExecuteScalar(sql, new { id = entity.Id, state = (Byte)EnumState.excluido });

                return count > 0;
            }
        }
        #endregion

        #region CRUD

        public ModelEvaluationMatrix Save(ModelEvaluationMatrix entity)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                ModelEvaluationMatrix _entity = new ModelEvaluationMatrix
                {
                    Description = entity.Description,
                    EntityId = entity.EntityId,
                    State = entity.State
                };

                foreach (var modelskill in entity.ModelSkillLevels)
                {
                    modelskill.State = (Byte)EnumState.ativo;
                    modelskill.CreateDate = dateNow;
                    modelskill.UpdateDate = dateNow;
                }

                _entity.ModelSkillLevels = new List<ModelSkillLevel>();
                _entity.ModelSkillLevels.AddRange(entity.ModelSkillLevels);



                gestaoAvaliacaoContext.ModelEvaluationMatrix.Add(_entity);
                gestaoAvaliacaoContext.SaveChanges();

                return _entity;
            }            
        }

        public ModelEvaluationMatrix Update(ModelEvaluationMatrix entity)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                var _entity = gestaoAvaliacaoContext.ModelEvaluationMatrix.Include("ModelSkillLevels").Where(x => x.Id == entity.Id && x.ModelSkillLevels.Any(m => m.State == (Byte)EnumState.ativo)).FirstOrDefault();

                gestaoAvaliacaoContext.Entry(_entity).CurrentValues.SetValues(entity);

                _entity.UpdateDate = dateNow;

                foreach (var existingChild in _entity.ModelSkillLevels.ToList())
                {
                    if (!entity.ModelSkillLevels.Any(c => c.Id == existingChild.Id))
                    {
                        existingChild.State = (Byte)EnumState.excluido;
                        existingChild.UpdateDate = dateNow;
                        gestaoAvaliacaoContext.Entry(existingChild);
                    }
                }
                List<ModelSkillLevel> newSkills = new List<ModelSkillLevel>();

                foreach (var childModel in entity.ModelSkillLevels)
                {
                    var existingChild = _entity.ModelSkillLevels.SingleOrDefault(c => c.Id == childModel.Id);

                    if (existingChild != null)
                    {
                        existingChild.UpdateDate = dateNow;
                        gestaoAvaliacaoContext.Entry(existingChild).CurrentValues.SetValues(childModel);
                    }
                    else
                    {
                        var newChild = new ModelSkillLevel
                        {
                            Description = childModel.Description,
                            Level = childModel.Level,
                            LastLevel = childModel.LastLevel,
                            State = (Byte)EnumState.ativo,
                            CreateDate = dateNow,
                            UpdateDate = dateNow
                        };
                        newSkills.Add(newChild);
                    }
                }
                _entity.ModelSkillLevels.AddRange(newSkills);
                gestaoAvaliacaoContext.SaveChanges();

                return _entity;
            }            

        }

        public void Delete(ModelEvaluationMatrix entity)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;
                ModelEvaluationMatrix _entity = gestaoAvaliacaoContext.ModelEvaluationMatrix.FirstOrDefault(x => x.Id == entity.Id);

                _entity.State = Convert.ToByte(EnumState.excluido);
                _entity.UpdateDate = dateNow;

                foreach (ModelSkillLevel item in _entity.ModelSkillLevels)
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

        public bool IsDeletedModelSkillBeenUsed(ModelEvaluationMatrix entity)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    var _entity = GestaoAvaliacaoContext.ModelEvaluationMatrix.Include("ModelSkillLevels").Where(x => x.Id == entity.Id && x.ModelSkillLevels.Any(m => m.State == (Byte)EnumState.ativo)).FirstOrDefault();
                    foreach (var existingChild in _entity.ModelSkillLevels.ToList())
                    {
                        if (!entity.ModelSkillLevels.Any(c => c.Id == existingChild.Id))
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
