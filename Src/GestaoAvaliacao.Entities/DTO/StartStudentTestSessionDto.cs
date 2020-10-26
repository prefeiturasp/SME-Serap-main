using GestaoAvaliacao.Entities.DTO.Abstractions;
using System;

namespace GestaoAvaliacao.Entities.DTO
{
    public class StartStudentTestSessionDto : NotificableDto
    {
        public long AluId { get; set; }
        public long TurId { get; set; }
        public long TestId { get; set; }
        public Guid UsuId { get; set; }
        public string ConnectionId { get; set; }
    }
}