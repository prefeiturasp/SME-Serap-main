using GestaoAvaliacao.MongoEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.StudentCorrections
{
    internal interface IAjusteStudentCorrectionRepository
    {
        Task<List<StudentCorrection>> GetCorrections(long alunoId, long tur_id, DateTime dataDaMatricula);

        Task InsertManyAsync(List<StudentCorrection> entity);

        Task<bool> Delete(StudentCorrection entity);
    }
}