using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
    public class RequestRevokeRepository : ConnectionReadOnly, IRequestRevokeRepository
    {
        public IEnumerable<long> GetRevokedItemsByTest(long test_id)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT bi.Item_Id");
            sql.AppendLine("FROM RequestRevoke rr WITH (NOLOCK)");
            sql.AppendLine("INNER JOIN BlockItem bi WITH (NOLOCK) ON rr.BlockItem_Id = bi.Id AND bi.State = @state");
            sql.AppendLine("WHERE rr.Test_id = @Test_Id AND(rr.Situation = @RevokedTest OR rr.Situation = @Revoked) AND rr.State = @state");
            sql.AppendLine("UNION");
            sql.AppendLine("SELECT i.Id");
            sql.AppendLine("FROM Item i WITH (NOLOCK)");
            sql.AppendLine("INNER JOIN BlockItem bi WITH (NOLOCK) ON bi.Item_id = i.Id AND bi.State = @state");
            sql.AppendLine("INNER JOIN Block bl WITH (NOLOCK) ON bl.Id = bi.Block_Id AND bl.State = @state");
            sql.AppendLine("INNER JOIN Booklet bo WITH (NOLOCK) ON bo.Id = bl.Booklet_Id AND bo.State = @state");
            sql.AppendLine("WHERE i.Revoked = 1 AND bo.Test_Id = @Test_Id AND i.State = @state");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<long>(sql.ToString(),
                    new
                    {
                        state = (byte)EnumState.ativo,
                        Test_Id = test_id,
                        RevokedTest = EnumSituation.RevokedTest,
                        Revoked = EnumSituation.Revoked
                    });
            }
        }

        public IEnumerable<RequestRevokeDTO> GetRequestRevoke(int blockItem_Id)
        {
            var sql = @"SELECT  usu_email, pes_nome, rr.Situation, rr.Justification, rr.UpdateDate, rr.UsuId " +
                                   "FROM Synonym_Core_SYS_USUARIO usu WITH (NOLOCK) " +
                                   "LEFT JOIN Synonym_Core_PES_Pessoa pes WITH (NOLOCK) ON usu.pes_id = pes.pes_id " +
                                   "INNER JOIN RequestRevoke rr WITH (NOLOCK) ON rr.UsuId = usu.usu_id " +
                                   "WHERE usu.usu_id = rr.UsuId AND rr.BlockItem_Id = @BlockItem_Id";

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var listRequestRevoke = cn.Query<RequestRevokeDTO>(sql,
                        new { state = (Byte)EnumState.ativo, BlockItem_Id = blockItem_Id });

                return listRequestRevoke;
            }
        }

        public RequestRevoke UpdateBlockItemsRevoked(RequestRevoke requestRevoke)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                RequestRevoke requestRevoke1;
                if (requestRevoke.Id > 0)
                {
                    requestRevoke1 = gestaoAvaliacaoContext.RequestRevoke.FirstOrDefault(a => a.Id == requestRevoke.Id);
                    requestRevoke1.Situation = requestRevoke.Situation;
                    requestRevoke1.UpdateDate = DateTime.Now;
                    requestRevoke1.Justification = requestRevoke.Justification;
                    gestaoAvaliacaoContext.Entry(requestRevoke1).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    requestRevoke1 = requestRevoke;
                    requestRevoke1.State = Convert.ToByte(EnumState.ativo);
                    requestRevoke1.CreateDate = DateTime.Now;
                    gestaoAvaliacaoContext.RequestRevoke.Add(requestRevoke1);
                }

                gestaoAvaliacaoContext.SaveChanges();

                return requestRevoke1;
            }            
        }

        public List<RequestRevoke> UpdateRequestRevokedByTestBlockItem(long Test_Id, long BlockItem_Id, EnumSituation Situation)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                List<RequestRevoke> requetRevokeList = gestaoAvaliacaoContext.RequestRevoke
                    .Where(rr => rr.Test_Id == Test_Id && rr.BlockItem_Id == BlockItem_Id).ToList();
                requetRevokeList.Select(c => { c.Situation = Situation; return c; }).ToList();

                foreach (RequestRevoke requestRevoke in requetRevokeList)
                {
                    gestaoAvaliacaoContext.Entry(requestRevoke).State = System.Data.Entity.EntityState.Modified;
                }

                gestaoAvaliacaoContext.SaveChanges();

                return requetRevokeList;
            }            
        }
    }
}
