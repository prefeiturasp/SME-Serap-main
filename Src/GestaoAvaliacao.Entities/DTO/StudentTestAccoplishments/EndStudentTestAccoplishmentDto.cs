using GestaoAvaliacao.Entities.DTO.Abstractions;

namespace GestaoAvaliacao.Entities.DTO.StudentTestAccoplishments
{
    public class EndStudentTestAccoplishmentDto : NotificableDto
    {
        public long AluId { get; set; }
        public long TurId { get; set; }
        public long TestId { get; set; }
        public string ConnectionId { get; set; }
    }
}