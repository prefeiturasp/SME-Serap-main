using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Entities.Projections;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class AlternativeRepository : ConnectionReadOnly, IAlternativeRepository
    {
        #region CRUD

        public Alternative Save(Alternative entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;
                entity.State = Convert.ToByte(EnumState.ativo);

                GestaoAvaliacaoContext.Alternative.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public void Update(Alternative entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Alternative _entity = GestaoAvaliacaoContext.Alternative.Include("Item").FirstOrDefault(a => a.Id == entity.Id);

                if (!string.IsNullOrEmpty(entity.Description))
                    _entity.Description = entity.Description;

                _entity.Correct = entity.Correct;

                if (entity.Item != new Item())
                    _entity.Item = entity.Item;

                if (entity.Order > 0)
                    _entity.Order = entity.Order;

                _entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public Alternative Get(long id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.Alternative.FirstOrDefault(a => a.Id == id);
                }

            }
        }

        public IEnumerable<Alternative> GetAlternativesByItem(long item_id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.Alternative.Include("Item").Where(a => a.Item.Id == item_id);
                }

            }
        }

        public IEnumerable<Alternative> Load(ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.Alternative.Where(x => x.State == (Byte)EnumState.ativo).OrderBy(x => x.Description));
                }
            }
        }

        public void Delete(long id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Alternative _entity = GestaoAvaliacaoContext.Alternative.FirstOrDefault(a => a.Id == id);

                _entity.State = Convert.ToByte(EnumState.excluido);
                _entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion

        #region Read

        public async Task<IEnumerable<AlternativesWithNumerationAndOrderProjection>> GetAlternativesWithNumerationAndOrderByTest(long test_id)
        {
            var situations = new[] { EnumSituation.RevokedTest, EnumSituation.Revoked };

            using (IDbConnection cn = Connection)
            {
                var sql = @"SELECT a.Item_Id, (DENSE_RANK() OVER(ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS ItemOrder, a.Numeration ,a.Id AS Alternative_Id, a.[Order], a.Correct,
                            CASE WHEN RR.Id IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS ItemRevoked  
                            FROM [Block] b
                            INNER JOIN BlockItem bi on bi.Block_Id = b.Id
                            INNER JOIN Item i WITH (NOLOCK) ON i.Id = bi.Item_Id 
                            INNER JOIN Alternative a on a.Item_Id = bi.Item_Id
                            INNER JOIN Test T WITH(NOLOCK) ON T.Id = b.Test_Id 
                            LEFT JOIN RequestRevoke RR ON RR.BlockItem_Id = BI.Id AND RR.State = @state AND RR.Situation IN @Situations 
                            LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = i.KnowledgeArea_Id AND B.Id = Bka.Block_Id AND Bka.State = @state 
                            WHERE b.Test_Id = @Test_Id AND bi.State = @State
                            ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order], a.[Order] ";

                var result = cn.QueryAsync<AlternativesWithNumerationAndOrderProjection>(sql, new { Test_Id = test_id, State = (Byte)EnumState.ativo, Situations = situations });

                return await result;
            }
        }

        public List<Alternative> GetAlternativesByItens(IEnumerable<string> itens, long test_id)
        {
            var sql = new StringBuilder("SELECT  A.[Id], A.[Description], [Correct], A.[Order], [Justificative], [Numeration], REPLACE(Numeration, ')', '') AS NumerationSem, [TCTBiserialCoefficient], [TCTDificulty], [TCTDiscrimination], A.[Item_Id], BI.[Order] AS ItemOrder ");
            sql.Append("FROM Alternative AS A WITH(NOLOCK) ");
            sql.Append("INNER JOIN Item AS I WITH(NOLOCK) ON I.Id = A.Item_Id ");
            sql.Append("INNER JOIN BlockItem AS BI WITH(NOLOCK) ON BI.Item_Id = I.Id ");
            sql.Append("INNER JOIN Block AS BL WITH(NOLOCK) ON BL.Id = BI.Block_Id ");
            sql.Append("WHERE A.State <> @State ");
            sql.Append("AND I.State <> @State ");
            sql.Append("AND BI.State <> @State ");
            sql.Append("AND BL.State <> @State ");
            sql.Append("AND BL.Test_Id = @TestId ");
            sql.AppendFormat("AND A.Item_Id IN ({0}) ", string.Join(",", itens));
            sql.Append("ORDER BY BI.[Order], A.[Order] ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var alternatives = cn.Query<Alternative>(sql.ToString(), new { State = (Byte)EnumState.excluido, TestId = test_id });

                return alternatives.ToList();
            }
        }

        public async Task<IEnumerable<Alternative>> GetAlternativesByItensAsync(IEnumerable<string> itens, long test_id)
        {
            var sql = new StringBuilder("SELECT  A.[Id], A.[Description], [Correct], A.[Order], [Justificative], [Numeration], REPLACE(Numeration, ')', '') AS NumerationSem, [TCTBiserialCoefficient], [TCTDificulty], [TCTDiscrimination], A.[Item_Id], BI.[Order] AS ItemOrder ");
            sql.Append("FROM Alternative AS A WITH(NOLOCK) ");
            sql.Append("INNER JOIN Item AS I WITH(NOLOCK) ON I.Id = A.Item_Id ");
            sql.Append("INNER JOIN BlockItem AS BI WITH(NOLOCK) ON BI.Item_Id = I.Id ");
            sql.Append("INNER JOIN Block AS BL WITH(NOLOCK) ON BL.Id = BI.Block_Id ");
            sql.Append("WHERE A.State <> @State ");
            sql.Append("AND I.State <> @State ");
            sql.Append("AND BI.State <> @State ");
            sql.Append("AND BL.State <> @State ");
            sql.Append("AND BL.Test_Id = @TestId ");
            sql.AppendFormat("AND A.Item_Id IN ({0}) ", string.Join(",", itens));
            sql.Append("ORDER BY BI.[Order], A.[Order] ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return await cn.QueryAsync<Alternative>(sql.ToString(), new { State = (Byte)EnumState.excluido, TestId = test_id });
            }
        }

        #endregion Read
    }
}