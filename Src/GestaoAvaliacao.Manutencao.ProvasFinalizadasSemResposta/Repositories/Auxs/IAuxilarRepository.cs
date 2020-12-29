using GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.Auxs.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.Auxs
{
    internal interface IAuxilarRepository
    {
        Task<IEnumerable<TestDto>> GetTestsById(IEnumerable<long> ids);
        Task<IEnumerable<StudentDto>> GetStudentsById(IEnumerable<long> ids);
        Task<IEnumerable<SchoolBySectionDto>> GetSchoolsBySectionAsync(IEnumerable<long> ids);
    }
}