using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.MongoRepository
{
    public class TempCorrectionResultRepository : BaseRepository<TempCorrectionResult>, ITempCorrectionResultRepository
    {
        public async Task<List<TempCorrectionResult>> _GetNotProcessed()
        {
            var filter1 = Builders<TempCorrectionResult>.Filter.Eq("Processed", false);
            var filter = Builders<TempCorrectionResult>.Filter.And(filter1);

            var count = await Count(filter);

            if (count == 0)
                return new List<TempCorrectionResult>();
            else
                return await Find(filter);
        }

        public async Task<long> _GetCount()
        {
            var filter = Builders<TempCorrectionResult>.Filter.Empty;
            return await Count(filter);
        }

        public async Task<TempCorrectionResult> GetByTestAndTeam(long test_id, long tur_id)
        {
            var filter1 = Builders<TempCorrectionResult>.Filter.In("Test_id", new List<long> { test_id });
            var filter2 = Builders<TempCorrectionResult>.Filter.In("Tur_id", new List<long> { tur_id });
            var filter = Builders<TempCorrectionResult>.Filter.And(filter1, filter2);

            var count = await base.Count(filter);

            if (count == 0)
                return new TempCorrectionResult();
            else
                return await base.FindOne(filter);
        }
    }
}
