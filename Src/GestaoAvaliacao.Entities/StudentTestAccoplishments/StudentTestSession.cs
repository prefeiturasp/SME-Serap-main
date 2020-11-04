using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Entities.Enumerator;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities.StudentTestAccoplishments
{
    public class StudentTestSession : EntityBase
    {
        public Guid ConnectionId { get; private set; }
        public long StudentTestAccoplishment_Id { get; private set; }
        public StudentTestAccoplishment StudentTestAccoplishment { get; private set; }
        public Sessionituation Situation { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        [NotMapped]
        public TimeSpan Time => GetTime();

        internal StudentTestSession(StudentTestAccoplishment studentTestAccoplishment, Guid connectionId)
            : base()
        {
            SetStudentTestAccoplishment(studentTestAccoplishment);
            SetConnectionId(connectionId);
            Situation = Sessionituation.NotStarted;
        }

        protected StudentTestSession()
        {
        }

        private void SetStudentTestAccoplishment(StudentTestAccoplishment studentTestAccoplishment)
        {
            if(studentTestAccoplishment is null || !studentTestAccoplishment.Validate.IsValid)
            {
                Validate.SetErrorMessage("A identificação da conexão deve ser informada.");
                return;
            }

            StudentTestAccoplishment = studentTestAccoplishment;
            StudentTestAccoplishment_Id = studentTestAccoplishment.Id;
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

        public void End(bool byLostConnection, DateTime? endDate = null)
        {
            EndDate = endDate ?? DateTime.Now;
            Situation = byLostConnection ? Sessionituation.DoneByLostConnection : Sessionituation.Done;
        }

        private TimeSpan GetTime()
        {
            if (EndDate is null) return default;
            return EndDate.GetValueOrDefault().Subtract(StartDate);
        }

        public bool Done() => Situation == Sessionituation.Done || Situation == Sessionituation.DoneByLostConnection;
    }
}