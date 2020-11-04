using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Entities.Enumerator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Entities.StudentTestAccoplishments
{
    public class StudentTestAccoplishment : EntityBase
    {
        public long AluId { get; private set; }
        public long TurId { get; private set; }
        public long Test_Id { get; private set; }
        public Test Test { get; private set; }
        public AccoplishmentoSituation Situation { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public ICollection<StudentTestSession> Sessions { get; private set; }

        public StudentTestAccoplishment(long aluId, long turId, Test test)
        {
            SetAluId(aluId);
            SetTurId(turId);
            SetTest(test);
            StartDate = DateTime.Now;
            Situation = AccoplishmentoSituation.Started;
            Sessions = new List<StudentTestSession>();
        }

        protected StudentTestAccoplishment()
        {
        }

        private void SetAluId(long aluId)
        {
            if (aluId <= 0)
            {
                Validate.SetErrorMessage("A identificação do aluno deve ser informada.");
                return;
            }

            AluId = aluId;
        }

        private void SetTurId(long turId)
        {
            if (turId <= 0)
            {
                Validate.SetErrorMessage("A identificação da turma do aluno deve ser informada.");
                return;
            }

            TurId = turId;
        }

        public void SetTest(Test test)
        {
            if (test is null || test.Id <= 0)
            {
                Validate.SetErrorMessage("A prova em andamento deve ser informada.");
                return;
            }

            Test = test;
            Test_Id = test.Id;
        }

        public StudentTestSession CreateSession(Guid connectionId)
        {
            var session = new StudentTestSession(this, connectionId);
            if (!session.Validate.IsValid)
            {
                Validate.SetErrorMessage(session.Validate.Message);
                return null;
            }

            Sessions.Add(session);
            return session;
        }

        public void End(DateTime? endDate = null)
        {
            EndDate = endDate ?? DateTime.Now;
            Situation = Sessions.Any(x => x.EndDate is null) ? AccoplishmentoSituation.DoneWithIncompleteSessions : AccoplishmentoSituation.Done;
        }
    }
}