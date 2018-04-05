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
    public class BookletRepository : ConnectionReadOnly, IBookletRepository
	{

		public IEnumerable<Booklet> GetAllByTest(long testId)
		{
			var sql = new StringBuilder(@"SELECT b.Id, b.[Order], b.CreateDate, b.UpdateDate, ");
			sql.Append("t.Id, t.TestSituation, t.KnowledgeAreaBlock ");
			sql.Append("FROM Booklet b  INNER JOIN Test t on b.Test_Id = t.Id ");
			sql.Append("WHERE Test_Id = @test_Id AND b.State = @state");

			var sqlBlock = new StringBuilder(@"SELECT bk.Id, bk.Description ");
			sqlBlock.Append("FROM Block bk INNER JOIN Booklet b on bk.Booklet_Id = b.Id ");
			sqlBlock.Append("WHERE b.State = @state AND b.Id = @bookletId ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var booklets = cn.Query<Booklet, Test, Booklet>(sql.ToString(),
					(b, t) =>
					{
						b.Test = t;
						return b;
					},
					new
					{
						test_Id = testId,
						state = (Byte)EnumState.ativo
					});

				if (booklets.Count() > 0)
				{
					foreach (var booklet in booklets)
					{
						var blockList = cn.Query<Block>(sqlBlock.ToString(),
							new
							{
								state = (Byte)EnumState.ativo,
								bookletId = booklet.Id
							});
						foreach (Block block in blockList)
						{
							booklet.Blocks.Add(block);
						}
					}

				}
				return booklets;
			}
		}

		public Booklet GetTestBooklet(long booklet_Id)
		{
			var sql = new StringBuilder(@"SELECT b.Id, t.Id, t.Description, t.FrequencyApplication, t.ApplicationStartDate, t.KnowledgeAreaBlock, tt.Id, tt.FrequencyApplication, tt.ModelTest_Id, it.Id, it.QuantityAlternative ");
			sql.AppendLine("FROM Booklet b WITH (NOLOCK) ");
			sql.AppendLine("INNER JOIN Test t WITH (NOLOCK) ON b.Test_Id = t.Id ");
			sql.AppendLine("INNER JOIN TestType tt WITH (NOLOCK) ON t.TestType_Id = tt.Id ");
			sql.AppendLine("INNER JOIN ItemType it WITH (NOLOCK) ON tt.ItemType_Id = it.Id ");
			sql.AppendLine("WHERE b.Id = @bookletId AND b.State = @state ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var booklet = cn.Query<Booklet, Test, TestType, ItemType, Booklet>(sql.ToString(),
					(b, t, tt, it) =>
					{
						tt.ItemType = it;
						t.TestType = tt;
						b.Test = t;
						return b;
					},
					new
					{
						bookletId = booklet_Id,
						state = (Byte)EnumState.ativo
					});
				return booklet.FirstOrDefault();
			}
		}

		public Booklet GetBookletByTest(long test_Id)
		{
			var sql = new StringBuilder(@"SELECT b.Id, b.Test_Id, ");
			sql.AppendLine("t.Id,t.Description,t.Bib,t.NumberItemsBlock,t.NumberBlock,t.NumberItem,t.ApplicationStartDate,t.ApplicationEndDate,t.CorrectionStartDate, ");
			sql.AppendLine("t.CorrectionEndDate,t.UsuId,t.CreateDate,t.UpdateDate,t.State,t.Discipline_Id,t.TestType_Id,t.FrequencyApplication,t.TestSituation,t.FormatType_Id,t.AllAdhered, t.KnowledgeAreaBlock, ");
			sql.AppendLine("tt.Id, tt.FrequencyApplication, tt.ModelTest_Id, tt.ItemType_Id, ");
			sql.AppendLine("it.Id, it.QuantityAlternative ");
			sql.AppendLine("FROM Booklet b WITH (NOLOCK) ");
			sql.AppendLine("INNER JOIN Test t WITH (NOLOCK) ON b.Test_Id = t.Id ");
			sql.AppendLine("INNER JOIN TestType tt WITH (NOLOCK) ON t.TestType_Id = tt.Id ");
			sql.AppendLine("INNER JOIN ItemType it WITH (NOLOCK) ON tt.ItemType_Id = it.Id ");
			sql.AppendLine("WHERE t.Id = @test_Id AND t.State = @state ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var booklet = cn.Query<Booklet, Test, TestType, ItemType, Booklet>(sql.ToString(),
				   (b, t, tt, it) =>
				   {
					   tt.ItemType = it;
					   t.TestType = tt;
					   b.Test = t;
					   return b;
				   },
					new
					{
						test_Id = test_Id,
						state = (Byte)EnumState.ativo
					});
				return booklet.FirstOrDefault();
			}
		}

		/**
         * Método para substituir o conteudo de GetBookletByTest e GetTestBooklet
         * */
		public Booklet GetBooklet(long id, bool param)
		{
			var sql = new StringBuilder(@"SELECT b.Id, ");
			sql.AppendLine("t.Id, t.Description, t.KnowledgeAreaBlock, ");
			sql.AppendLine("tt.Id, tt.ModelTest_Id ");
			sql.AppendLine("it.Id, it.QuantityAlternative ");
			sql.AppendLine("FROM Booklet b WITH (NOLOCK) ");
			sql.AppendLine("INNER JOIN Test t WITH (NOLOCK) ON b.Test_Id = t.Id ");
			sql.AppendLine("INNER JOIN TestType tt WITH (NOLOCK) ON t.TestType_Id = tt.Id ");
			sql.AppendLine("INNER JOIN ItemType it WITH (NOLOCK) ON tt.ItemType_Id = it.Id ");
			sql.AppendLine(param ? "WHERE b.Id = @id " : "WHERE t.Id = @id ");
			sql.AppendLine("AND b.State = @state ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var booklet = cn.Query<Booklet, Test, TestType, ItemType, Booklet>(sql.ToString(),
					(b, t, tt, it) =>
					{
						tt.ItemType = it;
						t.TestType = tt;
						b.Test = t;
						return b;
					},
					new
					{
						id = id,
						state = (Byte)EnumState.ativo
					});
				return booklet.FirstOrDefault();
			}
		}
	}
}
