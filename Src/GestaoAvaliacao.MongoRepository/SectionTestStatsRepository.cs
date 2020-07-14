using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.MongoRepository
{
    public class SectionTestStatsRepository : BaseRepository<SectionTestStats>, ISectionTestStatsRepository
	{
		public async Task<SectionTestStats> GetByTest(long test_id, long tur_id)
		{
			var filter1 = Builders<SectionTestStats>.Filter.Eq("Test_Id", test_id);
			var filter2 = Builders<SectionTestStats>.Filter.Eq("tur_id", tur_id);
			var filter = Builders<SectionTestStats>.Filter.And(filter1, filter2);

			return await FindOne(filter) ?? new SectionTestStats();
		}

        public async Task<List<SectionTestStats>> GetByTest(long test_id)
		{
			var filter = Builders<SectionTestStats>.Filter.Eq("Test_Id", test_id);

			return await base.Find(filter);
		}

		public async Task<List<SectionTestStatsGroupDTO>> GetGrouped(long test_id, IEnumerable<long> turmas)
		{
			var filter1 = Builders<SectionTestStats>.Filter.Eq("Test_Id", test_id);
			var filter2 = Builders<SectionTestStats>.Filter.In("tur_id", turmas);
			var and = Builders<SectionTestStats>.Filter.And(filter1, filter2);

			var aggregate = base.Collection.Aggregate()
				.Match(and)
				.Unwind<SectionTestStats, SectionTestStatsDTO>(i => i.Answers)
				.Group(new BsonDocument() { { "_id", "$Answers.Item_Id" },
					 { "Grade", new BsonDocument("$sum", "$Answers.Grade") },
					{ "Count", new BsonDocument("$sum", 1) }});

			var result = await aggregate.ToListAsync();

			var retorno = new List<SectionTestStatsGroupDTO>();

			foreach (var item in result)
				retorno.Add(BsonSerializer.Deserialize<SectionTestStatsGroupDTO>(item));

			return retorno;
		}
    }
}
