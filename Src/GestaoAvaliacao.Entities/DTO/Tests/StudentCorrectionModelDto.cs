using System.Collections.Generic;

namespace GestaoAvaliacao.Entities.DTO.Tests
{
    public class StudentCorrectionModelDto
    {
        public long AluId { get; set; }
        public long TurId { get; set; }
        public long TestId { get; set; }
        public bool Done { get; set; }
        public int? LastAnswer { get; set; }
        public IEnumerable<AnswerModelDto> Answers { get; set; }
    }
}