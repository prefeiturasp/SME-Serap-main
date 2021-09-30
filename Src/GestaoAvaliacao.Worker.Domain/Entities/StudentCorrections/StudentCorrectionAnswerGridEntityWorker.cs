using System.Collections.Generic;

namespace GestaoAvaliacao.Worker.Domain.Entities.StudentCorrections
{
    public class StudentCorrectionAnswerGridEntityWorker
    {
        public int Order { get; set; }
        public long Item_Id { get; set; }
        public List<StudentCorrectionAnswerGridAlternativesWorker> Alternatives { get; set; }
        public bool Null { get; set; }
        public bool StrikeThrough { get; set; }
    }

    public class StudentCorrectionAnswerGridAlternativesWorker
    {
        public long Id { get; set; }
        public int Order { get; set; }
        public string Numeration { get; set; }
        public bool Selected { get; set; }
    }
}