﻿using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Util;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestaoAvaliacao.Entities.StudentsTestSent
{
    public class StudentTestSent : EntityBase
    {
        public long TestId { get; private set; }
        public long TurId { get; private set; }
        public long AluId { get; private set; }
        public Guid EntId { get; private set; }
        public EnumSYS_Visao Vision { get; private set; }
        public StudentTestSentSituation Situation { get; private set; }

        public StudentTestSent(long testId, long turId, long aluId, Guid entId, EnumSYS_Visao vision)
            : base()
        {
            SetTestId(testId);
            SetTurId(turId);
            SetAluId(aluId);
            SetEntId(entId);
            Vision = vision;
            Situation = StudentTestSentSituation.Pending;
        }

        protected StudentTestSent()
        {
        }

        private void SetTestId(long testId)
        {
            if (testId <= 0)
            {
                Validate.SetErrorMessage("A prova deve ser informada.");
                return;
            }

            TestId = testId;
        }

        private void SetTurId(long turId)
        {
            if (turId <= 0)
            {
                Validate.SetErrorMessage("A turma deve ser informada.");
                return;
            }

            TurId = turId;
        }

        private void SetAluId(long aluId)
        {
            if (aluId <= 0)
            {
                Validate.SetErrorMessage("O aluno deve ser informado.");
                return;
            }

            AluId = aluId;
        }

        private void SetEntId(Guid entId)
        {
            if (entId == Guid.Empty)
            {
                Validate.SetErrorMessage("A entidade relacionada deve ser informada.");
                return;
            }

            EntId = entId;
        }

        public void SetPending()
        {
            if (Situation == StudentTestSentSituation.Done)
            {
                Validate.SetErrorMessage("O teste já foi processado e não pode ser colocado na fila de processamento novamente.");
                return;
            }

            Situation = StudentTestSentSituation.InProcess;
        }

        public void SetInProgress()
        {
            if (Situation == StudentTestSentSituation.Done)
            {
                Validate.SetErrorMessage("O teste já foi processado e não pode ser colocado na fila de processamento novamente.");
                return;
            }

            Situation = StudentTestSentSituation.InProcess;
        }

        public void SetDone() => Situation = StudentTestSentSituation.Done;
    }
}