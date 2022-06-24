using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Dtos.ItemApi
{
    public class CurriculumGradeDto
    {
        public int TypeCurriculumGradeId { get; set; }
        public TypeCurriculumGradeDto TypeCurriculumGrade { get; set; }
    }

    public class TypeCurriculumGradeDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public byte Order { get; set; }
    }
}
