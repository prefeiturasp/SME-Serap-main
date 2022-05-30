using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository.Map
{
    class TestTaiCurriculumGradeMap : EntityBaseMap<TestTaiCurriculumGrade>
    {
        public TestTaiCurriculumGradeMap()
        {
            ToTable("TestTaiCurriculumGrade");

            Property(p => p.MatrixId)
           .IsRequired();

            Property(p => p.DisciplineId)
         .IsRequired();

            Property(p => p.TypeCurriculumGradeId)
       .IsRequired();

            Property(p => p.Percentage)
   .IsRequired();

            Property(p => p.TestId)
   .IsRequired();
        }
    }
}