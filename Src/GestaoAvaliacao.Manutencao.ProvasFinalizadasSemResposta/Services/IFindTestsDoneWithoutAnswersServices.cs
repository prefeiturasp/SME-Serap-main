using GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Services
{
    internal interface IFindTestsDoneWithoutAnswersServices
    {
        Task<IEnumerable<StudentWithTestsDoneWithoutAnswersDto>> FindStudentsWithNoAnswersAsync(DateTime updateDateStart);
        Task FindStudentsMissingTheLastAnswerAsync(DateTime updateDateStart);
    }
}