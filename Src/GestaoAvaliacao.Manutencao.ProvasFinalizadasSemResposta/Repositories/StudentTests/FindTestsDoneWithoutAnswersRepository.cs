using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoRepository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.StudentTests
{
    internal class FindTestsDoneWithoutAnswersRepository : BaseRepository<StudentCorrection>, IFindTestsDoneWithoutAnswersRepository
    {
        private readonly int _limit = 30;
        private DateTime createDateStart = new DateTime(2020, 11, 23);

        public Task<List<StudentCorrection>> GetNoAnswersTestsAsync(DateTime updateDateStart) 
            => Collection.Find(x => 
            x.provaFinalizada == true 
            && x.CreateDate >= createDateStart
            && x.UpdateDate >= updateDateStart 
            && !x.Answers.Any())?.ToListAsync();

        public Task<List<StudentCorrection>> GetTestsAnswersEmptyAsync(DateTime updateDateStart, int skip = 0)
        {
            var filter3 = Builders<StudentCorrection>.Filter.Eq("provaFinalizada", true);
            var filter1 = Builders<StudentCorrection>.Filter.Gte("CreateDate", createDateStart);
            var filter2 = Builders<StudentCorrection>.Filter.Gte("UpdateDate", updateDateStart);
            var filter4 = Builders<StudentCorrection>.Filter.Eq("Answers.Empty", true);
            var filter = Builders<StudentCorrection>.Filter.And(filter1, filter2, filter3, filter4);
            var query = Collection.Find(filter);
            return query.Skip(skip*_limit).Limit(_limit).ToListAsync();
        }
    }
}