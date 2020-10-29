using GestaoAvaliacao.Entities.DTO.Abstractions;

namespace GestaoAvaliacao.Entities.DTO.StudentTestAccoplishments
{
    public class EndStudentTestSessionDto : NotificableDto
    {
        public string ConnectionId { get; set; }
        public bool ByLostConnection { get; set; }
    }
}