using GestaoAvaliacao.Entities.DTO.Abstractions;
using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Entities.DTO.StudentsTestSent
{
    public class FinalizeCorrectionDto : NotificableDto
    {
        public long TestId { get; set; }
        public long TurId { get; set; }
        public long AluId { get; set; }
        public Guid EntId { get; set; }
        public EnumSYS_Visao Visao { get; set; }
        public Guid UsuId { get; set; }
    }
}