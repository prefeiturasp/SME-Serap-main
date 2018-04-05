using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.MongoRepository
{
    public class ResponseChangeLogMongoRepository : BaseRepository<ResponseChangeLog>, IResponseChangeLogMongoRepository
    {
        public List<ResponseChangeLog> GetResponseChangeLog(long test_id, Guid? uad_id, long? esc_id, long? tur_id)
        {           
            var filter = new BsonDocument();

            if (uad_id.HasValue)
                filter.Add("Dre_id", uad_id);
            if (esc_id.HasValue)
                filter.Add("Esc_id", esc_id);
            if (tur_id.HasValue)
                filter.Add("Tur_id", tur_id);

            var responseChangeLog = DataBase
                               .GetCollection<BsonDocument>("ResponseChangeLog")
                               .Aggregate()
                               .Match(new BsonDocument("Test_id", test_id))
                               .Match(filter)
                               .As<ResponseChangeLog>()
                               .ToListAsync();

            return responseChangeLog.Result;
        }
    }
}
