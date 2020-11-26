using GestaoAvaliacao.Entities.StudentsTestSent;
using GestaoAvaliacao.Util;
using System;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness.StudentsTestSent
{
    public interface IStudentTestSentBusiness
    {
        Task<StudentTestSent> SaveAsync(long testId, long turId, long aluId, Guid entId, EnumSYS_Visao visao);
    }
}