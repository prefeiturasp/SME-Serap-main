using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Entities.Enumerator;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class StudentTestSession : EntityBase
    {
        public long AluId { get; private set; }
        public long TurId { get; private set; }
        public long TestId { get; private set; }
        public Guid UsuId { get; private set; }
        public Guid ConnectionId { get; private set; }
        public Test Test { get; private set; }
        public Sessionituation Situation { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        [NotMapped]
        public TimeSpan Time => GetTime();

        public StudentTestSession(long aluId, long turId, Test test, Guid usuId, Guid connectionId)
            : base()
        {
            SetAluId(aluId);
            SetTurId(turId);
            SetTest(test);
            SetUsuId(usuId);
            SetConnectionId(connectionId);
            Situation = Sessionituation.NotStarted;
        }

        protected StudentTestSession()
        {
        }

        private void SetAluId(long aluId)
        {
            if (aluId <= 0)
            {
                Validate.SetErrorMessage("A identificação do aluno deve ser informada.");
                return;
            }

            AluId = Id;
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

        private void SetTest(Test test)
        {
            if (test is null || test.Id <= 0)
            {
                Validate.SetErrorMessage("A prova em andamento deve ser informada.");
                return;
            }

            Test = test;
            TestId = test.Id;
        }

        public void SetUsuId(Guid usuId)
        {
            if(usuId == Guid.Empty)
            {
                Validate.SetErrorMessage("A identificação do usuário logado deve ser informada.");
                return;
            }

            UsuId = usuId;
        }

        public void SetConnectionId(Guid connectionId)
        {
            if (connectionId == Guid.Empty)
            {
                Validate.SetErrorMessage("A identificação da conexão deve ser informada.");
                return;
            }

            ConnectionId = connectionId;
        }

        public void Start()
        {
            StartDate = DateTime.Now;
            Situation = Sessionituation.Started;
        }

        public void End(bool byLostConnection)
        {
            EndDate = DateTime.Now;
            Situation = byLostConnection ? Sessionituation.DoneByLostConnection : Sessionituation.Done;
        }

        private TimeSpan GetTime()
        {
            if (EndDate is null) return default;
            return EndDate.GetValueOrDefault().Subtract(StartDate);
        }
    }
}