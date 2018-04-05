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
using System.Text;

namespace GestaoAvaliacao.Repository
{
    public class BaseTextRepository : ConnectionReadOnly, IBaseTextRepository
    {
        #region CRUD

        public BaseText Save(BaseText entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;
                entity.State = Convert.ToByte(EnumState.ativo);

                GestaoAvaliacaoContext.BaseText.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public void Update(BaseText entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                BaseText baseText = GestaoAvaliacaoContext.BaseText.FirstOrDefault(a => a.Id == entity.Id);

                if (!string.IsNullOrEmpty(entity.Description))
                    baseText.Description = entity.Description;

                if (!string.IsNullOrEmpty(entity.Source))
                    baseText.Source = entity.Source;

                baseText.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(baseText).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public BaseText Get(long id)
        {
            var sql = new StringBuilder(@"SELECT Id, Description, Source, InitialOrientation, BaseTextOrientation,NarrationInitialStatement, ");
            sql.Append("NarrationStudentBaseText, StudentBaseText ");
            sql.Append("FROM BaseText ");
            sql.Append("WHERE Id = @id AND State = @state");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var baseText = cn.Query<BaseText>(sql.ToString(), new { id = id, state = (Byte)EnumState.ativo }).FirstOrDefault();
                return baseText;
            }
        }

        public IEnumerable<BaseText> Load(ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.BaseText.Where(x => x.State == (Byte)EnumState.ativo).OrderBy(x => x.Description));
                }
            }
        }


        public void Delete(long id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                BaseText baseText = GestaoAvaliacaoContext.BaseText.FirstOrDefault(a => a.Id == id);

                baseText.State = Convert.ToByte(EnumState.excluido);
                baseText.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(baseText).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion
    }
}
