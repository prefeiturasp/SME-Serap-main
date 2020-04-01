using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
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
    public class StudentTestAbsenceReasonRepository : ConnectionReadOnly, IStudentTestAbsenceReasonRepository
	{
		#region Read

		public IEnumerable<CorrectionStudentGrid> GetByTestSection(long test_id, long tur_id, IEnumerable<long> aluMongoList, bool ignoreBlocked)
		{
            var sql = new StringBuilder();
			sql.Append("DECLARE @CorrectionStartDate DATE, @CorrectionEndDate DATE, @AllAdhered BIT ");
			sql.AppendLine("SELECT @CorrectionStartDate = CorrectionStartDate,  ");
            sql.AppendLine("@CorrectionEndDate = CorrectionEndDate, ");
            sql.AppendLine("@AllAdhered = AllAdhered ");
            sql.AppendLine("FROM Test WITH (NOLOCK) ");
            sql.AppendLine("WHERE Id = @test_id ");

            sql.AppendLine("SELECT DISTINCT alu.alu_id, alu.alu_nome, mtu.tur_id, stu.AbsenceReason_Id, mtu.mtu_numeroChamada, Esc.esc_id, Esc.uad_idSuperiorGestao AS dre_id, mtu.cur_id, mtu.crr_id, mtu.crp_id, CASE WHEN (Adr.TypeSelection = @TypeSelectionBlocked) THEN 1 ELSE 0 END AS blocked ");
            sql.AppendLine("FROM SGP_ACA_Aluno alu WITH (NOLOCK) ");
            sql.AppendLine("INNER JOIN SGP_MTR_MatriculaTurma mtu WITH (NOLOCK) ON alu.alu_id = mtu.alu_id ");
            sql.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ON mtu.tur_id = ttcp.tur_id ");
            sql.AppendLine("INNER JOIN SGP_ACA_TipoModalidadeEnsino tme WITH (NOLOCK) ON ttcp.tme_id = tme.tme_id ");
            sql.AppendLine("INNER JOIN SGP_ESC_Escola Esc WITH (NOLOCK) ON Esc.esc_id = mtu.esc_id ");
            sql.AppendLine("LEFT JOIN StudentTestAbsenceReason stu WITH (NOLOCK) ON stu.alu_id = alu.alu_id AND stu.Test_Id = @test_id AND mtu.tur_id = stu.tur_id ");
            sql.AppendLine("LEFT JOIN Adherence Adr WITH (NOLOCK) ON Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @state AND Adr.Test_Id = @test_id ");
            sql.AppendLine("WHERE mtu.tur_id = @tur_id AND alu.alu_situacao = @state  ");
            sql.AppendLine("AND mtu.mtu_situacao <> @stateExcluido ");
			sql.AppendLine("AND ((tme.tme_nome NOT LIKE '%EJA%' AND (mtu.mtu_dataMatricula IS NULL OR (mtu.mtu_dataMatricula <= @CorrectionEndDate AND (mtu.mtu_dataSaida IS NULL OR mtu.mtu_dataSaida >= @CorrectionStartDate)))) OR tme.tme_nome LIKE '%EJA%')");
            sql.AppendLine("AND ((@AllAdhered = 1 AND ISNULL(Adr.TypeSelection, 0) <> @TypeSelectionNotSelect) ");
            sql.AppendLine("OR (@AllAdhered = 0 AND (Adr.TypeSelection = @TypeSelectionSelected OR Adr.TypeSelection = @TypeSelectionBlocked))) ");

            if (aluMongoList != null && aluMongoList.Count() > 0)
            {
                sql.AppendLine("UNION ");
                sql.AppendLine("SELECT alu.alu_id, alu.alu_nome, mtu.tur_id, stu.AbsenceReason_Id, mtu.mtu_numeroChamada, Esc.esc_id, Esc.uad_idSuperiorGestao AS dre_id, mtu.cur_id, mtu.crr_id, mtu.crp_id, CASE WHEN (Adr.TypeSelection = @TypeSelectionBlocked) THEN 1 ELSE 0 END AS blocked  ");
                sql.AppendLine("FROM SGP_ACA_Aluno alu WITH (NOLOCK) ");
                sql.AppendLine("INNER JOIN SGP_MTR_MatriculaTurma mtu WITH (NOLOCK) ON alu.alu_id = mtu.alu_id ");
                sql.AppendLine("INNER JOIN SGP_ESC_Escola Esc WITH (NOLOCK) ON Esc.esc_id = mtu.esc_id ");
                sql.AppendLine("LEFT JOIN StudentTestAbsenceReason stu WITH (NOLOCK) ON stu.alu_id = alu.alu_id AND stu.Test_Id = @test_id AND mtu.tur_id = stu.tur_id ");
                sql.AppendLine("LEFT JOIN Adherence Adr WITH (NOLOCK) ON Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @state AND Adr.Test_Id = @test_id ");
                sql.AppendLine("WHERE mtu.tur_id = @tur_id  ");
                sql.AppendLine(string.Format("AND alu.alu_id IN ({0}) ", string.Join(",", aluMongoList)));
                sql.AppendLine("AND ((@AllAdhered = 1 AND ISNULL(Adr.TypeSelection, 0) <> @TypeSelectionNotSelect) ");
                sql.AppendLine("OR (@AllAdhered = 0 AND (Adr.TypeSelection = @TypeSelectionSelected OR Adr.TypeSelection = @TypeSelectionBlocked))) ");
            }
            sql.AppendLine("ORDER BY mtu.mtu_numeroChamada ");

            using (IDbConnection cn = Connection)
			{
				cn.Open();

                var result = cn.Query<CorrectionStudentGrid>(sql.ToString(), 
                    new {
                        test_id = test_id,
                        tur_id = tur_id,
                        state = (byte)EnumState.ativo,
                        stateExcluido = (byte)EnumState.excluido,
                        TypeEntityStudent = (byte)EnumAdherenceEntity.Student,
                        TypeSelectionSelected = (byte)EnumAdherenceSelection.Selected,
                        TypeSelectionNotSelect = (byte)EnumAdherenceSelection.NotSelected,
                        TypeSelectionBlocked = (byte)EnumAdherenceSelection.Blocked
                    });

                if (ignoreBlocked && result.Count() > 0)
                {
                    result = result.Where(p => !p.blocked);
                }

				return result;
			}
		}

        public IEnumerable<CorrectionStudentGrid> GetByTestSectionByAluId(long test_id, long tur_id, long alu_id, bool ignoreBlocked)
        {
            var sql = new StringBuilder();
            sql.Append("DECLARE @CorrectionStartDate DATE, @CorrectionEndDate DATE, @AllAdhered BIT ");
            sql.AppendLine("SELECT @CorrectionStartDate = CorrectionStartDate,  ");
            sql.AppendLine("@CorrectionEndDate = CorrectionEndDate, ");
            sql.AppendLine("@AllAdhered = AllAdhered ");
            sql.AppendLine("FROM Test WITH (NOLOCK) ");
            sql.AppendLine("WHERE Id = @test_id ");

            sql.AppendLine("SELECT DISTINCT alu.alu_id, alu.alu_nome, mtu.tur_id, stu.AbsenceReason_Id, mtu.mtu_numeroChamada, Esc.esc_id, Esc.uad_idSuperiorGestao AS dre_id, mtu.cur_id, mtu.crr_id, mtu.crp_id, CASE WHEN (Adr.TypeSelection = @TypeSelectionBlocked) THEN 1 ELSE 0 END AS blocked ");
            sql.AppendLine("FROM SGP_ACA_Aluno alu WITH (NOLOCK) ");
            sql.AppendLine("INNER JOIN SGP_MTR_MatriculaTurma mtu WITH (NOLOCK) ON alu.alu_id = mtu.alu_id ");
            sql.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ON mtu.tur_id = ttcp.tur_id ");
            sql.AppendLine("INNER JOIN SGP_ACA_TipoModalidadeEnsino tme WITH (NOLOCK) ON ttcp.tme_id = tme.tme_id ");
            sql.AppendLine("INNER JOIN SGP_ESC_Escola Esc WITH (NOLOCK) ON Esc.esc_id = mtu.esc_id ");
            sql.AppendLine("LEFT JOIN StudentTestAbsenceReason stu WITH (NOLOCK) ON stu.alu_id = alu.alu_id AND stu.Test_Id = @test_id AND mtu.tur_id = stu.tur_id ");
            sql.AppendLine("LEFT JOIN Adherence Adr WITH (NOLOCK) ON Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @state AND Adr.Test_Id = @test_id ");
            sql.AppendLine("WHERE mtu.tur_id = @tur_id AND alu.alu_situacao = @state  ");
            sql.AppendLine("AND mtu.mtu_situacao <> @stateExcluido ");
            sql.AppendLine("AND (tme.tme_nome NOT LIKE '%EJA%' AND (mtu.mtu_dataMatricula IS NULL OR (mtu.mtu_dataMatricula <= @CorrectionEndDate AND (mtu.mtu_dataSaida IS NULL OR mtu.mtu_dataSaida >= @CorrectionStartDate))) OR tme.tme_nome LIKE '%EJA%')");
            sql.AppendLine("AND ((@AllAdhered = 1 AND ISNULL(Adr.TypeSelection, 0) <> @TypeSelectionNotSelect) ");
            sql.AppendLine("OR (@AllAdhered = 0 AND (Adr.TypeSelection = @TypeSelectionSelected OR Adr.TypeSelection = @TypeSelectionBlocked))) ");

            if (alu_id > 0)
            {
                sql.AppendLine("UNION ");
                sql.AppendLine("SELECT alu.alu_id, alu.alu_nome, mtu.tur_id, stu.AbsenceReason_Id, mtu.mtu_numeroChamada, Esc.esc_id, Esc.uad_idSuperiorGestao AS dre_id, mtu.cur_id, mtu.crr_id, mtu.crp_id, CASE WHEN (Adr.TypeSelection = @TypeSelectionBlocked) THEN 1 ELSE 0 END AS blocked  ");
                sql.AppendLine("FROM SGP_ACA_Aluno alu WITH (NOLOCK) ");
                sql.AppendLine("INNER JOIN SGP_MTR_MatriculaTurma mtu WITH (NOLOCK) ON alu.alu_id = mtu.alu_id ");
                sql.AppendLine("INNER JOIN SGP_ESC_Escola Esc WITH (NOLOCK) ON Esc.esc_id = mtu.esc_id ");
                sql.AppendLine("LEFT JOIN StudentTestAbsenceReason stu WITH (NOLOCK) ON stu.alu_id = alu.alu_id AND stu.Test_Id = @test_id AND mtu.tur_id = stu.tur_id ");
                sql.AppendLine("LEFT JOIN Adherence Adr WITH (NOLOCK) ON Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @state AND Adr.Test_Id = @test_id ");
                sql.AppendLine("WHERE mtu.tur_id = @tur_id  ");
                sql.AppendLine("AND alu.alu_id = @alu_id ");
                sql.AppendLine("AND ((@AllAdhered = 1 AND ISNULL(Adr.TypeSelection, 0) <> @TypeSelectionNotSelect) ");
                sql.AppendLine("OR (@AllAdhered = 0 AND (Adr.TypeSelection = @TypeSelectionSelected OR Adr.TypeSelection = @TypeSelectionBlocked))) ");
            }
            sql.AppendLine("ORDER BY mtu.mtu_numeroChamada ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = cn.Query<CorrectionStudentGrid>(sql.ToString(),
                    new
                    {
                        test_id = test_id,
                        tur_id = tur_id,
                        alu_id = alu_id,
                        state = (byte)EnumState.ativo,
                        stateExcluido = (byte)EnumState.excluido,
                        TypeEntityStudent = (byte)EnumAdherenceEntity.Student,
                        TypeSelectionSelected = (byte)EnumAdherenceSelection.Selected,
                        TypeSelectionNotSelect = (byte)EnumAdherenceSelection.NotSelected,
                        TypeSelectionBlocked = (byte)EnumAdherenceSelection.Blocked
                    });

                if (ignoreBlocked && result.Count() > 0)
                {
                    result = result.Where(p => !p.blocked);
                }

                return result;
            }
        }

        public StudentTestAbsenceReason GetByTestStudent(long test_id, long tur_id, long alu_id)
		{
			var sql = new StringBuilder("SELECT Id,alu_id,Test_Id,AbsenceReason_Id,CreateDate,UpdateDate,State ");
            sql.Append("FROM StudentTestAbsenceReason WITH (NOLOCK) ");
			sql.Append("WHERE alu_id = @alu_id AND tur_id = @tur_id AND Test_Id = @test_Id");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = cn.Query<StudentTestAbsenceReason>(sql.ToString(), new { tur_id = tur_id, alu_id = alu_id, state = (byte)1, test_id = test_id }).FirstOrDefault();

				return result;
			}
		}

		public IEnumerable<StudentTestAbsenceReason> GetAbsencesByTestSection(long test_id, long tur_id)
		{
			var sql = new StringBuilder("SELECT s.alu_id, ar.Id, ar.Description ");
            sql.Append("FROM StudentTestAbsenceReason s WITH (NOLOCK) ");
            sql.Append("INNER JOIN AbsenceReason ar WITH (NOLOCK) on s.AbsenceReason_Id = ar.Id ");
			sql.Append("WHERE tur_id = @tur_id AND Test_Id = @test_Id");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = cn.Query<StudentTestAbsenceReason, AbsenceReason, StudentTestAbsenceReason>(sql.ToString(), 
					(s, a) => {
						s.AbsenceReason = a;
						return s;
					},
					new { tur_id = tur_id, state = (byte)1, test_id = test_id });

				return result;
			}
		}

        public IEnumerable<StudentTestAbsenceReason> GetAbsencesByTest(long test_id)
        {
            var sql = new StringBuilder("SELECT s.alu_id, ar.Id, ar.Description ");
            sql.Append("FROM StudentTestAbsenceReason s WITH (NOLOCK) ");
            sql.Append("INNER JOIN AbsenceReason ar WITH (NOLOCK) on s.AbsenceReason_Id = ar.Id ");
            sql.Append("WHERE Test_Id = @test_Id");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = cn.Query<StudentTestAbsenceReason, AbsenceReason, StudentTestAbsenceReason>(sql.ToString(),
                    (s, a) => {
                        s.AbsenceReason = a;
                        return s;
                    },
                    new { state = (byte)1, test_id = test_id });

                return result;
            }
        }

        public int CountAbsencesByTestSection(long test_id, long tur_id)
		{
			var sql = new StringBuilder("SELECT count(Id) ");
            sql.Append("FROM StudentTestAbsenceReason WITH (NOLOCK) ");
			sql.Append("WHERE tur_id = @tur_id AND Test_Id = @test_Id");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = cn.ExecuteScalar<int>(sql.ToString(), new { tur_id = tur_id, state = (byte)1, test_id = test_id });

				return result;
			}
		}

		public IEnumerable<long> StudentAbsencesByTestSection(long test_id, long tur_id)
		{
			var sql = new StringBuilder("SELECT alu_id ");
			sql.Append("FROM StudentTestAbsenceReason WITH (NOLOCK) ");
			sql.Append("WHERE tur_id = @tur_id AND Test_Id = @test_Id");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var result = cn.Query<long>(sql.ToString(), new { tur_id = tur_id, state = (byte)1, test_id = test_id });

				return result;
			}
		}

        public SchoolDTO GetEscIdDreIdByTeam(long tur_id)
        {
            string key = string.Format("StudentTestAbsenceReasonRepository_GetEscIdDreIdByTeam_{0}", tur_id);

            SchoolDTO result = Cache.GetFromCache<SchoolDTO>(key);
            if (result == null)
            {
                var sql = new StringBuilder();
                sql.AppendLine("SELECT Esc.esc_id, Esc.uad_idSuperiorGestao AS dre_id");
                sql.AppendLine("FROM SGP_TUR_Turma Tur WITH (NOLOCK) ");
                sql.AppendLine("INNER JOIN SGP_ESC_Escola Esc WITH (NOLOCK) ON Esc.esc_id = Tur.esc_id ");
                sql.Append("WHERE tur_id = @tur_id");

                using (IDbConnection cn = Connection)
                {
                    cn.Open();

                    result = cn.Query<SchoolDTO>(sql.ToString(), new { tur_id = tur_id }).FirstOrDefault();
                }

                Cache.SetInCache(key, result, 6);
            }

            return result;
        }

        #endregion

        public StudentTestAbsenceReason Save(StudentTestAbsenceReason entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				DateTime dateNow = DateTime.Now;

				entity.CreateDate = dateNow;
				entity.UpdateDate = dateNow;
				entity.State = Convert.ToByte(EnumState.ativo);

				GestaoAvaliacaoContext.StudentTestAbsenceReason.Add(entity);
				GestaoAvaliacaoContext.SaveChanges();

				return entity;
			}
		}

		public StudentTestAbsenceReason Update(StudentTestAbsenceReason entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				StudentTestAbsenceReason absenceReason = GestaoAvaliacaoContext.StudentTestAbsenceReason.FirstOrDefault(a => a.Id == entity.Id);

				absenceReason.AbsenceReason_Id = entity.AbsenceReason_Id;
				absenceReason.UpdateDate = DateTime.Now;

				GestaoAvaliacaoContext.Entry(absenceReason).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();

				return entity;
			}
		}

		public StudentTestAbsenceReason Remove(StudentTestAbsenceReason entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				StudentTestAbsenceReason absenceReason = GestaoAvaliacaoContext.StudentTestAbsenceReason.FirstOrDefault(a => a.Id == entity.Id);

				GestaoAvaliacaoContext.Entry(absenceReason).State = System.Data.Entity.EntityState.Deleted;
				GestaoAvaliacaoContext.SaveChanges();

				return entity;
			}
		}
	}
}
