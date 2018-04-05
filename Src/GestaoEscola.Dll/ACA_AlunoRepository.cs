using Dapper;
using GestaoEscolar.Entities;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoEscolar.Repository
{
    public class ACA_AlunoRepository : ConnectionReadOnly, IACA_AlunoRepository
	{
		public IEnumerable<ACA_Aluno> GetBySection(long tur_id)
		{
			var sql = new StringBuilder("SELECT alu.alu_id, alu.alu_nome, mtu_id, mtu_numeroChamada ");
			sql.Append("FROM ACA_Aluno (NOLOCK) alu ");
			sql.Append("INNER JOIN MTR_MatriculaTurma (NOLOCK) mtu ON alu.alu_id = mtu.alu_id ");
			sql.Append("WHERE tur_id = @tur_id AND alu.alu_situacao = @state AND mtu.mtu_situacao = @state ");
			sql.Append("ORDER BY alu.alu_nome ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = cn.Query<ACA_Aluno, MTR_MatriculaTurma, ACA_Aluno>(sql.ToString(), 
					(a, m)=>
					{
						a.MatriculaTurma = m;
						return a;
					},
					new { tur_id = tur_id, state = (byte)1 }, splitOn: "mtu_id");

				return result;
			}
		}

        public ACA_Aluno Get(long alu_id)
        {
            var sql = new StringBuilder(@"SELECT [alu_id]
                                              ,[alu_nome]
                                              ,[ent_id]
                                              ,[alu_matricula]
                                              ,[alu_dataCriacao]
                                              ,[alu_dataAlteracao]
                                              ,[alu_situacao]
                                              ,[MatriculaTurma_alu_id]
                                              ,[MatriculaTurma_mtu_id]");
            sql.Append("FROM ACA_Aluno (NOLOCK) ");
            sql.Append("WHERE alu_id = @alu_id ");


            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<ACA_Aluno>(sql.ToString(), new { alu_id = alu_id }).FirstOrDefault();
            }
        }

        public ACA_Aluno GetStudentByPesId(Guid pes_id)
        {
            var sql = new StringBuilder(@"SELECT [alu_id] ");
            sql.Append("FROM ACA_Aluno (NOLOCK) ");
            sql.Append("WHERE pes_id = @pes_id ");


            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<ACA_Aluno>(sql.ToString(), new { pes_id = pes_id }).FirstOrDefault();
            }
        }

        public IEnumerable<ACA_Aluno> Get(IEnumerable<long> alu_ids)
        {
            var sql = new StringBuilder(@"SELECT [alu_id]
                                              ,[alu_nome]
                                              ,[ent_id]
                                              ,[alu_matricula]
                                              ,[alu_situacao]");
            sql.Append("FROM ACA_Aluno (NOLOCK) ");
            sql.AppendFormat("WHERE alu_id IN ({0}) ", string.Join(",", alu_ids));


            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<ACA_Aluno>(sql.ToString());
            }
        }
    }
}
