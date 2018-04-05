using Dapper;
using GestaoAvaliacao.Entities;
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
    public class TestPermissionRepository : ConnectionReadOnly, ITestPermissionRepository
    {
        public IEnumerable<TestPermission> GetByTest(long Test_Id, Guid? gru_id)
        {
            var sql = @"SELECT 
                            ISNULL(TP.Id, 0) AS Id, ISNULL(TP.Test_Id, 0) AS Test_Id, Gru.gru_id, Gru.gru_nome, CAST(ISNULL(TP.AllowAnswer, 1) AS BIT) AS AllowAnswer, CAST(ISNULL(TP.ShowResult, 1) AS BIT) AS ShowResult, CAST(ISNULL(TP.TestHide, 0) AS BIT) AS TestHide, TP.CreateDate
                        FROM Synonym_Core_SYS_Grupo Gru WITH(NOLOCK)
	                        LEFT JOIN TestPermission TP WITH(NOLOCK)
		                        ON Gru.gru_id = TP.gru_id
		                        AND TP.[State] = 1
		                        AND TP.Test_Id = @test_id
                        WHERE 
                            Gru.sis_id = @sis_id 
	                        AND ((@gru_id IS NULL AND Gru.gru_situacao = 1 AND vis_id <> 1)
		                        OR (Gru.gru_id = @gru_id AND Gru.gru_situacao <> 3))
                        ORDER BY Gru.gru_nome";

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<TestPermission>(sql.ToString(), new { test_id = Test_Id, sis_id = Util.Constants.IdSistema, gru_id = gru_id, vis_id = Util.EnumSYS_Visao.Administracao });
            }
        }

        public bool Save(long test_id, List<TestPermission> permissions)
        {
            List<TestPermission> permissionsBD = GetByTest(test_id, null).ToList();

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                foreach (TestPermission entity in permissions)
                {
                    entity.Test_Id = test_id;
                    var permissionBD = permissionsBD.FirstOrDefault(p => p.Test_Id == test_id && p.gru_id == entity.gru_id);

                    if (permissionBD != null)
                    {
                        permissionBD.AllowAnswer = entity.AllowAnswer;
                        permissionBD.ShowResult = entity.ShowResult;
                        permissionBD.TestHide = entity.TestHide;

                        GestaoAvaliacaoContext.Entry(permissionBD).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        GestaoAvaliacaoContext.TestPermission.Add(entity);
                    }
                }

                GestaoAvaliacaoContext.SaveChanges();
            }

            return true;
        }
    }
}
