using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Entities
{
    public class TestTaiCurriculumGrade : EntityBase
    {

        [Column("Discipline_Id")]
        public virtual long DisciplineId { get; set; }
        [Column("EvaluationMatrix_Id")]
        public virtual long MatrixId { get; set; }
        [Column("TypeCurriculumGradeId")]
        public virtual long TypeCurriculumGradeId { get; set; }
        [Column("Percentage")]
        public virtual int Percentage { get; set; }

        [Column("TestId")]
        public virtual long TestId { get; set; }




    }
}
