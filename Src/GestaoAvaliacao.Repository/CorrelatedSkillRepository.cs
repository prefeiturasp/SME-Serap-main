using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
    public class CorrelatedSkillRepository : ConnectionReadOnly, ICorrelatedSkillRepository
    {
        #region CRUD

        public CorrelatedSkill Save(CorrelatedSkill entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                entity.Skill1 = GestaoAvaliacaoContext.Skill.FirstOrDefault(a => a.Id == entity.Skill1.Id);
                entity.Skill2 = GestaoAvaliacaoContext.Skill.FirstOrDefault(a => a.Id == entity.Skill2.Id);
                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;
                entity.State = Convert.ToByte(EnumState.ativo);

                GestaoAvaliacaoContext.CorrelatedSkill.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public CorrelatedSkill Get(long CorrelatedSkill_Id)
        {
            var sql = new StringBuilder(@"SELECT id, CreateDate, UpdateDate, State, Skill1_Id, Skill2_Id ");
            sql.Append("FROM CorrelatedSkill ");
            sql.Append("WHERE id = @id AND state = @state");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var correlatedSkill = cn.Query<CorrelatedSkill>(sql.ToString(),
                    new
                    {
                        id = CorrelatedSkill_Id,
                        state = (Byte)EnumState.ativo
                    });
                return correlatedSkill.FirstOrDefault();
            }
        }

        public void Delete(long id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                CorrelatedSkill correlatedSkill = GestaoAvaliacaoContext.CorrelatedSkill.FirstOrDefault(a => a.Id == id);

                correlatedSkill.State = Convert.ToByte(EnumState.excluido);
                correlatedSkill.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(correlatedSkill).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion

        #region Custom methods

        public List<CorrelatedSkillByEvaluationMatrix> LoadList(long MatrizId, ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    SqlParameter matrizId = new SqlParameter("@matrizId", MatrizId);
                    matrizId.SqlDbType = SqlDbType.Int;

                    SqlParameter pageSizeParameter = new SqlParameter("@pageSize", pager.PageSize);
                    pageSizeParameter.SqlDbType = SqlDbType.Int;

                    SqlParameter pageNumberParameter = new SqlParameter("@pageNumber", pager.CurrentPage);
                    pageNumberParameter.SqlDbType = SqlDbType.Int;

                    SqlParameter totalRecordsParameter = new SqlParameter("@totalRecords", 0) { Direction = ParameterDirection.Output };
                    totalRecordsParameter.SqlDbType = SqlDbType.Int;

                    var myEntities =
                        GestaoAvaliacaoContext.Database.SqlQuery<CorrelatedSkillByEvaluationMatrix>(
                            "EXEC MS_CorrelatedSkill_SELECTBY_EvaluationMatrixId @matrizId, @pageSize, @pageNumber, @totalRecords OUT",
                            matrizId, pageSizeParameter, pageNumberParameter, totalRecordsParameter).ToList
                            <CorrelatedSkillByEvaluationMatrix>();

                    pager.SetTotalPages((int)Math.Ceiling(Convert.ToInt32(totalRecordsParameter.Value) / (double)pager.PageSize));
                    pager.SetTotalItens(Convert.ToInt32(totalRecordsParameter.Value));
                    return myEntities;
                }

            }
        }

        public List<CorrelatedSkillByEvaluationMatrix> _LoadList(long MatrizId, ref Pager pager)
        {
            var p = new DynamicParameters();

            p.Add("@matrizId", MatrizId);
            p.Add("@pageSize", pager.PageSize);
            p.Add("@pageNumber", pager.CurrentPage);
            p.Add("@totalRecords", pager.RecordsCount);

            using (IDbConnection cn = Connection)
            {
                var query = cn.QueryMultiple("MS_CorrelatedSkill_SELECTBY_EvaluationMatrixId", param: p, commandType: CommandType.StoredProcedure);
                var ret = query.Read<CorrelatedSkillByEvaluationMatrix>();

                return ret.ToList();
            }
        }

        public List<Skill> LoadCorrelatedSkills(long skillId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    List<Skill> skills = new List<Skill>();

                    skills.AddRange(GestaoAvaliacaoContext.CorrelatedSkill.Include("Skill").AsNoTracking().Where(x => x.Skill1.Id == skillId && x.Skill2.LastLevel && x.State == (Byte)EnumState.ativo).Select(x => x.Skill2).ToList());
                    skills.AddRange(GestaoAvaliacaoContext.CorrelatedSkill.Include("Skill").AsNoTracking().Where(x => x.Skill2.Id == skillId && x.Skill1.LastLevel && x.State == (Byte)EnumState.ativo).Select(x => x.Skill1).ToList());

                    return skills;
                }
            }
        }

        public bool ExistsCorrelated(CorrelatedSkill entity)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.CorrelatedSkill.AsNoTracking().Where(
                        x => (x.Skill1.Id == entity.Skill1.Id && x.Skill2.Id == entity.Skill2.Id && (x.State == (Byte)EnumState.ativo))
                            || (x.Skill1.Id == entity.Skill2.Id && x.Skill2.Id == entity.Skill1.Id && (x.State == (Byte)EnumState.ativo))
                            ).Count() > 0;
                }
            }
        }

        #endregion
    }
}
