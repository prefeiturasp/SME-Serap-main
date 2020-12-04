using GestaoAvaliacao.Util;
using GestaoAvaliacao.Worker.Domain.Base;
using System;

namespace GestaoAvaliacao.Worker.Domain.Entities.Tests
{
    public class StudentTestSentEntityWorker : EntityWorkerBase
    {
        public long TestId { get; private set; }
        public long TurId { get; private set; }
        public long AluId { get; private set; }
        public Guid EntId { get; private set; }
        public EnumSYS_Visao Vision { get; private set; }
        public StudentTestSentSituation Situation { get; private set; }

        protected StudentTestSentEntityWorker()
        {
        }

        public void SetPending()
        {
            if (Situation == StudentTestSentSituation.Done)
            {
                AddError("O teste já foi processado e não pode ser colocado na fila de processamento novamente.");
                return;
            }

            Situation = StudentTestSentSituation.InProcess;
        }

        public void SetInProgress()
        {
            if (Situation == StudentTestSentSituation.Done)
            {
                AddError("O teste já foi processado e não pode ser colocado na fila de processamento novamente.");
                return;
            }

            Situation = StudentTestSentSituation.InProcess;
        }

        public void SetDone() => Situation = StudentTestSentSituation.Done;
    }
}