using System.Collections.Generic;

namespace GestaoAvaliacao.Entities.DTO
{
    public class StudentInformationAndPerformanceTestResult
    {
        public class Student
        {
            public Student()
            {
                Items = new List<Item>();
            }
            public long Alu_id { get; set; }
            public int Mtu_numeroChamada { get; set; }
            public string Alu_nome { get; set; }
            public int? Hits { get; set; }
            public double? Avg { get; set; }
            public List<Item> Items { get; set; }
        }

        public class Item
        {
            public long Item_Id { get; set; }
            public int Order { get; set; }
            public string CorrectAlternative { get; set; }
            public string SkillDescription { get; set; }
            public string SkillCode { get; set; }
            public string ChosenAlternative { get; set; }
            public bool Correct { get; set; }
            public double? AvgSME { get; set; }
            public double? AvgDRE { get; set; }
            public bool Revoked { get; set; }
        }
    }
}
