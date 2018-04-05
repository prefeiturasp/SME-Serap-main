using Dapper;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoEscolar.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
	public class TestCurriculumGradeRepository : ConnectionReadOnly, ITestCurriculumGradeRepository
	{
		#region Read
		public IEnumerable<ACA_CurriculoPeriodo> GetSimple(long test_id, int esc_id)
		{
			var sql = new StringBuilder("SELECT DISTINCT tt.tcp_id, tt.tcp_ordem AS crp_ordem, tt.tcp_descricao AS crp_descricao ");
			sql.Append("FROM TestCurriculumGrade tcg ");
			sql.Append("INNER JOIN SGP_ACA_TipoCurriculoPeriodo tt ON tcg.TypeCurriculumGradeId = tt.tcp_id ");
			sql.Append("INNER JOIN Test t ON t.Id = tcg.Test_Id ");
			sql.Append("INNER JOIN TestTypeCourse ttc ON ttc.TestType_Id = t.TestType_Id ");
			sql.Append("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp ON ttcp.crp_ordem = tt.tcp_ordem AND tt.tme_id = ttcp.tme_id AND tt.tne_id = ttcp.tne_id AND esc_id = @esc_id ");
			sql.Append("WHERE t.Id = @Test_Id AND t.State = @state AND tcg.State = @state AND ttc.State = @state ");
			sql.Append("ORDER BY tt.tcp_descricao");

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var result = cn.Query<ACA_CurriculoPeriodo>(sql.ToString(), new { state = 1, Test_Id = test_id, esc_id = esc_id });

				return result;
			}

		}

		public ACA_CurriculoPeriodo GetTestCurriculumGradeCrpDescricao(long test_id)
		{
			var sql = new StringBuilder("DECLARE @anos VARCHAR(8000) ");
			sql.Append("SELECT @anos = COALESCE(@anos + ', ', '') + Tcp.tcp_descricao ");
			sql.Append("FROM TestCurriculumGrade AS Tcg WITH(NOLOCK) ");
			sql.Append("INNER JOIN SGP_ACA_TipoCurriculoPeriodo AS Tcp WITH(NOLOCK)ON Tcp.tcp_id = Tcg.TypeCurriculumGradeId ");
			sql.Append("WHERE Tcg.Test_Id = @Test_Id AND Tcg.State = @state AND Tcp.tcp_situacao = @state ");
			sql.Append("ORDER BY Tcp.tcp_descricao ");
			sql.Append("SELECT @anos AS crp_descricao ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var result = cn.Query<ACA_CurriculoPeriodo>(sql.ToString(), new { state = (Byte)EnumState.ativo, Test_Id = test_id }).FirstOrDefault();

				return result;
			}

		}

		public IEnumerable<ACA_TipoCurriculoPeriodo> GetDistinctCurricumGradeByTestSubGroup_Id(long TestSubGroup_Id)
		{
			var sql = new StringBuilder("SELECT DISTINCT TCP.tcp_id, TCP.tcp_descricao ");
			sql.Append("FROM TestCurriculumGrade AS TCG WITH(NOLOCK) ");
			sql.Append("INNER JOIN Test AS TES WITH(NOLOCK)ON TES.Id = TCG.Test_Id ");
			sql.Append("INNER JOIN SGP_ACA_TipoCurriculoPeriodo AS TCP WITH(NOLOCK)ON TCP.tcp_id = TCG.TypeCurriculumGradeId ");
			sql.Append("WHERE TES.TestSubGroup_Id = @TestSubGroup_Id AND TCG.State = @state AND TCP.tcp_situacao = @state ");
			sql.Append("ORDER BY TCP.tcp_descricao ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var result = cn.Query<ACA_TipoCurriculoPeriodo>(sql.ToString(), new { state = (Byte)EnumState.ativo, TestSubGroup_Id = TestSubGroup_Id });

				return result;
			}

		}
		public IEnumerable<ACA_TipoCurriculoPeriodo> GetCurricumGradeByTest_Id(long Test_Id)
		{
			var sql = new StringBuilder("SELECT DISTINCT TCP.tcp_id, TCP.tcp_descricao ");
			sql.Append("FROM TestCurriculumGrade AS TCG WITH(NOLOCK) ");
			sql.Append("INNER JOIN SGP_ACA_TipoCurriculoPeriodo AS TCP WITH(NOLOCK)ON TCP.tcp_id = TCG.TypeCurriculumGradeId ");
			sql.Append("WHERE TCG.Test_Id = @Test_Id AND TCG.State = @state AND TCP.tcp_situacao = @state ");
			sql.Append("ORDER BY TCP.tcp_descricao ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var result = cn.Query<ACA_TipoCurriculoPeriodo>(sql.ToString(), new { state = (Byte)EnumState.ativo, Test_Id = Test_Id });

				return result;
			}

		}

		#endregion
		#region Custom Methods
		public bool ExistsTestCurriculumGrade(int typeCurriculumGradeId, long testTypeId)
		{
			var transactionOptions = new System.Transactions.TransactionOptions
			{
				IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
			};

			using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
			{
				using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
				{
					return GestaoAvaliacaoContext.TestCurriculumGrade.AsNoTracking().FirstOrDefault(
						a => a.TypeCurriculumGradeId == typeCurriculumGradeId && a.Test.TestType.Id == testTypeId && a.State == (Byte)EnumState.ativo && a.Test.State == (Byte)EnumState.ativo && a.Test.TestType.State == (Byte)EnumState.ativo) != null;
				}
			}
		}

		public bool ExistsTestCourse(IEnumerable<long> TypeLevelEducationList, long testTypeId)
		{
			var transactionOptions = new System.Transactions.TransactionOptions
			{
				IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
			};

			using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
			{
				using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
				{
					return GestaoAvaliacaoContext.TestCurriculumGrade.AsNoTracking().Where(
						a => TypeLevelEducationList.Contains(a.TypeCurriculumGradeId) && a.Test.TestType.Id == testTypeId && a.State == (Byte)EnumState.ativo && a.Test.State == (Byte)EnumState.ativo && a.Test.TestType.State == (Byte)EnumState.ativo).ToList().Count > 0;
				}
			}
		}
		#endregion
	}
}
