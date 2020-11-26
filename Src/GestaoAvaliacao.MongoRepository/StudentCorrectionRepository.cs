using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.MongoRepository
{
    public class StudentCorrectionRepository : BaseRepository<StudentCorrection>, IStudentCorrectionRepository
    {
        public async Task<List<StudentCorrection>> GetByTest(long test_id, long tur_id)
        {
            var filter1 = Builders<StudentCorrection>.Filter.Eq("Test_Id", test_id);
            var filter2 = Builders<StudentCorrection>.Filter.Eq("tur_id", tur_id);
            var filter = Builders<StudentCorrection>.Filter.And(filter1, filter2);

            return await Find(filter) ?? new List<StudentCorrection>();
        }

        public async Task<List<StudentCorrection>> GetByTest(List<long> testId)
        {
            var filter1 = Builders<StudentCorrection>.Filter.In("Test_Id", testId);
            var filter = Builders<StudentCorrection>.Filter.And(filter1);

            var count = await base.Count(filter);

            if (count == 0)
                return new List<StudentCorrection>();
            else
                return await base.Find(filter);
        }

        public async Task<long> CountInconsistency(long test_id, long tur_id)
        {
            var filter1 = Builders<StudentCorrection>.Filter.Eq("Test_Id", test_id);
            var filter2 = Builders<StudentCorrection>.Filter.Eq("tur_id", tur_id);
            var filter3 = Builders<StudentCorrection>.Filter.Eq("Answers.Empty", true);
            var filter4 = Builders<StudentCorrection>.Filter.Eq("Answers.StrikeThrough", true);

            var filterOr = Builders<StudentCorrection>.Filter.Or(filter3, filter4);
            var filter = Builders<StudentCorrection>.Filter.And(filter1, filter2, filterOr);

            return await base.Count(filter);
        }

        public Task<StudentCorrection> GetStudentCorrectionByTestAluId(long test_Id, long alu_id, long tur_id) 
            => DataBase
                .GetCollection<StudentCorrection>("StudentCorrection")
                .Aggregate()
                .Match(new BsonDocument { { "Test_Id", test_Id }, { "tur_id", tur_id }, { "alu_id", alu_id } })
                .Project(new BsonDocument {
                    { "_id", 0 },
                    { "Test_Id", "$_id.Test_Id" },
                    { "alu_id", "$_id.alu_id" },
                    { "tur_id", "$_id.tur_id" },
                    { "provaFinalizada", "$provaFinalizada" },
                    { "Answers", "$_id.Answers" },
                })
                .As<StudentCorrection>()
                .FirstOrDefaultAsync();
    }
}