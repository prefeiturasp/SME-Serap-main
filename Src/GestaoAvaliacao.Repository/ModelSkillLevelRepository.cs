using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Repository
{
    public class ModelSkillLevelRepository : IModelSkillLevelRepository
    {
        public IEnumerable<ModelSkillLevel> GetByMatrixModel(long idMatrix)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query =
                        ctx.ModelSkillLevel.AsNoTracking().Where(
                            m => m.ModelEvaluationMatrix.Id == idMatrix && m.State != (Byte)EnumState.excluido).ToList();

                    return query;
                }
            }
        }

        #region CRUD

        public ModelSkillLevel Get(long id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.ModelSkillLevel.AsNoTracking().FirstOrDefault(bt => bt.Id == id);
                }
            }
        }

        public IEnumerable<ModelSkillLevel> Load(long modelEvatuationMatrixId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.ModelSkillLevel.Include("ModelEvaluationMatrix").AsNoTracking().Where(x => x.ModelEvaluationMatrix.Id == modelEvatuationMatrixId && x.State != (Byte)EnumState.excluido && x.ModelEvaluationMatrix.State != (Byte)EnumState.excluido).OrderBy(x => x.Level).ToList();
                }
            }
        }

        #endregion

        #region Custom Methods

        public ModelSkillLevel GetByLevel(int level, long modelEvatuationMatrixId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.ModelSkillLevel.AsNoTracking().FirstOrDefault(x => x.ModelEvaluationMatrix.Id == modelEvatuationMatrixId && x.Level == level && x.State != (Byte)EnumState.excluido && x.ModelEvaluationMatrix.State != (Byte)EnumState.excluido);
                }
            }
        }

        public IEnumerable<ModelSkillLevel> GetById(long ModelSkillLevelId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.ModelSkillLevel.Include("ModelEvaluationMatrix").AsNoTracking().Where(x => x.Id == ModelSkillLevelId && x.State != (Byte)EnumState.excluido && x.ModelEvaluationMatrix.State != (Byte)EnumState.excluido).ToList();
                }
            }
        }
        #endregion
    }
}
