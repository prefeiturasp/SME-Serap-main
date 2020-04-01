using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class ResponseChangeLogRepository : ConnectionReadOnly, IResponseChangeLogRepository
    {
        public List<StudentDTO> GetInfoStudents(List<long> alunos)
        {
            var sql = new StringBuilder("SELECT alu_id, alu_nome ");
            sql.Append("FROM dbo.SGP_ACA_Aluno WITH (NOLOCK) ");
            sql.AppendFormat("WHERE alu_id IN ({0}) ", string.Join(",", alunos.ToArray()));

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<StudentDTO>(sql.ToString()).ToList();
            }
        }

        public List<TeamsDTO> GetInfoTeams(List<long> turmas)
        {
            var sql = new StringBuilder("SELECT tur_id, tur_codigo ");
            sql.Append("FROM dbo.SGP_TUR_Turma WITH (NOLOCK) ");
            sql.AppendFormat("WHERE tur_id IN ({0}) ", string.Join(",", turmas.ToArray()));

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<TeamsDTO>(sql.ToString()).ToList();
            }
        }

        public List<BlockItem> GetBlockItens(List<long> itens, long Test_id)
        {
            var sql = new StringBuilder("SELECT Item_Id, [Order] ");
            sql.Append("FROM dbo.BlockItem AS BLI WITH (NOLOCK) ");
            sql.Append("INNER JOIN dbo.Block AS BL WITH (NOLOCK) ");
            sql.Append("ON BL.Id = BLI.Block_Id ");
            sql.AppendFormat("WHERE Item_Id IN ({0}) ", string.Join(",", itens.ToArray()));
            sql.Append("AND BL.Test_Id = @test_id ");

            var p = new DynamicParameters();
            p.Add("@test_id", Test_id);

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<BlockItem>(sql.ToString(), p).ToList();
            }
        }

        public List<DresDTO> GetInfoDresSchools(IEnumerable<string> dres, Guid ent_id)
        {
            var sql = new StringBuilder("SELECT Uad.uad_id AS dre_id, Uad.uad_nome AS dre_nome, Esc.esc_id, Esc.esc_nome ");
            sql.Append("FROM SGP_SYS_UnidadeAdministrativa Uad WITH (NOLOCK) ");
            sql.Append("INNER JOIN SGP_ESC_Escola Esc WITH (NOLOCK) ");
            sql.AppendLine("ON Uad.uad_id = Esc.uad_idSuperiorGestao ");
            sql.AppendLine("WHERE Uad.ent_id = @ent_id ");
            sql.AppendFormat("AND Uad.uad_id IN ({0}) ", string.Join(",", dres.ToArray()));

            var p = new DynamicParameters();
            p.Add("@ent_id", ent_id);

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<DresDTO>(sql.ToString(), p).ToList();
            }
        }

        public List<UsersDTO> GetInfoUsers(IEnumerable<string> usuarios, Guid ent_id)
        {
            var sql = new StringBuilder("SELECT usu_id, usu_login, PES.pes_id, pes_nome ");
            sql.Append("FROM dbo.Synonym_Core_SYS_Usuario AS USU WITH (NOLOCK) ");
            sql.Append("LEFT JOIN dbo.Synonym_Core_PES_Pessoa AS PES WITH (NOLOCK) ");
            sql.AppendLine("ON PES.pes_id = USU.pes_id ");
            sql.AppendLine("WHERE ent_id = @ent_id ");
            sql.AppendFormat("AND usu_id IN ({0}) ", string.Join(",", usuarios.ToArray()));

            var p = new DynamicParameters();
            p.Add("@ent_id", ent_id);

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<UsersDTO>(sql.ToString(), p).ToList();
            }
        }
    }
}
