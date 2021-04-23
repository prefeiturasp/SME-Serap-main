using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class StudentTestSessionBusiness : IStudentTestSessionBusiness
    {
        private readonly IStudentTestAbsenceReasonBusiness _studentTestAbsenceReasonBusiness;

        public StudentTestSessionBusiness(IStudentTestAbsenceReasonBusiness studentTestAbsenceReasonBusiness)
        {
            _studentTestAbsenceReasonBusiness = studentTestAbsenceReasonBusiness;
        }

        public async Task GetStudentTestSession(long test_id, long tur_id) 
        {
            var alunos = _studentTestAbsenceReasonBusiness.GetByTestSection(test_id, tur_id, null,false).ToList();

            foreach (var aluno in alunos)
            {

            }
        }
    }
}
