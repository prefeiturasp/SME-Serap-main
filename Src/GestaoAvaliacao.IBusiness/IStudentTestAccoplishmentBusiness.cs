using GestaoAvaliacao.Dtos.StudentTestAccoplishments;
using GestaoAvaliacao.Entities.DTO.StudentTestAccoplishments;
using System;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IStudentTestAccoplishmentBusiness
    {
        Task StartSessionAsync(StartStudentTestSessionDto dto);

        Task EndSessionAsync(EndStudentTestSessionDto dto);

        Task EndStudentTestAccoplishmentAsync(EndStudentTestAccoplishmentDto dto);

        Task<StudentTestTimeResultDto> GetStudenteResultAsync(Guid pesId);
    }
}