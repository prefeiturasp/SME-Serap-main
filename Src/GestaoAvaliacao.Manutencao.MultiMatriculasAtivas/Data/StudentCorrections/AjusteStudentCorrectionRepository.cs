using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoRepository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.StudentCorrections
{
    public class AjusteStudentCorrectionRepository : BaseRepository<StudentCorrection>, IAjusteStudentCorrectionRepository
    {
        public async Task<List<StudentCorrection>> GetCorrections(long alunoId, long tur_id, DateTime dataDaMatricula)
        {
            var filter1 = Builders<StudentCorrection>.Filter.Eq("alu_id", alunoId);
            var filter2 = Builders<StudentCorrection>.Filter.Eq("tur_id", tur_id);
            var filter3 = Builders<StudentCorrection>.Filter.Gte("UpdateDate", dataDaMatricula);
            var filter = Builders<StudentCorrection>.Filter.And(filter1, filter2, filter3);

            return await Find(filter) ?? new List<StudentCorrection>();
        }
    }
}