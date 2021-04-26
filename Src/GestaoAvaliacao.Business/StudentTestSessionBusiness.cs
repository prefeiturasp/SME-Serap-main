using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Entities.StudentTestAccoplishments;
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
        private readonly IStudentTestAccoplishmentRepository _studentTestAccoplishmentRepository;

        public StudentTestSessionBusiness(IStudentTestAbsenceReasonBusiness studentTestAbsenceReasonBusiness, IStudentTestAccoplishmentRepository studentTestAccoplishmentRepository)
        {
            _studentTestAbsenceReasonBusiness = studentTestAbsenceReasonBusiness;
            _studentTestAccoplishmentRepository = studentTestAccoplishmentRepository;
        }

        public async Task<List<StudentTestSessionDto>> GetStudentTestSession(long test_id, long tur_id)
        {
            var alunos = _studentTestAbsenceReasonBusiness.GetByTestSection(test_id, tur_id, null, false).ToList();
            var studentsSession = await _studentTestAccoplishmentRepository.GetAsync(tur_id, test_id);
            var studentsTestSession = new List<StudentTestSessionDto>();
            foreach (var aluno in alunos)
            {
                var sessions = GetSessionsStudent(aluno, studentsSession);
                var tempoTotalEmTicks = sessions.Sum(s => s.TempoTotal.Ticks);
                var tempoTotal = new TimeSpan(tempoTotalEmTicks);
                var studentSession = new StudentTestSessionDto
                {
                    NumeroDaChamada = aluno.mtu_numeroChamada.ToString(),
                    NomeDoAluno = aluno.alu_nome,
                    TempoTotalDaSessao = tempoTotal.ToString(@"hh\:mm\:ss"),
                    Session = sessions,
                };
                studentsTestSession.Add(studentSession);
            }

            return studentsTestSession;
        }

        private List<TestSessionDto> GetSessionsStudent(CorrectionStudentGrid student, List<StudentTestAccoplishment> studentsSession)
        {
            var sessionsOfStudent = studentsSession.Where(o => o.AluId == student.alu_id).FirstOrDefault();
            var testsSession = new List<TestSessionDto>();
            if (sessionsOfStudent == null) return testsSession;
            var situacoes = new List<Sessionituation>() { Sessionituation.Done, Sessionituation.DoneByLostConnection };
            foreach (var session in sessionsOfStudent.Sessions.Where(s => situacoes.Contains(s.Situation)))
            {
                var tempoTotal = session.EndDate.HasValue ? session.EndDate.Value.Subtract(session.StartDate) : TimeSpan.Zero;
                testsSession.Add(new TestSessionDto
                {
                    DataEHoraInicial = session.StartDate.ToString("dd/MM/yyyy HH:mm:ss"),
                    DataEHoraFinal = session.EndDate?.ToString("dd/MM/yyyy HH:mm:ss"),
                    TempoTotalFormatado = tempoTotal.ToString(@"hh\:mm\:ss"),
                    TempoTotal = tempoTotal,
                });
            }

            return testsSession;
        }
    }
}
