using GestaoAvaliacao.MongoEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.StudentTests
{
    internal interface IFindTestsDoneWithoutAnswersRepository
    {
        Task<List<StudentCorrection>> GetNoAnswersTestsAsync(DateTime updateDateStart);
        Task<List<StudentCorrection>> GetTestsAnswersEmptyAsync(DateTime updateDateStart, int skip = 0);
        Task<StudentCorrection> Replace(StudentCorrection studentTest);
    }
}